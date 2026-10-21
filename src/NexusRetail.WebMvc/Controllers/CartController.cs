using Microsoft.AspNetCore.Mvc;
using NexusRetail.Shared.DTOs.BanHang;
using NexusRetail.Shared.DTOs.SanPham;
using NexusRetail.WebMvc.Extensions;
using NexusRetail.WebMvc.Models;

namespace NexusRetail.WebMvc.Controllers
{
    public class CartController : Controller
    {
        private const string CART_KEY = "NEXUS_CART_SESSION";
        private readonly IHttpClientFactory _factory;

        public CartController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        private List<CartItem> GetCartFromSession()
        {
            return HttpContext.Session.GetObject<List<CartItem>>(CART_KEY) ?? new List<CartItem>();
        }

        private void SaveCartToSession(List<CartItem> cart)
        {
            HttpContext.Session.SetObject(CART_KEY, cart);
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cart = GetCartFromSession();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(string maSp, int quantity = 1)
        {
            if (string.IsNullOrWhiteSpace(maSp))
                return Json(new { success = false, message = "Mã sản phẩm không hợp lệ!" });

            var cart = GetCartFromSession();
            var existingItem = cart.FirstOrDefault(x => x.MaSp == maSp);

            if (existingItem != null)
            {
                existingItem.SoLuong += Math.Max(1, quantity);
            }
            else
            {
                var client = _factory.CreateClient("CoreApi");
                var response = await client.GetAsync($"api/san-pham/{maSp}");
                if (!response.IsSuccessStatusCode)
                    return Json(new { success = false, message = "Không tìm thấy thông tin sản phẩm trên hệ thống!" });

                var sp = await response.Content.ReadFromJsonAsync<SanPhamDto>();
                if (sp == null)
                    return Json(new { success = false, message = "Dữ liệu sản phẩm rỗng!" });

                cart.Add(new CartItem
                {
                    MaSp = sp.MaSp,
                    TenSp = sp.TenSp,
                    GiaBan = sp.GiaBan,
                    SoLuong = Math.Max(1, quantity),
                    Anh = sp.Anh
                });
            }

            SaveCartToSession(cart);
            return Json(new
            {
                success = true,
                message = "Đã thêm sản phẩm vào giỏ hàng thành công!",
                totalItems = cart.Sum(x => x.SoLuong)
            });
        }

        [HttpPost]
        public IActionResult UpdateQuantity(string maSp, int delta)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(x => x.MaSp == maSp);

            if (item != null)
            {
                item.SoLuong += delta;
                if (item.SoLuong <= 0)
                {
                    cart.Remove(item);
                }
            }

            SaveCartToSession(cart);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = true,
                    quantity = item?.SoLuong ?? 0,
                    itemTotal = item?.ThanhTien ?? 0,
                    cartTotal = cart.Sum(x => x.ThanhTien),
                    totalItems = cart.Sum(x => x.SoLuong)
                });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult RemoveItem(string maSp)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(x => x.MaSp == maSp);
            if (item != null)
            {
                cart.Remove(item);
                SaveCartToSession(cart);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove(CART_KEY);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult GetCartCount()
        {
            var cart = GetCartFromSession();
            return Json(new { totalItems = cart.Sum(x => x.SoLuong) });
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = GetCartFromSession();
            if (cart.Count == 0)
                return RedirectToAction(nameof(Index));

            var model = new CheckoutViewModel
            {
                DanhSachMon = cart
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessCheckout(CheckoutViewModel model)
        {
            var cart = GetCartFromSession();
            if (cart.Count == 0)
            {
                ModelState.AddModelError("", "Giỏ hàng của bạn đang trống!");
                return View("Checkout", model);
            }

            model.DanhSachMon = cart;

            if (!ModelState.IsValid)
            {
                return View("Checkout", model);
            }

            // Chuẩn bị DTO thanh toán gửi sang Core API
            var dto = new HoaDonBanDto
            {
                MaNv = "NV01", // Mặc định nhân viên quản lý đơn Online
                TenKhachVangLai = model.TenKhach,
                DienThoai = model.DienThoai,
                DanhSachMon = cart.Select(c => new ChiTietHdbDto
                {
                    MaSp = c.MaSp,
                    TenSp = c.TenSp,
                    SoLuong = c.SoLuong,
                    KhuyenMai = 0
                }).ToList()
            };

            var client = _factory.CreateClient("CoreApi");
            var response = await client.PostAsJsonAsync("api/ban-hang/thanh-toan", dto);

            if (response.IsSuccessStatusCode)
            {
                var hdb = await response.Content.ReadFromJsonAsync<HoaDonBanResponseDto>();
                // Xóa giỏ hàng sau khi đặt thành công
                HttpContext.Session.Remove(CART_KEY);

                return RedirectToAction(nameof(OrderSuccess), new
                {
                    maHdb = hdb?.MaHdb ?? "HDB01",
                    tongTien = hdb?.TongTien ?? model.TongTien,
                    tenKhach = model.TenKhach
                });
            }
            else
            {
                var errorResult = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Lỗi khi đặt hàng: {errorResult}");
                return View("Checkout", model);
            }
        }

        [HttpGet]
        public IActionResult OrderSuccess(string maHdb, decimal tongTien, string tenKhach)
        {
            ViewBag.MaHdb = maHdb;
            ViewBag.TongTien = tongTien;
            ViewBag.TenKhach = tenKhach;
            return View();
        }
    }
}
