namespace NexusRetail.WebMvc.Models
{
    public class CartItem
    {
        public string MaSp { get; set; } = null!;
        public string TenSp { get; set; } = "";
        public string? MaMau { get; set; }
        public string? TenMau { get; set; }
        public string? MaHex { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuong { get; set; } = 1;
        public string? Anh { get; set; }
        public decimal ThanhTien => GiaBan * SoLuong;

        public string CartKey => $"{MaSp}_{(string.IsNullOrWhiteSpace(MaMau) ? "DEFAULT" : MaMau)}";
    }
}
