using Microsoft.EntityFrameworkCore;
using NexusRetail.Core.Api.Data;
using NexusRetail.Core.Api.Services.Payment;
using NexusRetail.Shared.DTOs.BanHang;
using NexusRetail.Shared.Entities;

namespace NexusRetail.Core.Api.BUS;

public interface IBanHangBUS
{
    Task<string> GetNextMaHdbAsync();
    Task<BusResult<HoaDonBanResponseDto>> ThanhToanAsync(HoaDonBanDto dto);
    Task<BusResult<object>> TraCuuDonHangAsync(string sdt, bool maskPii = true);
    Task<object> LayDanhSachAsync(int page, int pageSize, string? q);
    Task<BusResult<object>> LayChiTietAsync(string id);
    Task<BusResult<object>> CapNhatTrangThaiAsync(string id, CapNhatTrangThaiDonDto dto);
    Task<object> LayThongKeDashboardAsync();
}

/// <summary>
/// Tầng nghiệp vụ (BUS) xử lý quy trình bán hàng POS/Online, kiểm tra tồn kho và giao dịch.
/// </summary>
public class BanHangBUS : IBanHangBUS
{
    private static readonly string[] AllowedOrderStatuses = ["ChoXacNhan", "DangGiao", "HoanThanh", "DaHuy"];
    private static readonly string[] AllowedPaymentStatuses = ["ChuaThanhToan", "DaThanhToan", "ChuyenThieu"];

    private readonly RetailDbContext _db;
    private readonly ILogger<BanHangBUS>? _logger;
    private readonly IBankPaymentService? _paymentService;

    public BanHangBUS(
        RetailDbContext db,
        ILogger<BanHangBUS>? logger = null,
        IBankPaymentService? paymentService = null)
    {
        _db = db;
        _logger = logger;
        _paymentService = paymentService;
    }

    private static string NormalizePaymentMethod(string? method, bool isPosStaffOrder)
    {
        if (string.IsNullOrWhiteSpace(method))
            return isPosStaffOrder ? "Cash" : "COD";

        var trimmed = method.Trim();
        if (trimmed.Equals("Cash", StringComparison.OrdinalIgnoreCase) || trimmed.Contains("Tiền mặt", StringComparison.OrdinalIgnoreCase))
            return isPosStaffOrder ? "Cash" : "COD";
        if (trimmed.Equals("VietQR", StringComparison.OrdinalIgnoreCase) || trimmed.Contains("QR", StringComparison.OrdinalIgnoreCase))
            return "VietQR";
        if (trimmed.Equals("Card", StringComparison.OrdinalIgnoreCase) || trimmed.Contains("Thẻ", StringComparison.OrdinalIgnoreCase))
            return "Card";
        if (trimmed.Equals("COD", StringComparison.OrdinalIgnoreCase))
            return "COD";

        return isPosStaffOrder ? "Cash" : "COD";
    }

