namespace NexusRetail.WebMvc.Models
{
    public class CartItem
    {
        public string MaSp { get; set; } = null!;
        public string TenSp { get; set; } = "";
        public decimal GiaBan { get; set; }
        public int SoLuong { get; set; } = 1;
        public string? Anh { get; set; }
        public decimal ThanhTien => GiaBan * SoLuong;
    }
}
