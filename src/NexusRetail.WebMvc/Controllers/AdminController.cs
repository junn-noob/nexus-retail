using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusRetail.Shared.DTOs.Common;
using NexusRetail.Shared.DTOs.SanPham;
using NexusRetail.WebMvc.Models;
using System.Text.Json;

namespace NexusRetail.WebMvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _factory;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IHttpClientFactory factory, IWebHostEnvironment env, ILogger<AdminController> logger)
        {
            _factory = factory;
            _env = env;
            _logger = logger;
        }

        private HttpClient CoreClient => _factory.CreateClient("CoreApi");

        // 1. DASHBOARD TỔNG QUAN
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var spRes = await CoreClient.GetFromJsonAsync<PagedResultDto<SanPhamDto>>("api/san-pham?page=1&pageSize=1");
                ViewBag.TotalProducts = spRes?.TotalItems ?? 0;

                var dhRes = await CoreClient.GetFromJsonAsync<JsonElement>("api/ban-hang/danh-sach?page=1&pageSize=1");
                ViewBag.TotalOrders = dhRes.TryGetProperty("totalItems", out var ti) ? ti.GetInt32() : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi nạp dữ liệu Dashboard Admin");
                ViewBag.TotalProducts = 0;
                ViewBag.TotalOrders = 0;
            }

            return View();
        }

        // 2. QUẢN LÝ SẢN PHẨM (DANH SÁCH + PHÂN TRANG + TÌM KIẾM)
        [HttpGet]
        public async Task<IActionResult> SanPham(int page = 1, string? q = null)
        {
            page = Math.Max(1, page);
            ViewBag.CurrentPage = page;
            ViewBag.Query = q;

            var url = $"api/san-pham?page={page}&pageSize=10";
            if (!string.IsNullOrWhiteSpace(q))
                url += $"&q={Uri.EscapeDataString(q)}";

            try
            {
                var result = await CoreClient.GetFromJsonAsync<PagedResultDto<SanPhamDto>>(url);
                return View(result ?? new PagedResultDto<SanPhamDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy danh sách sản phẩm Admin");
                return View(new PagedResultDto<SanPhamDto>());
            }
        }

        private async Task LoadCategoriesViewBagAsync()
        {
            try
            {
                var filterRes = await CoreClient.GetFromJsonAsync<CategoryFilterDto>("api/san-pham/bo-loc-danh-muc");
                ViewBag.Loais = filterRes?.Loai ?? new List<CategoryOption>();
                ViewBag.NhanHieus = filterRes?.NhanHieu ?? new List<BrandOption>();
                ViewBag.ManHinhs = filterRes?.ManHinh ?? new List<ScreenOption>();
            }
            catch
            {
                ViewBag.Loais = new List<CategoryOption>();
                ViewBag.NhanHieus = new List<BrandOption>();
                ViewBag.ManHinhs = new List<ScreenOption>();
            }
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

            // Xử lý upload ảnh (Validate định dạng & dung lượng < 5MB)
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

            // DTO gửi sang Core API
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

            var response = await CoreClient.PostAsJsonAsync("api/san-pham", dto);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = $"Đã thêm mới sản phẩm '{model.TenSp}' thành công!";
                return RedirectToAction(nameof(SanPham));
            }

            var err = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Lỗi từ máy chủ: {err}");
            await LoadCategoriesViewBagAsync();
            return View(model);
        }

        // 4. SỬA SẢN PHẨM
        [HttpGet]
        public async Task<IActionResult> SuaSanPham(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var sp = await CoreClient.GetFromJsonAsync<SanPhamDto>($"api/san-pham/{id}");
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

            var response = await CoreClient.PutAsJsonAsync($"api/san-pham/{id}", dto);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = $"Cập nhật sản phẩm '{model.TenSp}' thành công!";
                return RedirectToAction(nameof(SanPham));
            }

            var err = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Lỗi từ máy chủ: {err}");
            await LoadCategoriesViewBagAsync();
            return View(model);
        }

        // 5. XÓA SẢN PHẨM
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaSanPham(string id)
        {
            var response = await CoreClient.DeleteAsync($"api/san-pham/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = $"Đã xóa sản phẩm {id} thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa sản phẩm đã phát sinh hóa đơn bán hoặc hóa đơn nhập!";
            }

            return RedirectToAction(nameof(SanPham));
        }

        // 6. QUẢN LÝ ĐƠN HÀNG
        [HttpGet]
        public async Task<IActionResult> DonHang(int page = 1)
        {
            page = Math.Max(1, page);
            ViewBag.CurrentPage = page;

            try
            {
                var response = await CoreClient.GetFromJsonAsync<JsonElement>($"api/ban-hang/danh-sach?page={page}&pageSize=15");
                var items = new List<AdminOrderListItem>();

                if (response.TryGetProperty("items", out var itemsElem))
                {
                    foreach (var it in itemsElem.EnumerateArray())
                    {
                        items.Add(new AdminOrderListItem
                        {
                            MaHdb = it.GetProperty("maHdb").GetString() ?? "",
                            NgayBan = it.GetProperty("ngayBan").GetDateTime(),
                            TongTien = it.GetProperty("tongTien").GetDecimal(),
                            TenKhach = it.GetProperty("tenKhach").GetString() ?? "",
                            DienThoai = it.GetProperty("dienThoai").GetString() ?? "",
                            TenNv = it.GetProperty("tenNv").GetString() ?? ""
                        });
                    }
                }

                ViewBag.TotalPages = response.TryGetProperty("totalPages", out var tp) ? tp.GetInt32() : 1;
                ViewBag.TotalItems = response.TryGetProperty("totalItems", out var ti) ? ti.GetInt32() : 0;

                return View(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy danh sách đơn hàng Admin");
                return View(new List<AdminOrderListItem>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> ChiTietDonHang(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            try
            {
                var response = await CoreClient.GetFromJsonAsync<JsonElement>($"api/ban-hang/chi-tiet/{id}");
                var vm = new AdminOrderDetailViewModel
                {
                    MaHdb = response.GetProperty("maHdb").GetString() ?? "",
                    NgayBan = response.GetProperty("ngayBan").GetDateTime(),
                    TongTien = response.GetProperty("tongTien").GetDecimal(),
                    TenKhach = response.GetProperty("tenKhach").GetString() ?? "",
                    DienThoai = response.GetProperty("dienThoai").GetString() ?? "",
                    TenNv = response.GetProperty("tenNv").GetString() ?? ""
                };

                if (response.TryGetProperty("chiTiet", out var ctList))
                {
                    foreach (var ct in ctList.EnumerateArray())
                    {
                        vm.ChiTiet.Add(new AdminOrderItem
                        {
                            MaSp = ct.GetProperty("maSp").GetString() ?? "",
                            TenSp = ct.GetProperty("tenSp").GetString() ?? "",
                            SoLuong = ct.GetProperty("soLuong").GetInt32(),
                            DonGia = ct.GetProperty("donGia").GetDecimal(),
                            KhuyenMai = ct.GetProperty("khuyenMai").GetDecimal(),
                            ThanhTien = ct.GetProperty("thanhTien").GetDecimal()
                        });
                    }
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi nạp chi tiết đơn hàng");
                return NotFound();
            }
        }

        // 7. BÁO CÁO THỐNG KÊ (4 BÁO CÁO BR-06, BR-07, BR-08, BR-09 VÀ XUẤT EXCEL)
        [HttpGet]
        public async Task<IActionResult> BaoCao(string? loai = "sp-ban-chay", int? thang = null, int? nam = null, int? quy = null)
        {
            loai ??= "sp-ban-chay";
            nam ??= 2026;
            thang ??= 9; // Mặc định tháng 9/2026 có đầy đủ hóa đơn bán hàng mẫu
            quy ??= 3;   // Mặc định Quý 3/2026 có các hóa đơn nhập hàng lớn

            ViewBag.Loai = loai;
            ViewBag.Thang = thang;
            ViewBag.Nam = nam;
            ViewBag.Quy = quy;

            string apiUrl = loai switch
            {
                "sp-ban-chay" => $"api/bao-cao/sp-ban-chay?month={thang}&year={nam}",
                "nhap-lon-nhat-quy" => $"api/bao-cao/nhap-lon-nhat-quy?quy={quy}&year={nam}",
                "doanh-so-nam" => $"api/bao-cao/doanh-so-nam?year={nam}",
                "nv-ban-it-nhat" => $"api/bao-cao/nv-ban-it-nhat?month={thang}&year={nam}",
                _ => $"api/bao-cao/sp-ban-chay?month={thang}&year={nam}"
            };

            try
            {
                var response = await CoreClient.GetFromJsonAsync<JsonElement>(apiUrl);
                return View(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy dữ liệu báo cáo: {Url}", apiUrl);
                return View(null);
            }
        }

        [HttpGet]
        public async Task<IActionResult> XuatExcel(string loai, int? thang = null, int? nam = null, int? quy = null)
        {
            nam ??= 2026;
            thang ??= 9;
            quy ??= 3;

            string excelUrl = loai switch
            {
                "sp-ban-chay" => $"api/bao-cao/sp-ban-chay/excel?month={thang}&year={nam}",
                "nhap-lon-nhat-quy" => $"api/bao-cao/nhap-lon-nhat-quy/excel?quy={quy}&year={nam}",
                "doanh-so-nam" => $"api/bao-cao/doanh-so-nam/excel?year={nam}",
                "nv-ban-it-nhat" => $"api/bao-cao/nv-ban-it-nhat/excel?month={thang}&year={nam}",
                _ => $"api/bao-cao/sp-ban-chay/excel?month={thang}&year={nam}"
            };

            try
            {
                var response = await CoreClient.GetAsync(excelUrl);
                if (response.IsSuccessStatusCode)
                {
                    var fileBytes = await response.Content.ReadAsByteArrayAsync();
                    var fileName = $"BaoCao_{loai}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }

                TempData["ErrorMessage"] = "Không thể xuất file Excel cho báo cáo này!";
                return RedirectToAction(nameof(BaoCao), new { loai, thang, nam, quy });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xuất Excel");
                TempData["ErrorMessage"] = "Lỗi khi gọi API xuất file Excel!";
                return RedirectToAction(nameof(BaoCao), new { loai, thang, nam, quy });
            }
        }
    }
}
