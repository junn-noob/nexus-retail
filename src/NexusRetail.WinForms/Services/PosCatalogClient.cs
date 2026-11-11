using NexusRetail.Shared.DTOs.Common;
using NexusRetail.Shared.DTOs.SanPham;
using System.Net.Http.Json;
using System.Text.Json;

namespace NexusRetail.WinForms.Services;

/// <summary>
/// Dịch vụ CRUD danh mục sản phẩm và bộ lọc thuộc tính cho WinForms POS.
/// </summary>
public class PosCatalogClient : PosApiClientBase, IPosCatalogClient
{
    public PosCatalogClient(PosSessionContext session) : base(session)
    {
    }

    public async Task<List<SanPhamDto>> GetProductsAsync(string? q = null)
    {
        string url = "api/san-pham?page=1&pageSize=100";
        if (!string.IsNullOrWhiteSpace(q))
            url += $"&q={Uri.EscapeDataString(q)}";

        try
        {
            var res = await Client.GetFromJsonAsync<PagedResultDto<SanPhamDto>>(url);
            return res?.Items ?? new List<SanPhamDto>();
        }
        catch
        {
            if (Session.BaseUrl.Contains("7143"))
            {
                try
                {
                    using var altClient = PosSessionContext.CreateHttpClient("http://localhost:5050/");
                    var res = await altClient.GetFromJsonAsync<PagedResultDto<SanPhamDto>>(url);
                    if (res?.Items != null && res.Items.Count > 0)
                    {
                        Session.SwitchPort("http://localhost:5050/");
                        return res.Items;
                    }
                }
                catch { }
            }

            return new List<SanPhamDto>();
        }
    }

    public async Task<CategoryFilterResult?> GetCategoryFilterAsync()
    {
        try
        {
            return await Client.GetFromJsonAsync<CategoryFilterResult>("api/san-pham/bo-loc-danh-muc");
        }
        catch
        {
            return null;
        }
    }

    public async Task<(bool Success, string Message, string? MaSp)> CreateProductAsync(ThemSanPhamDto dto)
    {
        try
        {
            var response = await Client.PostAsJsonAsync("api/san-pham", dto);
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadFromJsonAsync<JsonElement>();
                string? maSp = res.TryGetProperty("maSp", out var m) ? m.GetString() : null;
                return (true, "Thêm mới sản phẩm thành công!", maSp);
            }

            return (false, await FormatApiErrorAsync(response, "Lỗi từ máy chủ"), null);
        }
        catch (Exception ex)
        {
            return (false, $"Không thể kết nối đến máy chủ: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string Message)> UpdateProductAsync(string maSp, SuaSanPhamDto dto)
    {
        try
        {
            var response = await Client.PutAsJsonAsync($"api/san-pham/{Uri.EscapeDataString(maSp)}", dto);
            if (response.IsSuccessStatusCode)
                return (true, "Cập nhật sản phẩm thành công!");

            return (false, await FormatApiErrorAsync(response, "Lỗi cập nhật"));
        }
        catch (Exception ex)
        {
            return (false, $"Không thể kết nối đến máy chủ: {ex.Message}");
        }
    }

    public async Task<(bool Success, string Message)> DeleteProductAsync(string maSp)
    {
        try
        {
            var response = await Client.DeleteAsync($"api/san-pham/{Uri.EscapeDataString(maSp)}");
            if (response.IsSuccessStatusCode)
                return (true, "Xóa sản phẩm thành công!");

            return (false, await FormatApiErrorAsync(response, "Không thể xóa"));
        }
        catch (Exception ex)
        {
            return (false, $"Không thể kết nối đến máy chủ: {ex.Message}");
        }
    }
}

