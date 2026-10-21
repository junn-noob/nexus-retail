using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusRetail.Shared.DTOs.Common;
using NexusRetail.Shared.DTOs.SanPham;
using NexusRetail.WebMvc.Models;
using NexusRetail.WebMvc.Services.Admin;
using NexusRetail.WebMvc.Services.Catalog;
using NexusRetail.WebMvc.Services.Orders;
using NexusRetail.WebMvc.Services.Reports;

namespace NexusRetail.WebMvc.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class AdminController : Controller
    {
        private readonly ICatalogApiClient _catalogClient;
        private readonly IAdminProductApiClient _adminProductClient;
        private readonly IOrderApiClient _orderClient;
        private readonly IReportApiClient _reportClient;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            ICatalogApiClient catalogClient,
            IAdminProductApiClient adminProductClient,
            IOrderApiClient orderClient,
            IReportApiClient reportClient,
            IWebHostEnvironment env,
            ILogger<AdminController> logger)
        {
            _catalogClient = catalogClient;
            _adminProductClient = adminProductClient;
            _orderClient = orderClient;
            _reportClient = reportClient;
            _env = env;
            _logger = logger;
        }

        // 1. DASHBOARD TỔNG QUAN
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var stats = await _reportClient.GetDashboardStatsAsync();

            ViewBag.TotalProducts = stats.TotalProducts;
            ViewBag.TotalOrders = stats.TotalOrders;
            ViewBag.TotalRevenue = stats.TotalRevenue;
            ViewBag.TotalCustomers = stats.TotalCustomers;

            return View();
        }

        // 2. QUẢN LÝ SẢN PHẨM (DANH SÁCH + PHÂN TRANG + TÌM KIẾM)
        [HttpGet]
        public async Task<IActionResult> SanPham(int page = 1, string? q = null)
        {
            page = Math.Max(1, page);
            ViewBag.CurrentPage = page;
            ViewBag.Query = q;

            var result = await _catalogClient.GetProductsAsync(page, 10, q);
            return View(result);
        }

        private async Task LoadCategoriesViewBagAsync()
        {
            var filter = await _catalogClient.GetCategoryFilterAsync();
            ViewBag.Loais = filter?.Loai ?? new List<CategoryOption>();
            ViewBag.NhanHieus = filter?.NhanHieu ?? new List<BrandOption>();
            ViewBag.ManHinhs = filter?.ManHinh ?? new List<ScreenOption>();
        }

        // 3. THÊM SẢN PHẨM (BR-03: GiaBan = GiaNhap * 1.1 + UPLOAD ẢNH)
        [HttpGet]
        public async Task<IActionResult> ThemSanPham()
        {
            await LoadCategoriesViewBagAsync();
            return View(new AdminProductViewModel
            {
                GiaNhap = 10000000m,
                SoLuong = 10,
                ThoiGianBaoHanh = 12
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemSanPham(AdminProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesViewBagAsync();
                return View(model);
            }

            string fileName = "iphone_16_plus_black_37a9bceca0.png";
            if (model.FileAnh != null && model.FileAnh.Length > 0)
            {
                var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var ext = Path.GetExtension(model.FileAnh.FileName).ToLower();

                if (!allowedExts.Contains(ext))
                {
                    ModelState.AddModelError("FileAnh", "Chỉ chấp nhận định dạng ảnh .jpg, .png, .webp!");
                    await LoadCategoriesViewBagAsync();
                    return View(model);
                }

                if (model.FileAnh.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("FileAnh", "Dung lượng ảnh không được vượt quá 5MB!");
                    await LoadCategoriesViewBagAsync();
                    return View(model);
                }

                fileName = $"sp_{DateTime.UtcNow.Ticks}{ext}";
                var uploadDir = Path.Combine(_env.WebRootPath, "images", "products");
                if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);
                var filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.FileAnh.CopyToAsync(stream);
                }
            }

            var dto = new ThemSanPhamDto
            {
                TenSp = model.TenSp,
                MaLoai = model.MaLoai,
                MaNhanHieu = model.MaNhanHieu,
                MaManHinh = model.MaManHinh,
                GiaNhap = model.GiaNhap,
                SoLuong = model.SoLuong,
                ThoiGianBaoHanh = model.ThoiGianBaoHanh,
                AmThanh = model.AmThanh,
                ChupAnh = model.ChupAnh,
                Anh = fileName,
                GhiChu = model.GhiChu
            };

            var (success, error) = await _adminProductClient.CreateProductAsync(dto);
            if (success)
            {
                TempData["SuccessMessage"] = $"Đã thêm mới sản phẩm '{model.TenSp}' thành công!";
                return RedirectToAction(nameof(SanPham));
            }

            ModelState.AddModelError("", $"Lỗi từ máy chủ: {error}");
            await LoadCategoriesViewBagAsync();
            return View(model);
        }

        // 4. SỬA SẢN PHẨM
        [HttpGet]
        public async Task<IActionResult> SuaSanPham(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var sp = await _catalogClient.GetProductByIdAsync(id);
            if (sp == null) return NotFound();

            await LoadCategoriesViewBagAsync();

            var vm = new AdminProductViewModel
            {
                MaSp = sp.MaSp,
                TenSp = sp.TenSp,
                MaLoai = sp.MaLoai ?? "",
                MaNhanHieu = sp.MaNhanHieu,
                MaManHinh = sp.MaManHinh ?? "",
                GiaNhap = sp.GiaNhap > 0 ? sp.GiaNhap : Math.Round(sp.GiaBan / 1.1m, 0),
                SoLuong = sp.SoLuong,
                ThoiGianBaoHanh = sp.ThoiGianBaoHanh,
                AmThanh = sp.AmThanh,
                ChupAnh = sp.ChupAnh,
                Anh = sp.Anh,
                GhiChu = sp.GhiChu
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaSanPham(string id, AdminProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesViewBagAsync();
                return View(model);
            }

            string fileName = model.Anh ?? "iphone_16_plus_black_37a9bceca0.png";
            if (model.FileAnh != null && model.FileAnh.Length > 0)
            {
                var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var ext = Path.GetExtension(model.FileAnh.FileName).ToLower();

                if (!allowedExts.Contains(ext))
                {
                    ModelState.AddModelError("FileAnh", "Chỉ chấp nhận định dạng ảnh .jpg, .png, .webp!");
                    await LoadCategoriesViewBagAsync();
                    return View(model);
                }

                fileName = $"sp_{DateTime.UtcNow.Ticks}{ext}";
                var uploadDir = Path.Combine(_env.WebRootPath, "images", "products");
                if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);
                var filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.FileAnh.CopyToAsync(stream);
                }
            }

            var dto = new SuaSanPhamDto
            {
                TenSp = model.TenSp,
                MaLoai = model.MaLoai,
                MaNhanHieu = model.MaNhanHieu,
                MaManHinh = model.MaManHinh,
                GiaNhap = model.GiaNhap,
                SoLuong = model.SoLuong,
                ThoiGianBaoHanh = model.ThoiGianBaoHanh,
                AmThanh = model.AmThanh,
                ChupAnh = model.ChupAnh,
                Anh = fileName,
                GhiChu = model.GhiChu
            };

            var (success, error) = await _adminProductClient.UpdateProductAsync(id, dto);
            if (success)
            {
                TempData["SuccessMessage"] = $"Cập nhật sản phẩm '{model.TenSp}' thành công!";
                return RedirectToAction(nameof(SanPham));
            }

            ModelState.AddModelError("", $"Lỗi từ máy chủ: {error}");
            await LoadCategoriesViewBagAsync();
            return View(model);
        }

        // 5. XÓA SẢN PHẨM (CHỈ ADMIN)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaSanPham(string id)
        {
            var (success, error) = await _adminProductClient.DeleteProductAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = $"Đã xóa sản phẩm {id} thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = error ?? "Không thể xóa sản phẩm!";
            }

            return RedirectToAction(nameof(SanPham));
        }

        // 6. QUẢN LÝ ĐƠN HÀNG
        [HttpGet]
        public async Task<IActionResult> DonHang(int page = 1)
        {
            page = Math.Max(1, page);
            ViewBag.CurrentPage = page;

            var (items, totalPages, totalItems) = await _orderClient.GetOrdersListAsync(page, 15);
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> ChiTietDonHang(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var vm = await _orderClient.GetOrderDetailAsync(id);
            if (vm == null) return NotFound();

            return View(vm);
        }

        // 7. BÁO CÁO THỐNG KÊ (CHỈ ADMIN)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> BaoCao(string? loai = "sp-ban-chay", int? thang = null, int? nam = null, int? quy = null)
        {
            loai ??= "sp-ban-chay";
            nam ??= 2026;
            thang ??= 9; // Mặc định tháng 9/2026
            quy ??= 3;   // Mặc định Quý 3/2026

            ViewBag.Loai = loai;
            ViewBag.Thang = thang;
            ViewBag.Nam = nam;
            ViewBag.Quy = quy;

            var data = await _reportClient.GetReportDataAsync(loai, thang, nam, quy);
            return View(data);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> XuatExcel(string loai, int? thang = null, int? nam = null, int? quy = null)
        {
            nam ??= 2026;
            thang ??= 9;
            quy ??= 3;

            var fileBytes = await _reportClient.DownloadExcelReportAsync(loai, thang, nam, quy);
            if (fileBytes != null && fileBytes.Length > 0)
            {
                var fileName = $"BaoCao_{loai}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }

            TempData["ErrorMessage"] = "Không thể xuất file Excel cho báo cáo này!";
            return RedirectToAction(nameof(BaoCao), new { loai, thang, nam, quy });
        }
    }
}