    private static string MaskPhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone) || phone.Length < 7) return "***";
        return $"{phone[..3]}****{phone[^3..]}";
    }

    private static string MaskName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "Khách hàng";
        var parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Join(" ", parts.Select(p => p.Length <= 2 ? p[0] + "*" : p[..2] + new string('*', Math.Min(3, p.Length - 2))));
    }

    private async Task<string> SinhMaKhachHangAsync()
    {
        var allCodes = await _db.KhachHangs
            .Where(x => x.MaKhach.StartsWith("KH"))
            .Select(x => x.MaKhach)
            .ToListAsync();

        return CodeSequenceHelper.GenerateNextCode(allCodes, "KH");
    }

    public async Task<string> GetNextMaHdbAsync()
    {
        var allCodes = await _db.HoaDonBans
            .Where(x => x.MaHdb.StartsWith("HDB"))
            .Select(x => x.MaHdb)
            .ToListAsync();

        return CodeSequenceHelper.GenerateNextCode(allCodes, "HDB");
    }

    public async Task<BusResult<HoaDonBanResponseDto>> ThanhToanAsync(HoaDonBanDto dto)
    {
        if (dto.DanhSachMon == null || dto.DanhSachMon.Count == 0)
        {
            return BusResult<HoaDonBanResponseDto>.BadRequest("Hóa đơn bán hàng phải có ít nhất 1 sản phẩm!");
        }

        if (dto.DanhSachMon.Any(x => x.SoLuong <= 0))
        {
            return BusResult<HoaDonBanResponseDto>.BadRequest("Số lượng mua của từng sản phẩm phải lớn hơn 0!");
        }

        if (dto.DanhSachMon.Any(x => x.KhuyenMai < 0 || x.KhuyenMai > 100))
        {
            return BusResult<HoaDonBanResponseDto>.BadRequest("Mức chiết khấu / khuyến mãi phải từ 0% đến 100%!");
        }

        // Gộp các dòng trùng (MaSP, MaMau) có cùng mức khuyến mãi để tránh lỗi khóa chính kép (MaHDB, MaSP, MaMau)
        var mergedItems = new List<ChiTietHdbDto>();
        foreach (var group in dto.DanhSachMon.GroupBy(x => (MaSp: x.MaSp.Trim().ToUpperInvariant(), MaMau: string.IsNullOrWhiteSpace(x.MaMau) ? "DEFAULT" : x.MaMau.Trim().ToUpperInvariant())))
        {
            var first = group.First();
            if (group.Any(x => x.KhuyenMai != first.KhuyenMai))
            {
                return BusResult<HoaDonBanResponseDto>.BadRequest($"Sản phẩm '{first.MaSp}' màu '{first.MaMau}' bị trùng dòng với mức chiết khấu khác nhau!");
            }

            mergedItems.Add(new ChiTietHdbDto
            {
                MaSp = first.MaSp.Trim(),
                MaMau = string.IsNullOrWhiteSpace(first.MaMau) ? "DEFAULT" : first.MaMau.Trim(),
                SoLuong = group.Sum(x => x.SoLuong),
                KhuyenMai = first.KhuyenMai
            });
        }

        using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            var sdtKhach = dto.DienThoai?.Trim() ?? "";
            var maKhach = dto.MaKhach?.Trim();
            var diaChiGiao = dto.DiaChiGiaoHang?.Trim();

            var khachHang = await _db.KhachHangs.FirstOrDefaultAsync(x =>
                (!string.IsNullOrEmpty(sdtKhach) && x.DienThoai == sdtKhach) ||
                (!string.IsNullOrEmpty(maKhach) && x.MaKhach == maKhach));

            if (khachHang == null)
            {
                khachHang = new KhachHang
                {
                    MaKhach = await SinhMaKhachHangAsync(),
                    TenKhach = string.IsNullOrWhiteSpace(dto.TenKhachVangLai) ? "Khách Mua Online" : dto.TenKhachVangLai.Trim(),
                    DienThoai = string.IsNullOrWhiteSpace(sdtKhach) ? "0900000000" : sdtKhach,
                    DiaChi = string.IsNullOrWhiteSpace(diaChiGiao) ? "Tại quầy" : diaChiGiao
                };

                _db.KhachHangs.Add(khachHang);
                await _db.SaveChangesAsync();
            }
            else if (!string.IsNullOrWhiteSpace(diaChiGiao) &&
                     (string.IsNullOrWhiteSpace(khachHang.DiaChi) || khachHang.DiaChi == "Việt Nam" || khachHang.DiaChi == "Tại quầy"))
            {
                khachHang.DiaChi = diaChiGiao;
            }

            string? maNv = null;
            string tenNv = "Khách Online";
            if (!string.IsNullOrWhiteSpace(dto.MaNv))
            {
                var reqNv = dto.MaNv.Trim();
                var nvEntity = await _db.NhanViens.FirstOrDefaultAsync(x => x.MaNv == reqNv);
                if (nvEntity == null)
                {
                    string? aliasCode = reqNv.ToLowerInvariant() switch
                    {
                        "admin" => "NV01",
                        "nhanvien" => "NV02",
                        _ => null
                    };
                    if (aliasCode != null)
                    {
                        nvEntity = await _db.NhanViens.FirstOrDefaultAsync(x => x.MaNv == aliasCode);
                    }
                }

                if (nvEntity == null)
                {
                    await tx.RollbackAsync();
                    return BusResult<HoaDonBanResponseDto>.BadRequest($"Không tìm thấy nhân viên mã '{dto.MaNv}'!");
                }

                maNv = nvEntity.MaNv;
                tenNv = nvEntity.TenNv;
            }

            bool isPosOrder = !string.IsNullOrEmpty(maNv);
            string paymentMethod = NormalizePaymentMethod(dto.PhuongThucThanhToan, isPosOrder);
            string orderStatus = isPosOrder ? "HoanThanh" : "ChoXacNhan";
            string paymentStatus = paymentMethod is "Cash" or "Card" ? "DaThanhToan" : "ChuaThanhToan";
            string? maGiaoDich = paymentMethod == "Card"
                ? $"CARD_{DateTime.UtcNow:yyyyMMddHHmmss}_{Random.Shared.Next(1000, 9999)}"
                : null;

            string maHdb;
            if (!string.IsNullOrWhiteSpace(dto.MaHdb))
            {
                var reqMa = dto.MaHdb.Trim().ToUpper();
                bool daTonTai = await _db.HoaDonBans.AnyAsync(x => x.MaHdb == reqMa);
                maHdb = daTonTai ? await GetNextMaHdbAsync() : reqMa;
            }
            else
            {
                maHdb = await GetNextMaHdbAsync();
            }

            decimal tongTienHoaDon = 0m;
            var responseItems = new List<ChiTietHdbDto>();

            foreach (var item in mergedItems)
            {
                var sp = await _db.SanPhams.FirstOrDefaultAsync(x => x.MaSp == item.MaSp);
                if (sp == null)
                {
                    await tx.RollbackAsync();
                    return BusResult<HoaDonBanResponseDto>.BadRequest($"Không tìm thấy sản phẩm có mã {item.MaSp}");
                }

                // Kiểm tra biến thể màu sắc
                string maMau = string.IsNullOrWhiteSpace(item.MaMau) ? "DEFAULT" : item.MaMau.Trim();
                var spMau = await _db.SanPhamMauSacs
                    .Include(sm => sm.MaMauNavigation)
                    .FirstOrDefaultAsync(x => x.MaSp == item.MaSp && x.MaMau == maMau);

                if (spMau == null && maMau != "DEFAULT")
                {
                    await tx.RollbackAsync();
                    return BusResult<HoaDonBanResponseDto>.BadRequest($"Không tìm thấy màu '{maMau}' của sản phẩm '{sp.TenSp}'!");
                }

                decimal donGia = spMau?.GiaBan ?? sp.GiaBan;
                string tenMau = spMau?.MaMauNavigation?.TenMau ?? "Tiêu chuẩn";
                string? maHex = spMau?.MaMauNavigation?.MaHex;

                if (spMau != null && spMau.SoLuong < item.SoLuong)
                {
                    await tx.RollbackAsync();
                    return BusResult<HoaDonBanResponseDto>.BadRequest($"Sản phẩm '{sp.TenSp}' màu '{tenMau}' chỉ còn {spMau.SoLuong} chiếc trong kho, không đủ để bán!");
                }

                if (sp.SoLuong < item.SoLuong)
                {
                    await tx.RollbackAsync();
                    return BusResult<HoaDonBanResponseDto>.BadRequest($"Sản phẩm '{sp.TenSp}' chỉ còn {sp.SoLuong} chiếc trong kho, không đủ để bán!");
                }

                // InMemory database does not execute SQL Server trigger trg_ChiTietHDB_Change.
                if (_db.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
                {
                    if (spMau != null) spMau.SoLuong -= item.SoLuong;
                    sp.SoLuong -= item.SoLuong;
                }

                var thanhTien = Math.Round(item.SoLuong * donGia * (1m - (item.KhuyenMai / 100m)), 0);
                var chiTietHdb = new ChiTietHdb
                {
                    MaHdb = maHdb,
                    MaSp = sp.MaSp,
                    MaMau = maMau,
                    SoLuong = item.SoLuong,
                    DonGia = donGia,
                    KhuyenMai = item.KhuyenMai,
                    ThanhTien = thanhTien
                };
                tongTienHoaDon += thanhTien;
                _db.ChiTietHdbs.Add(chiTietHdb);

                responseItems.Add(new ChiTietHdbDto
                {
                    MaSp = sp.MaSp,
                    TenSp = sp.TenSp,
                    MaMau = maMau,
                    TenMau = tenMau,
                    MaHex = maHex,
                    SoLuong = item.SoLuong,
                    DonGia = donGia,
                    KhuyenMai = item.KhuyenMai,
                    ThanhTien = thanhTien
                });
            }

            decimal soTienDaThanhToan = paymentStatus == "DaThanhToan" ? tongTienHoaDon : 0m;
            if (paymentMethod == "VietQR" && _paymentService != null)
            {
                var existingPayment = _paymentService.GetPaymentStatus(maHdb, tongTienHoaDon);
                if (existingPayment.IsPaid && existingPayment.PaidAmount >= tongTienHoaDon)
                {
                    paymentStatus = "DaThanhToan";
                    soTienDaThanhToan = existingPayment.PaidAmount;
                    maGiaoDich = existingPayment.TransactionCode;
                }
                else if (existingPayment.PaidAmount > 0)
                {
                    paymentStatus = "ThanhToanMotPhan";
                    soTienDaThanhToan = existingPayment.PaidAmount;
                    maGiaoDich = existingPayment.TransactionCode;
                }
            }

            var effectiveAddress = !string.IsNullOrWhiteSpace(diaChiGiao) ? diaChiGiao : khachHang.DiaChi;
            var hdb = new HoaDonBan
            {
                MaHdb = maHdb,
                MaNv = maNv,
                NgayBan = DateTime.Now,
                MaKhach = khachHang.MaKhach,
                TongTien = tongTienHoaDon,
                PhuongThucThanhToan = paymentMethod,
                TrangThaiThanhToan = paymentStatus,
                TrangThaiDonHang = orderStatus,
                DiaChiGiaoHang = effectiveAddress,
                GhiChu = string.IsNullOrWhiteSpace(dto.GhiChu) ? null : dto.GhiChu.Trim(),
                MaGiaoDich = maGiaoDich,
                SoTienDaThanhToan = soTienDaThanhToan
            };

            _db.HoaDonBans.Add(hdb);
            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            if (paymentMethod == "VietQR" && paymentStatus != "DaThanhToan")
            {
                _paymentService?.RegisterExpectedAmount(maHdb, tongTienHoaDon);
            }

            var response = new HoaDonBanResponseDto
            {
                MaHdb = maHdb,
                MaNv = maNv,
                TenNv = tenNv,
                NgayBan = hdb.NgayBan,
                TongTien = tongTienHoaDon,
                PhuongThucThanhToan = paymentMethod,
                TrangThaiThanhToan = paymentStatus,
                TrangThaiDonHang = orderStatus,
                DiaChiGiaoHang = effectiveAddress,
                GhiChu = hdb.GhiChu,
                SanPham = responseItems
            };

            return BusResult<HoaDonBanResponseDto>.Ok(response);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            _logger?.LogError(ex, "[BanHangBUS] Lỗi xử lý thanh toán hóa đơn.");
            return BusResult<HoaDonBanResponseDto>.Failure("Có lỗi xảy ra khi xử lý thanh toán. Vui lòng thử lại sau.");
        }
    }

    public async Task<BusResult<object>> TraCuuDonHangAsync(string sdt, bool maskPii = true)
    {
        if (string.IsNullOrWhiteSpace(sdt))
        {
            return BusResult<object>.BadRequest("Số điện thoại không được để trống!");
        }

        sdt = sdt.Trim();

        var orders = await _db.HoaDonBans
            .AsNoTracking()
            .Include(x => x.MaKhachNavigation)
            .Include(x => x.ChiTietHdbs)
                .ThenInclude(ct => ct.MaSpNavigation)
            .Where(x => x.MaKhachNavigation != null && x.MaKhachNavigation.DienThoai == sdt)
            .OrderByDescending(x => x.NgayBan)
            .ToListAsync();

        var list = orders.Select(x => new
        {
            x.MaHdb,
            x.NgayBan,
            x.TongTien,
            x.PhuongThucThanhToan,
            x.TrangThaiThanhToan,
            x.TrangThaiDonHang,
            TenKhach = maskPii ? MaskName(x.MaKhachNavigation?.TenKhach) : (x.MaKhachNavigation?.TenKhach ?? ""),
            DienThoai = maskPii ? MaskPhone(x.MaKhachNavigation?.DienThoai) : (x.MaKhachNavigation?.DienThoai ?? ""),
            DanhSachMon = x.ChiTietHdbs.Select(ct => new
            {
                ct.MaSp,
                TenSp = ct.MaSpNavigation?.TenSp ?? ct.MaSp,
                ct.SoLuong,
                DonGia = ct.DonGia > 0 ? ct.DonGia : (ct.MaSpNavigation?.GiaBan ?? (ct.SoLuong > 0 ? Math.Round(ct.ThanhTien / ct.SoLuong, 0) : 0m)),
                ct.ThanhTien,
                ThoiGianBaoHanh = ct.MaSpNavigation?.ThoiGianBaoHanh ?? 12,
                HanBaoHanh = x.NgayBan.AddMonths(ct.MaSpNavigation?.ThoiGianBaoHanh ?? 12)
            }).ToList()
        }).ToList();

        return BusResult<object>.Ok(list);
    }

    public async Task<object> LayDanhSachAsync(int page, int pageSize, string? q)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _db.HoaDonBans
            .AsNoTracking()
            .Include(x => x.MaKhachNavigation)
            .Include(x => x.MaNvNavigation)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim();
            query = query.Where(x =>
                x.MaHdb.Contains(q) ||
                (x.MaKhachNavigation != null && (x.MaKhachNavigation.TenKhach.Contains(q) || x.MaKhachNavigation.DienThoai.Contains(q))) ||
                (x.MaNvNavigation != null && x.MaNvNavigation.TenNv.Contains(q)) ||
                x.TrangThaiDonHang.Contains(q));
        }

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        var items = await query
            .OrderByDescending(x => x.NgayBan)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new
            {
                maHdb = x.MaHdb,
                ngayBan = x.NgayBan,
                tongTien = x.TongTien,
                tenKhach = x.MaKhachNavigation != null ? x.MaKhachNavigation.TenKhach : "Khách Lẻ",
                dienThoai = x.MaKhachNavigation != null ? x.MaKhachNavigation.DienThoai : "",
                tenNv = x.MaNvNavigation != null ? x.MaNvNavigation.TenNv : "Khách Online",
                phuongThucThanhToan = x.PhuongThucThanhToan,
                trangThaiThanhToan = x.TrangThaiThanhToan,
                trangThaiDonHang = x.TrangThaiDonHang,
                diaChiGiaoHang = x.DiaChiGiaoHang ?? ""
            })
            .ToListAsync();

        return new
        {
            page,
            pageSize,
            totalItems,
            totalPages,
            items
        };
    }

    public async Task<BusResult<object>> LayChiTietAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BusResult<object>.BadRequest("Mã hóa đơn không được để trống!");
        }

        var hdb = await _db.HoaDonBans
            .AsNoTracking()
            .Include(x => x.MaKhachNavigation)
            .Include(x => x.MaNvNavigation)
            .Include(x => x.ChiTietHdbs)
                .ThenInclude(ct => ct.MaSpNavigation)
            .FirstOrDefaultAsync(x => x.MaHdb == id);

        if (hdb == null)
        {
            return BusResult<object>.NotFound($"Không tìm thấy hóa đơn {id}");
        }

        return BusResult<object>.Ok(new
        {
            maHdb = hdb.MaHdb,
            ngayBan = hdb.NgayBan,
            tongTien = hdb.TongTien,
            tenKhach = hdb.MaKhachNavigation?.TenKhach ?? "Khách Lẻ",
            dienThoai = hdb.MaKhachNavigation?.DienThoai ?? "",
            tenNv = hdb.MaNvNavigation?.TenNv ?? "Khách Online",
            phuongThucThanhToan = hdb.PhuongThucThanhToan,
            trangThaiThanhToan = hdb.TrangThaiThanhToan,
            trangThaiDonHang = hdb.TrangThaiDonHang,
            diaChiGiaoHang = hdb.DiaChiGiaoHang ?? hdb.MaKhachNavigation?.DiaChi ?? "",
            ghiChu = hdb.GhiChu ?? "",
            soTienDaThanhToan = hdb.SoTienDaThanhToan,
            maGiaoDich = hdb.MaGiaoDich ?? "",
            chiTiet = hdb.ChiTietHdbs.Select(ct => new
            {
                maSp = ct.MaSp,
                tenSp = ct.MaSpNavigation?.TenSp ?? ct.MaSp,
                soLuong = ct.SoLuong,
                donGia = ct.DonGia > 0
                    ? ct.DonGia
                    : (ct.MaSpNavigation?.GiaBan ?? (ct.SoLuong > 0 ? Math.Round(ct.ThanhTien / ct.SoLuong, 0) : 0m)),
                khuyenMai = ct.KhuyenMai,
                thanhTien = ct.ThanhTien
            }).ToList()
        });
    }

    public async Task<BusResult<object>> CapNhatTrangThaiAsync(string id, CapNhatTrangThaiDonDto dto)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BusResult<object>.BadRequest("Mã hóa đơn không hợp lệ.");

        var hdb = await _db.HoaDonBans
            .Include(x => x.ChiTietHdbs)
            .FirstOrDefaultAsync(x => x.MaHdb == id.Trim());

        if (hdb == null)
            return BusResult<object>.NotFound($"Không tìm thấy hóa đơn '{id}'.");

        if (!string.IsNullOrWhiteSpace(dto.TrangThaiDonHang))
        {
            var nextStatus = dto.TrangThaiDonHang.Trim();
            if (!AllowedOrderStatuses.Contains(nextStatus, StringComparer.Ordinal))
                return BusResult<object>.BadRequest("Trạng thái đơn hàng không hợp lệ.");

            if (hdb.TrangThaiDonHang == "DaHuy" && nextStatus != "DaHuy")
                return BusResult<object>.BadRequest("Đơn hàng đã hủy, không thể khôi phục trạng thái!");

            if (hdb.TrangThaiDonHang != "DaHuy" && nextStatus == "DaHuy")
            {
                using var tx = await _db.Database.BeginTransactionAsync();
                try
                {
                    foreach (var ct in hdb.ChiTietHdbs)
                    {
                        var sp = await _db.SanPhams.FirstOrDefaultAsync(x => x.MaSp == ct.MaSp);
                        if (sp != null)
                        {
                            sp.SoLuong += ct.SoLuong;
                        }
                    }

                    hdb.TrangThaiDonHang = "DaHuy";
                    await _db.SaveChangesAsync();
                    await tx.CommitAsync();
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    _logger?.LogError(ex, "[BanHangBUS] Lỗi hoàn kho khi hủy đơn {MaHdb}", id);
                    return BusResult<object>.Failure("Không thể hủy đơn hàng lúc này.");
                }
            }
            else
            {
                hdb.TrangThaiDonHang = nextStatus;
            }
        }

        if (!string.IsNullOrWhiteSpace(dto.TrangThaiThanhToan))
        {
            var nextPayStatus = dto.TrangThaiThanhToan.Trim();
            if (!AllowedPaymentStatuses.Contains(nextPayStatus, StringComparer.Ordinal))
                return BusResult<object>.BadRequest("Trạng thái thanh toán không hợp lệ.");

            hdb.TrangThaiThanhToan = nextPayStatus;
            if (nextPayStatus == "DaThanhToan")
            {
                hdb.SoTienDaThanhToan = Math.Max(hdb.SoTienDaThanhToan, hdb.TongTien);
            }
        }

        await _db.SaveChangesAsync();
        return BusResult<object>.Ok(new
        {
            maHdb = hdb.MaHdb,
            trangThaiDonHang = hdb.TrangThaiDonHang,
            trangThaiThanhToan = hdb.TrangThaiThanhToan,
            soTienDaThanhToan = hdb.SoTienDaThanhToan
        });
    }

    public async Task<object> LayThongKeDashboardAsync()
    {
        var totalProducts = await _db.SanPhams.CountAsync();
        var activeOrders = _db.HoaDonBans.Where(x => x.TrangThaiDonHang != "DaHuy");
        var totalOrders = await activeOrders.CountAsync();
        var totalRevenue = await activeOrders.SumAsync(x => (decimal?)x.TongTien) ?? 0m;
        var totalCustomers = await _db.KhachHangs.CountAsync();

        return new
        {
            totalProducts,
            totalOrders,
            totalRevenue,
            totalCustomers
        };
    }
}

