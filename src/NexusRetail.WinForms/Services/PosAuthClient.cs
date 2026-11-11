using NexusRetail.Shared.DTOs.Auth;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace NexusRetail.WinForms.Services;

/// <summary>
/// Dịch vụ xác thực tài khoản và tự động phát hiện cổng Core API cho WinForms POS.
/// </summary>
public class PosAuthClient : PosApiClientBase, IPosAuthClient
{
    private static readonly string[] AuthCandidateUrls =
    [
        "http://localhost:5001/",
        "http://localhost:5000/",
        "https://localhost:7143/",
        "http://localhost:5050/"
    ];

    private static readonly string[] CoreCandidateUrls =
    [
        "https://localhost:7143/",
        "http://localhost:5050/",
        "http://localhost:5000/"
    ];

    public PosAuthClient(PosSessionContext session) : base(session)
    {
    }

    public string? CurrentUser => Session.CurrentUser;
    public string? CurrentRole => Session.CurrentRole;

    public async Task<(bool Success, string Role, string Message)> LoginAsync(string username, string password)
    {
        const string defaultError = "Tên đăng nhập hoặc mật khẩu không chính xác!";
        bool reachedAnyAuthService = false;

        foreach (var authUrl in AuthCandidateUrls)
        {
            try
            {
                using var authClient = PosSessionContext.CreateHttpClient(authUrl);
                var response = await authClient.PostAsJsonAsync("api/auth/login", new { username, password });

                if (response.StatusCode == HttpStatusCode.NotFound)
                    continue;

                reachedAnyAuthService = true;

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    string role = result?.Role ?? "Staff";
                    Session.SetAuthenticatedUser(result?.Username ?? username, role, result?.Token);
                    await EnsureCoreApiConnectedAsync();
                    return (true, role, "Đăng nhập thành công!");
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return (false, "", defaultError);

                try
                {
                    var errObj = await response.Content.ReadFromJsonAsync<JsonElement>();
                    if (errObj.TryGetProperty("message", out var m))
                        return (false, "", m.GetString() ?? defaultError);
                }
                catch { }

                return (false, "", defaultError);
            }
            catch
            {
                // Thử candidate URL tiếp theo nếu cổng hiện tại chưa chạy.
            }
        }

        if (!reachedAnyAuthService)
        {
            return (false, "", "Không thể kết nối đến máy chủ xác thực Identity (Port 5001) hoặc Gateway (Port 5000)!\nVui lòng kiểm tra dịch vụ NexusRetail.Identity.Api đã được khởi động chưa.");
        }

        return (false, "", defaultError);
    }

    public void Logout() => Session.ClearSession();

    private async Task EnsureCoreApiConnectedAsync()
    {
        foreach (var url in CoreCandidateUrls)
        {
            try
            {
                using var testClient = PosSessionContext.CreateHttpClient(url);
                var testRes = await testClient.GetAsync("api/san-pham?page=1&pageSize=1");
                if (testRes.IsSuccessStatusCode)
                {
                    Session.SwitchPort(url);
                    break;
                }
            }
            catch { }
        }
    }
}

