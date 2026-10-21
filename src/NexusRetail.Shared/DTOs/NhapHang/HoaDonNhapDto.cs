using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NexusRetail.Shared.DTOs.NhapHang
{
    public class HoaDonNhapDto
    {
        [Required(ErrorMessage = "Mã nhân viên nhập không được để trống!")]
        [StringLength(20, ErrorMessage = "Mã nhân viên chỉ được nhập tối đa 20 ký tự")]
        public string MaNv { get; set; } = null!;

        [Required(ErrorMessage = "Mã nhà cung cấp không được để trống!")]
        [StringLength(20, ErrorMessage = "Mã nhà cung cấp chỉ được nhập tối đa 20 ký tự")]
        public string MaNcc { get; set; } = null!;

        [Required(ErrorMessage = "Danh sách hàng nhập không được để trống!")]
        [MinLength(1, ErrorMessage = "Hóa đơn nhập phải có ít nhất 1 sản phẩm!")]
        public List<ChiTietHdnDto> DanhSachMon { get; set; } = new();
    }
}
