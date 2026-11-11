using Microsoft.EntityFrameworkCore;
using NexusRetail.Core.Api.Data;
using NexusRetail.Shared.DTOs.NhapHang;
using NexusRetail.Shared.Entities;

namespace NexusRetail.Core.Api.BUS;

public interface INhapHangBUS
{
    Task<string> GetNextMaHdnAsync();
    Task<object> LayDanhSachHoaDonNhapAsync();
    Task<object> LayDanhSachNhaCungCapAsync();
    Task<object> LayDanhSachNhanVienAsync();
    Task<BusResult<HoaDonNhapResponseDto>> LayChiTietHoaDonNhapAsync(string maHdn);
    Task<BusResult<HoaDonNhapResponseDto>> TaoHoaDonNhapAsync(HoaDonNhapDto dto);
}

/// <summary>
/// Tầng nghiệp vụ (BUS) quản lý nhập kho, cập nhật giá vốn và định giá bán niêm yết.
/// </summary>
public class NhapHangBUS : INhapHangBUS
{
    private readonly RetailDbContext _db;
    private readonly ILogger<NhapHangBUS>? _logger;

    public NhapHangBUS(RetailDbContext db, ILogger<NhapHangBUS>? logger = null)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<string> GetNextMaHdnAsync()
    {
        var allCodes = await _db.HoaDonNhaps
            .Where(x => x.MaHdn.StartsWith("HDN"))
            .Select(x => x.MaHdn)
            .ToListAsync();

        return CodeSequenceHelper.GenerateNextCode(allCodes, "HDN");
    }

    public async Task<object> LayDanhSachHoaDonNhapAsync()
    {
        return await _db.HoaDonNhaps
            .AsNoTracking()
            .OrderByDescending(x => x.NgayNhap)
            .Select(x => new
            {
                x.MaHdn,
                x.MaNv,
                TenNv = x.MaNvNavigation.TenNv,
                x.MaNcc,
                TenNcc = x.MaNccNavigation.TenNcc,
                x.NgayNhap,
                x.TongTien,
                SoMatHang = x.ChiTietHdns.Count
            })
            .ToListAsync();
    }

    public async Task<object> LayDanhSachNhaCungCapAsync()
    {
        return await _db.NhaCungCaps
            .AsNoTracking()
            .Select(x => new { x.MaNcc, x.TenNcc, x.DiaChi, x.DienThoai })
            .ToListAsync();
    }

    public async Task<object> LayDanhSachNhanVienAsync()
    {
        return await _db.NhanViens
            .AsNoTracking()
            .Select(x => new { x.MaNv, x.TenNv, x.DienThoai })
            .ToListAsync();
    }

    public async Task<BusResult<HoaDonNhapResponseDto>> LayChiTietHoaDonNhapAsync(string maHdn)
    {
        var hdn = await _db.HoaDonNhaps
            .AsNoTracking()
            .Include(x => x.MaNvNavigation)
            .Include(x => x.MaNccNavigation)
            .Include(x => x.ChiTietHdns)
                .ThenInclude(ct => ct.MaSpNavigation)
            .FirstOrDefaultAsync(x => x.MaHdn == maHdn);

        if (hdn == null)
        {
            return BusResult<HoaDonNhapResponseDto>.NotFound($"Không tìm thấy hóa đơn nhập mã {maHdn}");
        }

        var result = new HoaDonNhapResponseDto
        {
            MaHdn = hdn.MaHdn,
            MaNv = hdn.MaNv ?? "",
            TenNv = hdn.MaNvNavigation?.TenNv ?? "",
            MaNcc = hdn.MaNcc,
            TenNcc = hdn.MaNccNavigation?.TenNcc ?? "",
            NgayNhap = hdn.NgayNhap,
            TongTien = hdn.TongTien,
            SanPham = hdn.ChiTietHdns.Select(ct => new ChiTietHdnDto
            {
                MaSp = ct.MaSp,
                TenSp = ct.MaSpNavigation?.TenSp ?? "",
                SoLuong = ct.SoLuong,
                DonGia = ct.DonGia,
                KhuyenMai = ct.KhuyenMai,
                ThanhTien = ct.ThanhTien
            }).ToList()
        };

        return BusResult<HoaDonNhapResponseDto>.Ok(result);
    }

