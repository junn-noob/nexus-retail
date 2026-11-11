using Microsoft.EntityFrameworkCore;
using NexusRetail.Core.Api.Data;
using NexusRetail.Shared.DTOs.BanHang;
using NexusRetail.Shared.Entities;

namespace NexusRetail.Core.Api.BUS;

public interface IBanHangBUS
{
    Task<string> GetNextMaHdbAsync();
    Task<BusResult<HoaDonBanResponseDto>> ThanhToanAsync(HoaDonBanDto dto);
    Task<BusResult<object>> TraCuuDonHangAsync(string sdt);
    Task<object> LayDanhSachAsync(int page, int pageSize, string? q);
    Task<BusResult<object>> LayChiTietAsync(string id);
    Task<object> LayThongKeDashboardAsync();
}

/// <summary>
/// Tầng nghiệp vụ (BUS) xử lý quy trình bán hàng POS/Online, kiểm tra tồn kho và giao dịch.
/// </summary>
public class BanHangBUS : IBanHangBUS
{
    private readonly RetailDbContext _db;
    private readonly ILogger<BanHangBUS>? _logger;

    public BanHangBUS(RetailDbContext db, ILogger<BanHangBUS>? logger = null)
    {
        _db = db;
        _logger = logger;
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

        using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            var sdtKhach = dto.DienThoai?.Trim() ?? "";
            var maKhach = dto.MaKhach?.Trim();

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
                    DiaChi = "Việt Nam"
                };

                _db.KhachHangs.Add(khachHang);
                await _db.SaveChangesAsync();
            }

            var nvHopLe = await _db.NhanViens.AnyAsync(x => x.MaNv == dto.MaNv);
            var maNv = nvHopLe ? dto.MaNv : (await _db.NhanViens.Select(x => x.MaNv).FirstOrDefaultAsync() ?? "NV01");

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

            foreach (var item in dto.DanhSachMon)
            {
                var sp = await _db.SanPhams.FirstOrDefaultAsync(x => x.MaSp == item.MaSp);
                if (sp == null)
                {
                    await tx.RollbackAsync();
                    return BusResult<HoaDonBanResponseDto>.BadRequest($"Không tìm thấy sản phẩm có mã {item.MaSp}");
                }

                if (sp.SoLuong < item.SoLuong)
                {
                    await tx.RollbackAsync();
                    return BusResult<HoaDonBanResponseDto>.BadRequest($"Sản phẩm '{sp.TenSp}' chỉ còn {sp.SoLuong} chiếc trong kho, không đủ để bán!");
                }

                // InMemory database does not execute SQL Server trigger trg_ChiTietHDB_Change.
                if (_db.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
                {
                    sp.SoLuong -= item.SoLuong;
                }

                var thanhTien = item.SoLuong * sp.GiaBan * (1 - (item.KhuyenMai / 100m));
                var chiTietHdb = new ChiTietHdb
                {
                    MaHdb = maHdb,
                    MaSp = item.MaSp,
                    SoLuong = item.SoLuong,
                    KhuyenMai = item.KhuyenMai,
                    ThanhTien = thanhTien
                };
                tongTienHoaDon += thanhTien;
                _db.ChiTietHdbs.Add(chiTietHdb);

                responseItems.Add(new ChiTietHdbDto
                {
                    MaSp = item.MaSp,
                    TenSp = sp.TenSp,
                    SoLuong = item.SoLuong,
                    KhuyenMai = item.KhuyenMai
                });
            }

            var hdb = new HoaDonBan
            {
                MaHdb = maHdb,
                MaNv = maNv,
                NgayBan = DateTime.Now,
                MaKhach = khachHang.MaKhach,
                TongTien = tongTienHoaDon
            };

            _db.HoaDonBans.Add(hdb);
            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            var response = new HoaDonBanResponseDto
            {
                MaHdb = maHdb,
                MaNv = maNv,
                TenNv = await _db.NhanViens.Where(x => x.MaNv == maNv).Select(x => x.TenNv).FirstOrDefaultAsync() ?? "",
                NgayBan = hdb.NgayBan,
                TongTien = tongTienHoaDon,
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

    public async Task<BusResult<object>> TraCuuDonHangAsync(string sdt)
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
            TenKhach = x.MaKhachNavigation?.TenKhach ?? "",
            DienThoai = x.MaKhachNavigation?.DienThoai ?? "",
            DanhSachMon = x.ChiTietHdbs.Select(ct => new
            {
                ct.MaSp,
                TenSp = ct.MaSpNavigation?.TenSp ?? ct.MaSp,
                ct.SoLuong,
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
                (x.MaNvNavigation != null && x.MaNvNavigation.TenNv.Contains(q)));
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
                tenNv = x.MaNvNavigation != null ? x.MaNvNavigation.TenNv : "Khách Online"
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
            chiTiet = hdb.ChiTietHdbs.Select(ct => new
            {
                maSp = ct.MaSp,
                tenSp = ct.MaSpNavigation?.TenSp ?? ct.MaSp,
                soLuong = ct.SoLuong,
                donGia = ct.MaSpNavigation != null ? ct.MaSpNavigation.GiaBan : (ct.SoLuong > 0 ? ct.ThanhTien / ct.SoLuong : 0),
                khuyenMai = ct.KhuyenMai,
                thanhTien = ct.ThanhTien
            }).ToList()
        });
    }

    public async Task<object> LayThongKeDashboardAsync()
    {
        var totalProducts = await _db.SanPhams.CountAsync();
        var totalOrders = await _db.HoaDonBans.CountAsync();
        var totalRevenue = await _db.HoaDonBans.SumAsync(x => (decimal?)x.TongTien) ?? 0m;
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

