using System.Data;
using System.Linq.Expressions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NexusRetail.Core.Api.Data;
using NexusRetail.Shared.DTOs.Common;
using NexusRetail.Shared.DTOs.SanPham;
using NexusRetail.Shared.Entities;

namespace NexusRetail.Core.Api.BUS;

public record CreatedSanPhamResult(string MaSp, string TenSp, decimal GiaBan);

public interface ISanPhamBUS
{
    Task<PagedResultDto<SanPhamDto>> LayDanhSachAsync(int page, int pageSize, string? q, decimal? minPrice, decimal? maxPrice, string? brand);
    Task<SanPhamDto?> ChiTietSanPhamAsync(string maSp);
    Task<List<SanPhamMauSacDto>> LayDanhSachMauSacTheoSpAsync(string maSp);
    Task<List<MauSacDto>> LayTatCaMauSacAsync();
    Task<PagedResultDto<SanPhamDto>> LayTheoNhanHieuAsync(string maNhanHieu, int page, int pageSize);
    Task<object> LayBoLocDanhMucAsync();
    Task<List<SanPhamDto>> TimKiemAsync(string? keyword, string? tenLoai, decimal? minPrice, decimal? maxPrice);
    Task<BusResult<CreatedSanPhamResult>> ThemSanPhamAsync(ThemSanPhamDto dto);
    Task<BusResult<object?>> SuaSanPhamAsync(string maSp, SuaSanPhamDto dto);
    Task<BusResult<object?>> XoaSanPhamAsync(string maSp);
}

/// <summary>
/// Tầng nghiệp vụ (BUS) quản lý danh mục sản phẩm, định giá bán niêm yết và kiểm tra tồn kho.
/// </summary>
public class SanPhamBUS : ISanPhamBUS
{
    private readonly RetailDbContext _db;
    private readonly IAdoNetDataProvider? _adoNet;

    public SanPhamBUS(RetailDbContext db, IAdoNetDataProvider? adoNet = null)
    {
        _db = db;
        _adoNet = adoNet;
    }

    private bool UseAdoNet => _adoNet != null && _db.Database.IsSqlServer();

    /// <summary>
    /// Quy tắc định giá thống nhất: Giá bán tối thiểu = 110% Giá nhập (làm tròn đồng).
    /// </summary>
    public static decimal CalculateSellingPrice(decimal giaNhap, decimal? requestedGiaBan = null)
    {
        decimal minGiaBan = Math.Round(giaNhap * 1.10m, 0);
        return (requestedGiaBan.HasValue && requestedGiaBan.Value >= minGiaBan)
            ? requestedGiaBan.Value
            : minGiaBan;
    }

    private static readonly Expression<Func<VwDanhSachSanPham, SanPhamDto>> ProjectFromView = sp => new SanPhamDto
    {
        MaSp = sp.MaSp,
        TenSp = sp.TenSp,
        MaLoai = sp.MaLoai,
        TenLoai = sp.TenLoai,
        MaNhanHieu = sp.MaNhanHieu,
        TenNhanHieu = sp.TenNhanHieu,
        MaManHinh = sp.MaManHinh,
        TenManHinh = sp.TenManHinh,
        GiaNhap = sp.GiaNhap,
        GiaBan = sp.GiaBan,
        SoLuong = sp.SoLuong,
        ThoiGianBaoHanh = sp.ThoiGianBaoHanh,
        AmThanh = sp.AmThanh,
        ChupAnh = sp.ChupAnh,
        Anh = sp.Anh,
        GhiChu = sp.GhiChu,
        GiaBanThapNhat = sp.GiaBanThapNhat ?? sp.GiaBan,
        GiaBanCaoNhat = sp.GiaBanCaoNhat ?? sp.GiaBan
    };