    public async Task<BusResult<HoaDonNhapResponseDto>> TaoHoaDonNhapAsync(HoaDonNhapDto dto)
    {
        if (dto.DanhSachMon == null || dto.DanhSachMon.Count == 0)
        {
            return BusResult<HoaDonNhapResponseDto>.BadRequest("Phiếu nhập kho phải có ít nhất 1 sản phẩm!");
        }

        if (dto.DanhSachMon.Any(x => x.SoLuong <= 0))
        {
            return BusResult<HoaDonNhapResponseDto>.BadRequest("Số lượng nhập của từng sản phẩm phải lớn hơn 0!");
        }

        if (dto.DanhSachMon.Any(x => x.DonGia <= 0))
        {
            return BusResult<HoaDonNhapResponseDto>.BadRequest("Đơn giá nhập của từng sản phẩm phải lớn hơn 0đ!");
        }

        if (dto.DanhSachMon.Any(x => x.KhuyenMai < 0 || x.KhuyenMai > 100))
        {
            return BusResult<HoaDonNhapResponseDto>.BadRequest("Chiết khấu nhập hàng phải từ 0% đến 100%!");
        }

        var nv = await _db.NhanViens.FindAsync(dto.MaNv);
        if (nv == null)
        {
            return BusResult<HoaDonNhapResponseDto>.BadRequest($"Không tìm thấy nhân viên mã '{dto.MaNv}'!");
        }

        var ncc = await _db.NhaCungCaps.FindAsync(dto.MaNcc);
        if (ncc == null)
        {
            return BusResult<HoaDonNhapResponseDto>.BadRequest($"Không tìm thấy nhà cung cấp mã '{dto.MaNcc}'!");
        }

        // Gộp dòng trùng MaSP nếu cùng đơn giá & chiết khấu, hoặc báo lỗi 400 nếu xung đột giá/chiết khấu
        var mergedItems = new List<ChiTietHdnDto>();
        foreach (var group in dto.DanhSachMon.GroupBy(x => x.MaSp.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            var first = group.First();
            bool conflictingTerms = group.Any(x => x.DonGia != first.DonGia || x.KhuyenMai != first.KhuyenMai);
            if (conflictingTerms)
            {
                return BusResult<HoaDonNhapResponseDto>.BadRequest($"Sản phẩm '{group.Key}' bị trùng dòng với đơn giá hoặc chiết khấu khác nhau!");
            }

            mergedItems.Add(new ChiTietHdnDto
            {
                MaSp = first.MaSp.Trim(),
                SoLuong = group.Sum(x => x.SoLuong),
                DonGia = first.DonGia,
                KhuyenMai = first.KhuyenMai
            });
        }

        using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var maHdn = await GetNextMaHdnAsync();
            decimal tongTienHoaDon = 0m;
            var chiTietList = new List<ChiTietHdnDto>();

            foreach (var item in mergedItems)
            {
                var sp = await _db.SanPhams.FirstOrDefaultAsync(x => x.MaSp == item.MaSp);
                if (sp == null)
                {
                    await tx.RollbackAsync();
                    return BusResult<HoaDonNhapResponseDto>.BadRequest($"Không tìm thấy sản phẩm mã '{item.MaSp}' để nhập hàng!");
                }

                // InMemory database does not execute SQL Server trigger trg_ChiTietHDN_Change.
                if (_db.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
                {
                    decimal netUnitCost = item.DonGia * (1m - item.KhuyenMai / 100m);
                    int oldQty = Math.Max(0, sp.SoLuong);
                    decimal weightedCost = oldQty > 0
                        ? Math.Round((oldQty * sp.GiaNhap + item.SoLuong * netUnitCost) / (oldQty + item.SoLuong), 0)
                        : Math.Round(netUnitCost, 0);

                    sp.SoLuong += item.SoLuong;
                    sp.GiaNhap = weightedCost;
                    sp.GiaBan = Math.Max(sp.GiaBan, SanPhamBUS.CalculateSellingPrice(weightedCost));
                }

                decimal thanhTien = item.SoLuong * item.DonGia * (1 - item.KhuyenMai / 100m);
                tongTienHoaDon += thanhTien;

                var chiTiet = new ChiTietHdn
                {
                    MaHdn = maHdn,
                    MaSp = item.MaSp,
                    SoLuong = item.SoLuong,
                    DonGia = item.DonGia,
                    KhuyenMai = item.KhuyenMai,
                    ThanhTien = thanhTien
                };
                _db.ChiTietHdns.Add(chiTiet);

                chiTietList.Add(new ChiTietHdnDto
                {
                    MaSp = item.MaSp,
                    TenSp = sp.TenSp,
                    SoLuong = item.SoLuong,
                    DonGia = item.DonGia,
                    KhuyenMai = item.KhuyenMai,
                    ThanhTien = thanhTien
                });
            }

            var hdn = new HoaDonNhap
            {
                MaHdn = maHdn,
                MaNv = dto.MaNv,
                MaNcc = dto.MaNcc,
                NgayNhap = DateTime.Now,
                TongTien = tongTienHoaDon
            };

            _db.HoaDonNhaps.Add(hdn);
            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            var response = new HoaDonNhapResponseDto
            {
                MaHdn = maHdn,
                MaNv = dto.MaNv,
                TenNv = nv.TenNv,
                MaNcc = dto.MaNcc,
                TenNcc = ncc.TenNcc,
                NgayNhap = hdn.NgayNhap,
                TongTien = tongTienHoaDon,
                SanPham = chiTietList
            };

            return BusResult<HoaDonNhapResponseDto>.Created(response);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            _logger?.LogError(ex, "[NhapHangBUS] Lỗi xử lý phiếu nhập kho.");
            return BusResult<HoaDonNhapResponseDto>.Failure("Có lỗi xảy ra khi xử lý phiếu nhập hàng. Vui lòng thử lại sau.");
        }
    }
}

