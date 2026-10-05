using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NexusRetail.Shared.DTOs.BanHang
{
    public class ChiTietHdbDto
    {
        [Required(ErrorMessage = "Mã sản phẩm bán không được trống!")]
        [StringLength(20, ErrorMessage = "Mã sản phẩm chỉ được nhập tối đa 20 ký tự")]
        public string MaSp { get; set; } = null!;

        public string? TenSp { get; set; } = null!;

        [Required(ErrorMessage = "Số lượng bán không được để trống!")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng bán phải lớn hơn 0 và tối thiểu 1 sản phẩm!")]        
        public int SoLuong { get; set; }

        [Range(0, 100, ErrorMessage = "Khuyến mãi phải trong khoảng 0% - 100%")]
        public decimal KhuyenMai { get; set; } = 0;
    }
}
