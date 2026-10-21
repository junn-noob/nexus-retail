using System.ComponentModel.DataAnnotations;

namespace NexusRetail.WebMvc.Models
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên của bạn!")]
        [StringLength(50, ErrorMessage = "Họ tên tối đa 50 ký tự")]
        public string TenKhach { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại nhận hàng!")]
        [RegularExpression(@"^(03|05|07|08|09)\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ (gồm 10 số, bắt đầu bằng 03, 05, 07, 08, 09)!")]
        public string DienThoai { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng!")]
        [StringLength(200, ErrorMessage = "Địa chỉ tối đa 200 ký tự")]
        public string DiaChi { get; set; } = "";

        public string? GhiChu { get; set; }

        public List<CartItem> DanhSachMon { get; set; } = new();

        public decimal TongTien => DanhSachMon.Sum(x => x.ThanhTien);
    }
}