    public async Task<PagedResultDto<SanPhamDto>> LayDanhSachAsync(
        int page,
        int pageSize,
        string? q,
        decimal? minPrice,
        decimal? maxPrice,
        string? brand)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _db.VwDanhSachSanPhams.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(brand))
        {
            brand = brand.Trim();
            query = query.Where(x => x.MaNhanHieu == brand || x.TenNhanHieu.Contains(brand));
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim();
            query = query.Where(x => x.TenSp.Contains(q) || x.MaSp.Contains(q));
        }

        if (minPrice.HasValue) query = query.Where(x => x.GiaBan >= minPrice.Value);
        if (maxPrice.HasValue) query = query.Where(x => x.GiaBan <= maxPrice.Value);

        var totalItems = await query.CountAsync();
        var items = await query
            .OrderBy(x => x.MaSp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ProjectFromView)
            .ToListAsync();

        if (items.Count > 0)
        {
            var maSpList = items.Select(x => x.MaSp).ToList();
            var colors = await _db.SanPhamMauSacs
                .Include(sm => sm.MaMauNavigation)
                .Where(sm => maSpList.Contains(sm.MaSp))
                .AsNoTracking()
                .ToListAsync();

            var colorsBySp = colors.GroupBy(x => x.MaSp).ToDictionary(g => g.Key, g => g.Select(c => new SanPhamMauSacDto
            {
                MaSp = c.MaSp,
                MaMau = c.MaMau,
                TenMau = c.MaMauNavigation != null ? c.MaMauNavigation.TenMau : c.MaMau,
                MaHex = c.MaMauNavigation?.MaHex,
                GiaNhap = c.GiaNhap,
                GiaBan = c.GiaBan,
                SoLuong = c.SoLuong,
                HinhAnh = c.HinhAnh
            }).OrderBy(c => c.GiaBan).ToList());

            foreach (var item in items)
            {
                if (colorsBySp.TryGetValue(item.MaSp, out var cList))
                {
                    item.DanhSachMauSac = cList;
                    if (cList.Count > 0)
                    {
                        item.GiaBanThapNhat = cList.Min(c => c.GiaBan);
                        item.GiaBanCaoNhat = cList.Max(c => c.GiaBan);
                    }
                }
            }
        }

        return new PagedResultDto<SanPhamDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
            Items = items
        };
    }

    public async Task<SanPhamDto?> ChiTietSanPhamAsync(string maSp)
    {
        var raw = await _db.SanPhams
            .Include(s => s.MaLoaiNavigation)
            .Include(s => s.MaNhanHieuNavigation)
            .Include(s => s.MaManHinhNavigation)
            .Include(s => s.SanPhamMauSacs)
                .ThenInclude(sm => sm.MaMauNavigation)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaSp == maSp);

        if (raw == null) return null;

        var colorList = raw.SanPhamMauSacs
            .OrderBy(c => c.GiaBan)
            .Select(c => new SanPhamMauSacDto
            {
                MaSp = c.MaSp,
                MaMau = c.MaMau,
                TenMau = c.MaMauNavigation != null ? c.MaMauNavigation.TenMau : c.MaMau,
                MaHex = c.MaMauNavigation?.MaHex,
                GiaNhap = c.GiaNhap,
                GiaBan = c.GiaBan,
                SoLuong = c.SoLuong,
                HinhAnh = c.HinhAnh
            }).ToList();

        decimal minPrice = colorList.Count > 0 ? colorList.Min(c => c.GiaBan) : raw.GiaBan;
        decimal maxPrice = colorList.Count > 0 ? colorList.Max(c => c.GiaBan) : raw.GiaBan;

        return new SanPhamDto
        {
            MaSp = raw.MaSp,
            TenSp = raw.TenSp,
            MaLoai = raw.MaLoai,
            TenLoai = raw.MaLoaiNavigation?.TenLoai ?? "",
            MaNhanHieu = raw.MaNhanHieu,
            TenNhanHieu = raw.MaNhanHieuNavigation?.TenNhanHieu ?? "",
            MaManHinh = raw.MaManHinh,
            TenManHinh = raw.MaManHinhNavigation?.TenManHinh ?? "",
            GiaNhap = raw.GiaNhap,
            GiaBan = raw.GiaBan,
            SoLuong = raw.SoLuong,
            ThoiGianBaoHanh = raw.ThoiGianBaoHanh,
            AmThanh = raw.AmThanh,
            ChupAnh = raw.ChupAnh,
            Anh = raw.Anh,
            GhiChu = raw.GhiChu,
            GiaBanThapNhat = minPrice,
            GiaBanCaoNhat = maxPrice,
            DanhSachMauSac = colorList
        };
    }

    public async Task<List<SanPhamMauSacDto>> LayDanhSachMauSacTheoSpAsync(string maSp)
    {
        var colors = await _db.SanPhamMauSacs
            .Include(sm => sm.MaMauNavigation)
            .Where(sm => sm.MaSp == maSp)
            .OrderBy(sm => sm.GiaBan)
            .AsNoTracking()
            .ToListAsync();

        return colors.Select(c => new SanPhamMauSacDto
        {
            MaSp = c.MaSp,
            MaMau = c.MaMau,
            TenMau = c.MaMauNavigation != null ? c.MaMauNavigation.TenMau : c.MaMau,
            MaHex = c.MaMauNavigation?.MaHex,
            GiaNhap = c.GiaNhap,
            GiaBan = c.GiaBan,
            SoLuong = c.SoLuong,
            HinhAnh = c.HinhAnh
        }).ToList();
    }

    public async Task<List<MauSacDto>> LayTatCaMauSacAsync()
    {
        return await _db.MauSacs
            .AsNoTracking()
            .OrderBy(m => m.TenMau)
            .Select(m => new MauSacDto
            {
                MaMau = m.MaMau,
                TenMau = m.TenMau,
                MaHex = m.MaHex
            })
            .ToListAsync();
    }

    public async Task<PagedResultDto<SanPhamDto>> LayTheoNhanHieuAsync(string maNhanHieu, int page, int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _db.VwDanhSachSanPhams
            .AsNoTracking()
            .Where(x => x.MaNhanHieu == maNhanHieu);

        var totalItems = await query.CountAsync();
        var items = await query
            .OrderBy(x => x.MaSp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ProjectFromView)
            .ToListAsync();

        return new PagedResultDto<SanPhamDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
            Items = items
        };
    }

    public async Task<object> LayBoLocDanhMucAsync()
    {
        var loais = await _db.Loais.AsNoTracking().Select(x => new { x.MaLoai, x.TenLoai }).ToListAsync();
        var nhanHieus = await _db.NhanHieus.AsNoTracking().Select(x => new { x.MaNhanHieu, x.TenNhanHieu }).ToListAsync();
        var manHinhs = await _db.ManHinhs.AsNoTracking().Select(x => new { x.MaManHinh, x.TenManHinh }).ToListAsync();

        return new
        {
            Loai = loais,
            NhanHieu = nhanHieus,
            ManHinh = manHinhs
        };
    }

    public async Task<List<SanPhamDto>> TimKiemAsync(string? keyword, string? tenLoai, decimal? minPrice, decimal? maxPrice)
    {
        if (UseAdoNet)
        {
            const string sql = @"
                SELECT sp.MaSP, sp.TenSP, sp.MaLoai, l.TenLoai, sp.MaNhanHieu, nh.TenNhanHieu,
                       sp.MaManHinh, mh.TenManHinh, sp.GiaNhap, sp.GiaBan, sp.SoLuong,
                       sp.ThoiGianBaoHanh, sp.AmThanh, sp.ChupAnh, sp.Anh, sp.GhiChu
                FROM SanPham sp
                LEFT JOIN Loai l ON sp.MaLoai = l.MaLoai
                LEFT JOIN NhanHieu nh ON sp.MaNhanHieu = nh.MaNhanHieu
                LEFT JOIN ManHinh mh ON sp.MaManHinh = mh.MaManHinh
                WHERE (@Keyword IS NULL OR sp.TenSP LIKE N'%' + @Keyword + N'%' OR sp.GhiChu LIKE N'%' + @Keyword + N'%')
                  AND (@TenLoai IS NULL OR l.TenLoai LIKE N'%' + @TenLoai + N'%')
                  AND (@MinPrice IS NULL OR sp.GiaBan >= @MinPrice)
                  AND (@MaxPrice IS NULL OR sp.GiaBan <= @MaxPrice)
                ORDER BY sp.MaSP";

            var parameters = new[]
            {
                new SqlParameter("@Keyword", SqlDbType.NVarChar, 150) { Value = string.IsNullOrWhiteSpace(keyword) ? DBNull.Value : keyword.Trim() },
                new SqlParameter("@TenLoai", SqlDbType.NVarChar, 100) { Value = string.IsNullOrWhiteSpace(tenLoai) ? DBNull.Value : tenLoai.Trim() },
                new SqlParameter("@MinPrice", SqlDbType.Decimal) { Value = minPrice.HasValue ? minPrice.Value : DBNull.Value },
                new SqlParameter("@MaxPrice", SqlDbType.Decimal) { Value = maxPrice.HasValue ? maxPrice.Value : DBNull.Value }
            };

            return _adoNet!.ExecuteReaderText(sql, r => new SanPhamDto
            {
                MaSp = r["MaSP"]?.ToString() ?? "",
                TenSp = r["TenSP"]?.ToString() ?? "",
                MaLoai = r["MaLoai"]?.ToString(),
                TenLoai = r["TenLoai"]?.ToString() ?? "",
                MaNhanHieu = r["MaNhanHieu"]?.ToString() ?? "",
                TenNhanHieu = r["TenNhanHieu"]?.ToString() ?? "",
                MaManHinh = r["MaManHinh"]?.ToString(),
                TenManHinh = r["TenManHinh"]?.ToString() ?? "",
                GiaNhap = Convert.ToDecimal(r["GiaNhap"]),
                GiaBan = Convert.ToDecimal(r["GiaBan"]),
                SoLuong = Convert.ToInt32(r["SoLuong"]),
                ThoiGianBaoHanh = Convert.ToInt32(r["ThoiGianBaoHanh"]),
                AmThanh = r["AmThanh"] == DBNull.Value ? null : r["AmThanh"].ToString(),
                ChupAnh = r["ChupAnh"] == DBNull.Value ? null : r["ChupAnh"].ToString(),
                Anh = r["Anh"] == DBNull.Value ? null : r["Anh"].ToString(),
                GhiChu = r["GhiChu"] == DBNull.Value ? null : r["GhiChu"].ToString()
            }, parameters);
        }

        var query = _db.SanPhams
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.Trim();
            query = query.Where(x => x.TenSp.Contains(kw) || (x.GhiChu != null && x.GhiChu.Contains(kw)));
        }
        if (!string.IsNullOrWhiteSpace(tenLoai))
        {
            var tl = tenLoai.Trim();
            query = query.Where(x => x.MaLoaiNavigation != null && x.MaLoaiNavigation.TenLoai.Contains(tl));
        }
        if (minPrice.HasValue) query = query.Where(x => x.GiaBan >= minPrice.Value);
        if (maxPrice.HasValue) query = query.Where(x => x.GiaBan <= maxPrice.Value);

        return await query
            .OrderBy(x => x.MaSp)
            .Select(sp => new SanPhamDto
            {
                MaSp = sp.MaSp,
                TenSp = sp.TenSp,
                MaLoai = sp.MaLoai,
                TenLoai = sp.MaLoaiNavigation != null ? sp.MaLoaiNavigation.TenLoai : "",
                MaNhanHieu = sp.MaNhanHieu,
                TenNhanHieu = sp.MaNhanHieuNavigation != null ? sp.MaNhanHieuNavigation.TenNhanHieu : "",
                MaManHinh = sp.MaManHinh,
                TenManHinh = sp.MaManHinhNavigation != null ? sp.MaManHinhNavigation.TenManHinh : "",
                GiaNhap = sp.GiaNhap,
                GiaBan = sp.GiaBan,
                SoLuong = sp.SoLuong,
                ThoiGianBaoHanh = sp.ThoiGianBaoHanh,
                AmThanh = sp.AmThanh,
                ChupAnh = sp.ChupAnh,
                Anh = sp.Anh,
                GhiChu = sp.GhiChu
            })
            .ToListAsync();
    }

    private async Task<string> SinhMaSanPhamAsync()
    {
        var allCodes = await _db.SanPhams
            .Where(x => x.MaSp.StartsWith("SP"))
            .Select(x => x.MaSp)
            .ToListAsync();

        return CodeSequenceHelper.GenerateNextCode(allCodes, "SP");
    }

    private async Task<string?> ValidateForeignKeysAsync(string maLoai, string maNhanHieu, string maManHinh)
    {
        if (!await _db.Loais.AnyAsync(x => x.MaLoai == maLoai))
            return $"Không tìm thấy loại sản phẩm mã '{maLoai}'!";
        if (!await _db.NhanHieus.AnyAsync(x => x.MaNhanHieu == maNhanHieu))
            return $"Không tìm thấy nhãn hiệu mã '{maNhanHieu}'!";
        if (!await _db.ManHinhs.AnyAsync(x => x.MaManHinh == maManHinh))
            return $"Không tìm thấy loại màn hình mã '{maManHinh}'!";
        return null;
    }

    public async Task<BusResult<CreatedSanPhamResult>> ThemSanPhamAsync(ThemSanPhamDto dto)
    {
        var fkError = await ValidateForeignKeysAsync(dto.MaLoai, dto.MaNhanHieu, dto.MaManHinh);
        if (fkError != null) return BusResult<CreatedSanPhamResult>.BadRequest(fkError);

        decimal giaBan = CalculateSellingPrice(dto.GiaNhap, dto.GiaBan);
        string maSp = await SinhMaSanPhamAsync();

        if (UseAdoNet)
        {
            const string sql = @"
                INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNhanHieu, MaManHinh, GiaNhap, GiaBan, SoLuong, ThoiGianBaoHanh, AmThanh, ChupAnh, Anh, GhiChu)
                VALUES (@MaSP, @TenSP, @MaLoai, @MaNhanHieu, @MaManHinh, @GiaNhap, @GiaBan, @SoLuong, @ThoiGianBaoHanh, @AmThanh, @ChupAnh, @Anh, @GhiChu)";

            var parameters = new[]
            {
                new SqlParameter("@MaSP", SqlDbType.NVarChar, 20) { Value = maSp },
                new SqlParameter("@TenSP", SqlDbType.NVarChar, 150) { Value = dto.TenSp },
                new SqlParameter("@MaLoai", SqlDbType.NVarChar, 20) { Value = dto.MaLoai },
                new SqlParameter("@MaNhanHieu", SqlDbType.NVarChar, 20) { Value = dto.MaNhanHieu },
                new SqlParameter("@MaManHinh", SqlDbType.NVarChar, 20) { Value = dto.MaManHinh },
                new SqlParameter("@GiaNhap", SqlDbType.Decimal) { Value = dto.GiaNhap },
                new SqlParameter("@GiaBan", SqlDbType.Decimal) { Value = giaBan },
                new SqlParameter("@SoLuong", SqlDbType.Int) { Value = dto.SoLuong },
                new SqlParameter("@ThoiGianBaoHanh", SqlDbType.Int) { Value = dto.ThoiGianBaoHanh },
                new SqlParameter("@AmThanh", SqlDbType.NVarChar, 100) { Value = (object?)dto.AmThanh ?? DBNull.Value },
                new SqlParameter("@ChupAnh", SqlDbType.NVarChar, 200) { Value = (object?)dto.ChupAnh ?? DBNull.Value },
                new SqlParameter("@Anh", SqlDbType.NVarChar, 250) { Value = (object?)dto.Anh ?? DBNull.Value },
                new SqlParameter("@GhiChu", SqlDbType.NVarChar) { Value = (object?)dto.GhiChu ?? DBNull.Value }
            };

            _adoNet!.ExecuteNonQueryText(sql, parameters);
            return BusResult<CreatedSanPhamResult>.Created(new CreatedSanPhamResult(maSp, dto.TenSp, giaBan));
        }

        var sp = new SanPham
        {
            MaSp = maSp,
            TenSp = dto.TenSp,
            MaLoai = dto.MaLoai,
            MaNhanHieu = dto.MaNhanHieu,
            MaManHinh = dto.MaManHinh,
            GiaNhap = dto.GiaNhap,
            GiaBan = giaBan,
            SoLuong = dto.SoLuong,
            ThoiGianBaoHanh = dto.ThoiGianBaoHanh,
            AmThanh = dto.AmThanh,
            ChupAnh = dto.ChupAnh,
            Anh = dto.Anh,
            GhiChu = dto.GhiChu
        };

        _db.SanPhams.Add(sp);
        _db.SanPhamMauSacs.Add(new SanPhamMauSac
        {
            MaSp = maSp,
            MaMau = "DEFAULT",
            GiaNhap = dto.GiaNhap,
            GiaBan = giaBan,
            SoLuong = dto.SoLuong,
            HinhAnh = dto.Anh
        });
        await _db.SaveChangesAsync();

        return BusResult<CreatedSanPhamResult>.Created(new CreatedSanPhamResult(sp.MaSp, sp.TenSp, sp.GiaBan));
    }

    public async Task<BusResult<object?>> SuaSanPhamAsync(string maSp, SuaSanPhamDto dto)
    {
        if (!await _db.SanPhams.AnyAsync(x => x.MaSp == maSp))
            return BusResult<object?>.NotFound();

        var fkError = await ValidateForeignKeysAsync(dto.MaLoai, dto.MaNhanHieu, dto.MaManHinh);
        if (fkError != null) return BusResult<object?>.BadRequest(fkError);

        decimal giaBan = CalculateSellingPrice(dto.GiaNhap, dto.GiaBan);

        if (UseAdoNet)
        {
            const string sql = @"
                UPDATE SanPham
                SET TenSP = @TenSP, MaLoai = @MaLoai, MaNhanHieu = @MaNhanHieu, MaManHinh = @MaManHinh,
                    GiaNhap = @GiaNhap, GiaBan = @GiaBan, SoLuong = @SoLuong, ThoiGianBaoHanh = @ThoiGianBaoHanh,
                    AmThanh = @AmThanh, ChupAnh = @ChupAnh, Anh = @Anh, GhiChu = @GhiChu
                WHERE MaSP = @MaSP";

            var parameters = new[]
            {
                new SqlParameter("@MaSP", SqlDbType.NVarChar, 20) { Value = maSp },
                new SqlParameter("@TenSP", SqlDbType.NVarChar, 150) { Value = dto.TenSp },
                new SqlParameter("@MaLoai", SqlDbType.NVarChar, 20) { Value = dto.MaLoai },
                new SqlParameter("@MaNhanHieu", SqlDbType.NVarChar, 20) { Value = dto.MaNhanHieu },
                new SqlParameter("@MaManHinh", SqlDbType.NVarChar, 20) { Value = dto.MaManHinh },
                new SqlParameter("@GiaNhap", SqlDbType.Decimal) { Value = dto.GiaNhap },
                new SqlParameter("@GiaBan", SqlDbType.Decimal) { Value = giaBan },
                new SqlParameter("@SoLuong", SqlDbType.Int) { Value = dto.SoLuong },
                new SqlParameter("@ThoiGianBaoHanh", SqlDbType.Int) { Value = dto.ThoiGianBaoHanh },
                new SqlParameter("@AmThanh", SqlDbType.NVarChar, 100) { Value = (object?)dto.AmThanh ?? DBNull.Value },
                new SqlParameter("@ChupAnh", SqlDbType.NVarChar, 200) { Value = (object?)dto.ChupAnh ?? DBNull.Value },
                new SqlParameter("@Anh", SqlDbType.NVarChar, 250) { Value = (object?)dto.Anh ?? DBNull.Value },
                new SqlParameter("@GhiChu", SqlDbType.NVarChar) { Value = (object?)dto.GhiChu ?? DBNull.Value }
            };

            int affected = _adoNet!.ExecuteNonQueryText(sql, parameters);
            return affected > 0 ? BusResult<object?>.NoContent() : BusResult<object?>.NotFound();
        }

        var sp = await _db.SanPhams.FirstOrDefaultAsync(x => x.MaSp == maSp);
        if (sp == null) return BusResult<object?>.NotFound();

        sp.TenSp = dto.TenSp;
        sp.MaLoai = dto.MaLoai;
        sp.MaNhanHieu = dto.MaNhanHieu;
        sp.MaManHinh = dto.MaManHinh;
        sp.GiaNhap = dto.GiaNhap;
        sp.GiaBan = giaBan;
        sp.SoLuong = dto.SoLuong;
        sp.ThoiGianBaoHanh = dto.ThoiGianBaoHanh;
        sp.AmThanh = dto.AmThanh;
        sp.ChupAnh = dto.ChupAnh;
        sp.Anh = dto.Anh;
        sp.GhiChu = dto.GhiChu;

        await _db.SaveChangesAsync();
        return BusResult<object?>.NoContent();
    }

    public async Task<BusResult<object?>> XoaSanPhamAsync(string maSp)
    {
        var sp = await _db.SanPhams.FindAsync(maSp);
        if (sp == null) return BusResult<object?>.NotFound();

        var daPhatSinhGiaoDich = await _db.ChiTietHdbs.AnyAsync(x => x.MaSp == maSp)
                              || await _db.ChiTietHdns.AnyAsync(x => x.MaSp == maSp);
        if (daPhatSinhGiaoDich)
        {
            return BusResult<object?>.BadRequest("Không thể xóa sản phẩm đã phát sinh hóa đơn bán hoặc nhập!");
        }

        try
        {
            if (UseAdoNet)
            {
                const string sql = "DELETE FROM SanPham WHERE MaSP = @MaSP";
                int affected = _adoNet!.ExecuteNonQueryText(sql, new[]
                {
                    new SqlParameter("@MaSP", SqlDbType.NVarChar, 20) { Value = maSp }
                });
                return affected > 0 ? BusResult<object?>.NoContent() : BusResult<object?>.NotFound();
            }

            _db.SanPhams.Remove(sp);
            await _db.SaveChangesAsync();
            return BusResult<object?>.NoContent();
        }
        catch (DbUpdateException)
        {
            return BusResult<object?>.BadRequest("Không thể xóa sản phẩm đã phát sinh hóa đơn bán hoặc nhập!");
        }
        catch (SqlException)
        {
            return BusResult<object?>.BadRequest("Không thể xóa sản phẩm đã phát sinh hóa đơn bán hoặc nhập!");
        }
    }
}
