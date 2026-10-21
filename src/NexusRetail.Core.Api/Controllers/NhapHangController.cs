using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusRetail.Core.Api.Data;
using NexusRetail.Shared.DTOs.NhapHang;
using NexusRetail.Shared.Entities;

namespace NexusRetail.Core.Api.Controllers
{
    [Route("api/nhap-hang")]
    [ApiController]
    public class NhapHangController : ControllerBase
    {
        private readonly RetailDbContext _db;

        public NhapHangController(RetailDbContext db)
        {
            _db = db;
        }

        private async Task<string> sinhMaHdn()
        {
            var maCuoi = await _db.HoaDonNhaps
                .Where(x => x.MaHdn.StartsWith("HDN"))
                .OrderByDescending(x => x.MaHdn)
                .Select(x => x.MaHdn)
                .FirstOrDefaultAsync();

            if (maCuoi == null) return "HDN01";

            if (int.TryParse(maCuoi.Substring(3), out int soHienTai))
                return $"HDN{(soHienTai + 1):D2}";

            return $"HDN{Guid.NewGuid().ToString()[..6].ToUpper()}";
        }

        /// <summary>
        /// Lấy danh sách toàn bộ hóa đơn nhập hàng
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> layDanhSachHoaDonNhap()
        {
            var list = await _db.HoaDonNhaps
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

            return Ok(list);
        }

        /// <summary>
        /// Danh sách nhà cung cấp cho ComboBox
        /// </summary>
        [HttpGet("nha-cung-cap")]
        public async Task<IActionResult> layDanhSachNhaCungCap()
        {
            var list = await _db.NhaCungCaps
                .AsNoTracking()
                .Select(x => new { x.MaNcc, x.TenNcc, x.DiaChi, x.DienThoai })
                .ToListAsync();
            return Ok(list);
        }

        /// <summary>
        /// Danh sách nhân viên cho ComboBox
        /// </summary>
        [HttpGet("nhan-vien")]
        public async Task<IActionResult> layDanhSachNhanVien()
        {
            var list = await _db.NhanViens
                .AsNoTracking()
                .Select(x => new { x.MaNv, x.TenNv, x.DienThoai })
                .ToListAsync();
            return Ok(list);
        }

        /// <summary>
        /// Xem chi tiết một hóa đơn nhập hàng
        /// </summary>
        [HttpGet("{maHdn}")]
        public async Task<IActionResult> layChiTietHoaDonNhap(string maHdn)
        {
            var hdn = await _db.HoaDonNhaps
                .AsNoTracking()
                .Include(x => x.MaNvNavigation)
                .Include(x => x.MaNccNavigation)
                .Include(x => x.ChiTietHdns)
                    .ThenInclude(ct => ct.MaSpNavigation)
                .FirstOrDefaultAsync(x => x.MaHdn == maHdn);

            if (hdn == null) return NotFound(new { message = $"Không tìm thấy hóa đơn nhập mã {maHdn}" });

            var result = new HoaDonNhapResponseDto
            {
                MaHdn = hdn.MaHdn,
                MaNv = hdn.MaNv,
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

            return Ok(result);
        }

        /// <summary>
        /// Tạo hóa đơn nhập hàng - Thực thi BR-01 (Tăng tồn), BR-02 (Cập nhật giá vốn), BR-03 (Tính giá bán = giá vốn * 1.1)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> nhapHang([FromBody] HoaDonNhapDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var nv = await _db.NhanViens.FindAsync(dto.MaNv);
            if (nv == null)
                return BadRequest(new { message = $"Không tìm thấy nhân viên mã '{dto.MaNv}'!" });

            var ncc = await _db.NhaCungCaps.FindAsync(dto.MaNcc);
            if (ncc == null)
                return BadRequest(new { message = $"Không tìm thấy nhà cung cấp mã '{dto.MaNcc}'!" });

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var maHdn = await sinhMaHdn();
                decimal tongTienHoaDon = 0m;
                var chiTietList = new List<ChiTietHdnDto>();

                foreach (var item in dto.DanhSachMon)
                {
                    var sp = await _db.SanPhams.AsNoTracking().FirstOrDefaultAsync(x => x.MaSp == item.MaSp);
                    if (sp == null)
                    {
                        await tx.RollbackAsync();
                        return BadRequest(new { message = $"Không tìm thấy sản phẩm mã '{item.MaSp}' để nhập hàng!" });
                    }

                    // BR-01, BR-02, BR-03: Được thực thi tự động qua trigger trg_ChiTietHDN_Change (Tăng tồn kho, cập nhật giá nhập và tính giá bán = 1.1 * giá nhập)
                    // Tính thành tiền có chiết khấu/khuyến mãi nếu có
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

                return CreatedAtAction(nameof(layChiTietHoaDonNhap), new { maHdn = hdn.MaHdn }, response);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, new { message = "Lỗi khi xử lý nhập hàng: " + ex.Message });
            }
        }
    }
}
