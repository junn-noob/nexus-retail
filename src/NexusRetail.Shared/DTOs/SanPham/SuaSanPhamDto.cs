using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NexusRetail.Shared.DTOs.SanPham
{
    public class SuaSanPhamDto
    {
        [Required(ErrorMessage = "Tên sản phẩm không được để trống!")]
        [StringLength(150, ErrorMessage = "Tên sản phẩm chỉ được nhập tối đa 150 ký tự")]
        public string TenSp { get; set; } = null!;

        [Required(ErrorMessage = "Mã loại không được để trống!")]
        [StringLength(20, ErrorMessage = "Mã loại chỉ được nhập tối đa 20 ký tự")]
        public string MaLoai { get; set; } = null!;

        [Required(ErrorMessage = "Mã nhãn hiệu không được để trống!")]
        [StringLength(20, ErrorMessage = "Mã nhãn hiệu chỉ được nhập tối đa 20 ký tự")]
        public string MaNhanHieu { get; set; } = null!;

        [Required(ErrorMessage = "Mã màn hình không được để trống!")]
        [StringLength(20, ErrorMessage = "Mã màn hình chỉ được nhập tối đa 20 ký tự")]
        public string MaManHinh { get; set; } = null!;

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá nhập phải lớn hơn 0!")]
        public decimal GiaNhap { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0!")]
        public int SoLuong { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Thời gian bảo hành phải lớn hơn 0!")]
        public int ThoiGianBaoHanh { get; set; } = 12;

        public string? AmThanh { get; set; }

        public string? ChupAnh { get; set; }

        public string? Anh { get; set; }

        public string? GhiChu { get; set; }
    }
}
