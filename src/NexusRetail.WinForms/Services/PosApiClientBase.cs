using System.Net;
using System.Text.Json;

namespace NexusRetail.WinForms.Services;

/// <summary>
/// Lớp cơ sở dùng chung cho các client gọi REST API từ WinForms POS.
/// </summary>
public abstract class PosApiClientBase
{
    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected PosSessionContext Session { get; }
    protected HttpClient Client => Session.Client;

    protected PosApiClientBase(PosSessionContext session)
    {
        Session = session;
    }

    protected static async Task<string> FormatApiErrorAsync(HttpResponseMessage response, string prefix)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return "Phiên đăng nhập chưa hợp lệ hoặc đã hết hạn. Vui lòng đăng nhập lại!";
        if (response.StatusCode == HttpStatusCode.Forbidden)
            return "Tài khoản hiện tại không có quyền thực hiện thao tác này!";

        var raw = await response.Content.ReadAsStringAsync();
        if (!string.IsNullOrWhiteSpace(raw))
        {
            try
            {
                using var doc = JsonDocument.Parse(raw);
                if (doc.RootElement.TryGetProperty("message", out var msg))
                    return $"{prefix}: {msg.GetString()}";
                if (doc.RootElement.TryGetProperty("title", out var title))
                    return $"{prefix}: {title.GetString()}";
            }
            catch { }
        }

        return string.IsNullOrWhiteSpace(raw) ? $"{prefix} (HTTP {(int)response.StatusCode})" : $"{prefix}: {raw}";
    }

    protected async Task<List<T>> GetPropertyListAsync<T>(string url, string propertyName)
    {
        try
        {
            using var res = await Client.GetAsync(url);
            if (!res.IsSuccessStatusCode) return new();

            var json = await res.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty(propertyName, out var arr))
            {
                return JsonSerializer.Deserialize<List<T>>(arr.GetRawText(), JsonOptions) ?? new();
            }
        }
        catch { }

        return new();
    }
}

