using System.ComponentModel.DataAnnotations;

namespace NexusRetail.WebMvc.Models
{
    public class AdminProductViewModel
    {
        public string? MaSp { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [Display(Name = "Tên sản phẩm")]
        public string TenSp { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn loại sản phẩm")]
        [Display(Name = "Loại sản phẩm")]
        public string MaLoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn nhãn hiệu")]
        [Display(Name = "Nhãn hiệu")]
        public string MaNhanHieu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn màn hình")]
        [Display(Name = "Màn hình")]
        public string MaManHinh { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập giá nhập")]
        [Range(1000, 1000000000, ErrorMessage = "Giá nhập phải từ 1.000đ trở lên")]
        [Display(Name = "Giá nhập (VNĐ)")]
        public decimal GiaNhap { get; set; }

        [Display(Name = "Giá bán (Tự động = Giá nhập × 1.1)")]
        public decimal GiaBan => Math.Round(GiaNhap * 1.1m, 0);

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Range(0, 10000, ErrorMessage = "Số lượng phải từ 0 trở lên")]
        [Display(Name = "Số lượng tồn kho")]
        public int SoLuong { get; set; }

        [Display(Name = "Thời gian bảo hành (tháng)")]
        [Range(1, 60, ErrorMessage = "Bảo hành từ 1 đến 60 tháng")]
        public int ThoiGianBaoHanh { get; set; } = 12;

        [Display(Name = "Âm thanh")]
        public string? AmThanh { get; set; } = "Stereo";

        [Display(Name = "Chụp ảnh / Camera")]
        public string? ChupAnh { get; set; } = "Chính 50MP";

        [Display(Name = "Ảnh hiện tại")]
        public string? Anh { get; set; } = "default.webp";

        [Display(Name = "Tải ảnh mới")]
        public IFormFile? FileAnh { get; set; }

        [Display(Name = "Ghi chú / Cấu hình tóm tắt")]
        public string? GhiChu { get; set; }
    }

    public class AdminOrderListItem
    {
        public string MaHdb { get; set; } = string.Empty;
        public DateTime NgayBan { get; set; }
        public decimal TongTien { get; set; }
        public string TenKhach { get; set; } = string.Empty;
        public string DienThoai { get; set; } = string.Empty;
        public string TenNv { get; set; } = string.Empty;
    }

    public class AdminOrderDetailViewModel
    {
        public string MaHdb { get; set; } = string.Empty;
        public DateTime NgayBan { get; set; }
        public decimal TongTien { get; set; }
        public string TenKhach { get; set; } = string.Empty;
        public string DienThoai { get; set; } = string.Empty;
        public string TenNv { get; set; } = string.Empty;
        public List<AdminOrderItem> ChiTiet { get; set; } = new();
    }

    public class AdminOrderItem
    {
        public string MaSp { get; set; } = string.Empty;
        public string TenSp { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal KhuyenMai { get; set; }
        public decimal ThanhTien { get; set; }
    }

    public class CategoryFilterDto
    {
        public List<CategoryOption> Loai { get; set; } = new();
        public List<BrandOption> NhanHieu { get; set; } = new();
        public List<ScreenOption> ManHinh { get; set; } = new();
    }

    public class CategoryOption
    {
        public string MaLoai { get; set; } = string.Empty;
        public string TenLoai { get; set; } = string.Empty;
    }

    public class BrandOption
    {
        public string MaNhanHieu { get; set; } = string.Empty;
        public string TenNhanHieu { get; set; } = string.Empty;
    }

    public class ScreenOption
    {
        public string MaManHinh { get; set; } = string.Empty;
        public string TenManHinh { get; set; } = string.Empty;
    }
}
