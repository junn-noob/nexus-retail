using System.ComponentModel.DataAnnotations;

namespace NexusRetail.Shared.DTOs.BanHang
{
    public class CapNhatTrangThaiDonDto
    {
        [StringLength(30)]
        public string? TrangThaiDonHang { get; set; }

        [StringLength(30)]
        public string? TrangThaiThanhToan { get; set; }
    }
}

