using System;
using System.Collections.Generic;

namespace NexusRetail.Shared.Entities;

public partial class ChatbotTuVanLog
{
    public int LogId { get; set; }

    public string? SessionId { get; set; }

    public string? NhuCauKhach { get; set; }

    public decimal? NganSachToiDa { get; set; }

    public string? MaSpGoiY { get; set; }

    public string? PhanHoiChatbot { get; set; }

    public DateTime ThoiGian { get; set; }

    public virtual SanPham? MaSpGoiYNavigation { get; set; }
}
