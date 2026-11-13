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
        public async Task<IActionResult> AddToCart(string maSp, int quantity = 1, string? maMau = null)
        {
            if (string.IsNullOrWhiteSpace(maSp))
                return Json(new { success = false, message = "Mã sản phẩm không hợp lệ!" });

            int addQty = Math.Max(1, quantity);
            var sp = await _catalogClient.GetProductByIdAsync(maSp);
            if (sp == null)
                return Json(new { success = false, message = "Không tìm thấy thông tin sản phẩm trên hệ thống!" });

            // Tìm biến thể màu sắc tương ứng
            var variant = sp.DanhSachMauSac?.FirstOrDefault(x =>
                !string.IsNullOrWhiteSpace(maMau) && x.MaMau.Equals(maMau.Trim(), StringComparison.OrdinalIgnoreCase))
                ?? (string.IsNullOrWhiteSpace(maMau) ? sp.DanhSachMauSac?.FirstOrDefault() : null);

            string effectiveMaMau = variant?.MaMau ?? (string.IsNullOrWhiteSpace(maMau) ? "DEFAULT" : maMau.Trim());
            string? effectiveTenMau = variant?.TenMau;
            string? effectiveMaHex = variant?.MaHex;
            decimal effectiveGiaBan = variant?.GiaBan ?? sp.GiaBan;
            int effectiveSoLuong = variant?.SoLuong ?? sp.SoLuong;
            string? effectiveAnh = !string.IsNullOrWhiteSpace(variant?.HinhAnh) ? variant.HinhAnh : sp.Anh;

            if (effectiveSoLuong <= 0)
            {
                string outOfStockName = !string.IsNullOrWhiteSpace(effectiveTenMau)
                    ? $"Sản phẩm '{sp.TenSp}' màu {effectiveTenMau}"
                    : $"Sản phẩm '{sp.TenSp}'";
                return Json(new { success = false, message = $"{outOfStockName} hiện đã tạm hết hàng!" });
            }

            var cart = GetCartFromSession();
            string targetKey = $"{sp.MaSp}_{effectiveMaMau}";
            var existingItem = cart.FirstOrDefault(x => x.CartKey == targetKey || (x.MaSp == sp.MaSp && (x.MaMau ?? "DEFAULT") == effectiveMaMau));
            int nextQty = (existingItem?.SoLuong ?? 0) + addQty;

            if (nextQty > effectiveSoLuong)
            {
                string variantLabel = !string.IsNullOrWhiteSpace(effectiveTenMau) ? $" màu '{effectiveTenMau}'" : "";
                return Json(new
                {
                    success = false,
                    message = $"Kho chỉ còn {effectiveSoLuong} chiếc '{sp.TenSp}'{variantLabel} (trong giỏ đã có {existingItem?.SoLuong ?? 0} chiếc)!"
                });
            }

            if (existingItem != null)
            {
                existingItem.SoLuong = nextQty;
                existingItem.GiaBan = effectiveGiaBan;
                existingItem.TenSp = sp.TenSp;
                existingItem.MaMau = effectiveMaMau;
                existingItem.TenMau = effectiveTenMau;
                existingItem.MaHex = effectiveMaHex;
                existingItem.Anh = effectiveAnh;
            }
            else
            {
                cart.Add(new CartItem
                {
                    MaSp = sp.MaSp,
                    TenSp = sp.TenSp,
                    MaMau = effectiveMaMau,
                    TenMau = effectiveTenMau,
                    MaHex = effectiveMaHex,
                    GiaBan = effectiveGiaBan,
                    SoLuong = addQty,
                    Anh = effectiveAnh
                });
            }

            SaveCartToSession(cart);
            string successMsg = !string.IsNullOrWhiteSpace(effectiveTenMau)
                ? $"Đã thêm '{sp.TenSp}' ({effectiveTenMau}) vào giỏ hàng!"
                : $"Đã thêm '{sp.TenSp}' vào giỏ hàng!";

            return Json(new
            {
                success = true,
                message = successMsg,
                totalItems = cart.Sum(x => x.SoLuong)
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(string? cartKey, string? maSp = null, int delta = 0, int? quantity = null)
        {
            var cart = GetCartFromSession();
            var item = (!string.IsNullOrWhiteSpace(cartKey) ? cart.FirstOrDefault(x => x.CartKey == cartKey) : null)
                    ?? (!string.IsNullOrWhiteSpace(maSp) ? cart.FirstOrDefault(x => x.MaSp == maSp) : null);

            if (item != null)
            {
                int targetQty = quantity ?? (item.SoLuong + delta);

                if (targetQty > item.SoLuong)
                {
                    var sp = await _catalogClient.GetProductByIdAsync(item.MaSp);
                    if (sp != null)
                    {
                        var variant = sp.DanhSachMauSac?.FirstOrDefault(x =>
                            !string.IsNullOrWhiteSpace(item.MaMau) && x.MaMau.Equals(item.MaMau, StringComparison.OrdinalIgnoreCase));
                        int maxAvailable = variant?.SoLuong ?? sp.SoLuong;

                        if (targetQty > maxAvailable)
                        {
                            string variantLabel = !string.IsNullOrWhiteSpace(item.TenMau) ? $" màu '{item.TenMau}'" : "";
                            string limitMsg = $"Sản phẩm '{sp.TenSp}'{variantLabel} chỉ còn tối đa {maxAvailable} chiếc trong kho!";
                            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                            {
                                return Json(new { success = false, message = limitMsg, quantity = item.SoLuong });
                            }
                            TempData["CartError"] = limitMsg;
                            return RedirectToAction(nameof(Index));
                        }
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
        public IActionResult RemoveItem(string? cartKey, string? maSp = null)
        {
            var cart = GetCartFromSession();
            var item = (!string.IsNullOrWhiteSpace(cartKey) ? cart.FirstOrDefault(x => x.CartKey == cartKey) : null)
                    ?? (!string.IsNullOrWhiteSpace(maSp) ? cart.FirstOrDefault(x => x.MaSp == maSp) : null);
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
                    MaMau = c.MaMau,
                    TenMau = c.TenMau,
                    MaHex = c.MaHex,
                    DonGia = c.GiaBan,
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
