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

        private decimal? _giaBan;
        [Display(Name = "Giá bán niêm yết (Tối thiểu = Giá nhập × 1.10)")]
        public decimal GiaBan
        {
            get => (_giaBan.HasValue && _giaBan.Value >= Math.Round(GiaNhap * 1.1m, 0)) ? _giaBan.Value : Math.Round(GiaNhap * 1.1m, 0);
            set => _giaBan = value;
        }

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

    public class AdminInboundInvoiceItem
    {
        public string MaHdn { get; set; } = string.Empty;
        public string MaNv { get; set; } = string.Empty;
        public string TenNv { get; set; } = string.Empty;
        public string MaNcc { get; set; } = string.Empty;
        public string TenNcc { get; set; } = string.Empty;
        public DateTime NgayNhap { get; set; }
        public decimal TongTien { get; set; }
        public int SoMatHang { get; set; }
    }

    public class AdminSupplierOption
    {
        public string MaNcc { get; set; } = string.Empty;
        public string TenNcc { get; set; } = string.Empty;
        public string? DiaChi { get; set; }
        public string? DienThoai { get; set; }
    }

    public class AdminEmployeeOption
    {
        public string MaNv { get; set; } = string.Empty;
        public string TenNv { get; set; } = string.Empty;
        public string? DienThoai { get; set; }
    }

    public class AdminInboundCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn nhân viên lập phiếu")]
        public string MaNv { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn nhà cung cấp")]
        public string MaNcc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn sản phẩm nhập")]
        public string MaSp { get; set; } = string.Empty;

        [Range(1, 10000, ErrorMessage = "Số lượng nhập phải từ 1 trở lên")]
        public int SoLuong { get; set; } = 1;

        [Range(1000, 1000000000, ErrorMessage = "Đơn giá nhập phải từ 1.000đ trở lên")]
        public decimal DonGia { get; set; }

        [Range(0, 100, ErrorMessage = "Khuyến mại từ 0% đến 100%")]
        public decimal KhuyenMai { get; set; }
    }
}
