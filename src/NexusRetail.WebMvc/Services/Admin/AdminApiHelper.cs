using System.Text.Json;

namespace NexusRetail.WebMvc.Services.Admin
{
    internal static class AdminApiHelper
    {
        public static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response, string fallback)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                return "Tài khoản không có quyền thực hiện thao tác này.";

            var raw = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(raw))
            {
                try
                {
                    using var doc = JsonDocument.Parse(raw);
                    if (doc.RootElement.TryGetProperty("message", out var msg))
                        return msg.GetString() ?? fallback;
                    if (doc.RootElement.TryGetProperty("title", out var title))
                        return title.GetString() ?? fallback;
                }
                catch { }
            }

            return string.IsNullOrWhiteSpace(raw) ? fallback : raw;
        }
    }
}

