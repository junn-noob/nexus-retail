using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusRetail.Shared.DTOs.BanHang;
using NexusRetail.WebMvc.Extensions;
using NexusRetail.WebMvc.Models;
using NexusRetail.WebMvc.Services.Catalog;
using NexusRetail.WebMvc.Services.Orders;

namespace NexusRetail.WebMvc.Controllers
{
    public class CartController : Controller
    {
        private const string CART_KEY = "NEXUS_CART_SESSION";
        private readonly ICatalogApiClient _catalogClient;
        private readonly IOrderApiClient _orderClient;
        private readonly ILogger<CartController> _logger;

        public CartController(ICatalogApiClient catalogClient, IOrderApiClient orderClient, ILogger<CartController> logger)
        {
            _catalogClient = catalogClient;
            _orderClient = orderClient;
            _logger = logger;
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

            int addQty = Math.Max(1, quantity);
            var sp = await _catalogClient.GetProductByIdAsync(maSp);
            if (sp == null)
                return Json(new { success = false, message = "Không tìm thấy thông tin sản phẩm trên hệ thống!" });

            if (sp.SoLuong <= 0)
                return Json(new { success = false, message = $"Sản phẩm '{sp.TenSp}' hiện đã tạm hết hàng!" });

            var cart = GetCartFromSession();
            var existingItem = cart.FirstOrDefault(x => x.MaSp == maSp);
            int nextQty = (existingItem?.SoLuong ?? 0) + addQty;

            if (nextQty > sp.SoLuong)
            {
                return Json(new
                {
                    success = false,
                    message = $"Kho chỉ còn {sp.SoLuong} chiếc '{sp.TenSp}' (trong giỏ đã có {existingItem?.SoLuong ?? 0} chiếc)!"
                });
            }

            if (existingItem != null)
            {
                existingItem.SoLuong = nextQty;
                existingItem.GiaBan = sp.GiaBan;
                existingItem.TenSp = sp.TenSp;
                existingItem.Anh = sp.Anh;
            }
            else
            {
                cart.Add(new CartItem
                {
                    MaSp = sp.MaSp,
                    TenSp = sp.TenSp,
                    GiaBan = sp.GiaBan,
                    SoLuong = addQty,
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
        public async Task<IActionResult> UpdateQuantity(string maSp, int delta = 0, int? quantity = null)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(x => x.MaSp == maSp);

            if (item != null)
            {
                int targetQty = quantity ?? (item.SoLuong + delta);

                if (targetQty > item.SoLuong)
                {
                    var sp = await _catalogClient.GetProductByIdAsync(maSp);
                    if (sp != null && targetQty > sp.SoLuong)
                    {
                        string limitMsg = $"Sản phẩm '{sp.TenSp}' chỉ còn tối đa {sp.SoLuong} chiếc trong kho!";
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        {
                            return Json(new { success = false, message = limitMsg, quantity = item.SoLuong });
                        }
                        TempData["CartError"] = limitMsg;
                        return RedirectToAction(nameof(Index));
                    }
                }

                item.SoLuong = targetQty;
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

            if (User.Identity?.IsAuthenticated == true)
            {
                model.TenKhach = User.Identity.Name ?? "";
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

            var dto = new HoaDonBanDto
            {
                MaNv = null,
                TenKhachVangLai = model.TenKhach,
                DienThoai = model.DienThoai,
                DiaChiGiaoHang = model.DiaChi,
                GhiChu = model.GhiChu,
                PhuongThucThanhToan = model.PhuongThucThanhToan,
                DanhSachMon = cart.Select(c => new ChiTietHdbDto
                {
                    MaSp = c.MaSp,
                    TenSp = c.TenSp,
                    SoLuong = c.SoLuong,
                    KhuyenMai = 0
                }).ToList()
            };

            var (success, message, maHdb, tongTien) = await _orderClient.CreateOrderAsync(dto);
            if (success)
            {
                HttpContext.Session.Remove(CART_KEY);
                _logger.LogInformation("Khách {TenKhach} ({Phone}) đặt hàng thành công: {MaHdb} ({Method})",
                    model.TenKhach, model.DienThoai, maHdb, model.PhuongThucThanhToan);

                return RedirectToAction(nameof(OrderSuccess), new
                {
                    maHdb = maHdb ?? string.Empty,
                    tongTien = tongTien > 0 ? tongTien : model.TongTien,
                    tenKhach = model.TenKhach,
                    dienThoai = model.DienThoai,
                    phuongThuc = model.PhuongThucThanhToan,
                    diaChi = model.DiaChi
                });
            }

            ModelState.AddModelError("", message);
            return View("Checkout", model);
        }

        [HttpGet]
        public async Task<IActionResult> OrderSuccess(
            string maHdb,
            decimal tongTien,
            string tenKhach,
            string? dienThoai = null,
            string? phuongThuc = "COD",
            string? diaChi = null)
        {
            ViewBag.MaHdb = maHdb;
            ViewBag.TongTien = tongTien;
            ViewBag.TenKhach = tenKhach;
            ViewBag.DienThoai = dienThoai;
            ViewBag.PhuongThuc = phuongThuc ?? "COD";
            ViewBag.DiaChi = diaChi ?? "";

            if (string.Equals(phuongThuc, "VietQR", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(maHdb))
            {
                var payState = await _orderClient.GetBankPaymentStatusAsync(maHdb, tongTien > 0 ? tongTien : null);
                ViewBag.InitialPaid = payState.IsPaid;
            }
            else
            {
                ViewBag.InitialPaid = string.Equals(phuongThuc, "Card", StringComparison.OrdinalIgnoreCase);
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CheckPaymentStatus(string maHdb, decimal? expectedAmount = null)
        {
            var status = await _orderClient.GetBankPaymentStatusAsync(maHdb, expectedAmount);
            return Json(new
            {
                isPaid = status.IsPaid,
                paidAmount = status.PaidAmount,
                expectedAmount = status.ExpectedAmount,
                message = status.Message,
                transactionCode = status.TransactionCode
            });
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SimulateVietQrPayment(string maHdb, decimal amount)
        {
            var (success, message) = await _orderClient.SimulateBankPaymentAsync(maHdb, amount);
            return Json(new { success, message });
        }
    }
}
