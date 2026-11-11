using System.Net.Http.Headers;

namespace NexusRetail.WinForms.Services;

/// <summary>
/// Ngữ cảnh phiên làm việc và kết nối HTTP dùng chung cho các dịch vụ WinForms POS.
/// </summary>
public class PosSessionContext
{
    public HttpClient Client { get; private set; }
    public string BaseUrl { get; private set; } = "https://localhost:7143/";
    public string? Token { get; private set; }
    public string? CurrentUser { get; private set; }
    public string? CurrentRole { get; private set; }

    public PosSessionContext()
    {
        Client = CreateHttpClient(BaseUrl);
    }

    public static HttpClient CreateHttpClient(string baseUrl)
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        return new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromSeconds(15)
        };
    }

    public void SwitchPort(string newBaseUrl)
    {
        if (BaseUrl == newBaseUrl) return;

        var oldClient = Client;
        BaseUrl = newBaseUrl;
        Client = CreateHttpClient(newBaseUrl);
        if (!string.IsNullOrEmpty(Token))
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
        }
        oldClient.Dispose();
    }

    public void SetAuthenticatedUser(string username, string role, string? token)
    {
        CurrentUser = username;
        CurrentRole = role;
        Token = token;

        Client.DefaultRequestHeaders.Authorization = !string.IsNullOrEmpty(token)
            ? new AuthenticationHeaderValue("Bearer", token)
            : null;
    }

    public void ClearSession()
    {
        CurrentUser = null;
        CurrentRole = null;
        Token = null;
        Client.DefaultRequestHeaders.Authorization = null;
    }
}

