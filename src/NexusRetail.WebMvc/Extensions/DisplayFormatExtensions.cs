namespace NexusRetail.WebMvc.Extensions;

/// <summary>
/// Chuẩn hóa nhãn hiển thị tiếng Việt cho trạng thái và phương thức thanh toán trên toàn bộ giao diện Web.
/// </summary>
public static class DisplayFormatExtensions
{
    public static string FormatPaymentMethod(string? method) => method?.Trim() switch
    {
        "Cash" => "Tiền mặt tại quầy",
        "VietQR" => "Chuyển khoản VietQR",
        "COD" => "Thanh toán khi nhận hàng (COD)",
        "Card" => "Thẻ ngân hàng",
        null or "" => "Tiền mặt",
        var other => other
    };
}

