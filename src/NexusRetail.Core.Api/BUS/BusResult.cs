using Microsoft.AspNetCore.Mvc;

namespace NexusRetail.Core.Api.BUS;

/// <summary>
/// Kết quả xử lý chuẩn hóa từ tầng nghiệp vụ (BUS) trả về cho Controller (GUI/API).
/// Giúp tách biệt hoàn toàn logic nghiệp vụ khỏi ASP.NET Core HTTP ActionResult.
/// </summary>
public sealed record BusResult<T>(bool IsSuccess, int StatusCode, T? Data = default, string? ErrorMessage = null)
{
    public static BusResult<T> Ok(T data) => new(true, StatusCodes.Status200OK, data);

    public static BusResult<T> Created(T data) => new(true, StatusCodes.Status201Created, data);

    public static BusResult<T> NoContent() => new(true, StatusCodes.Status204NoContent);

    public static BusResult<T> BadRequest(string message) => new(false, StatusCodes.Status400BadRequest, default, message);

    public static BusResult<T> NotFound(string message = "Không tìm thấy dữ liệu yêu cầu!")
        => new(false, StatusCodes.Status404NotFound, default, message);

    public static BusResult<T> Failure(string message) => new(false, StatusCodes.Status500InternalServerError, default, message);

    public IActionResult ToActionResult()
    {
        if (IsSuccess)
        {
            return StatusCode switch
            {
                StatusCodes.Status200OK => new OkObjectResult(Data),
                StatusCodes.Status204NoContent => new NoContentResult(),
                _ => new ObjectResult(Data) { StatusCode = StatusCode }
            };
        }

        var errorPayload = new { message = ErrorMessage ?? "Yêu cầu không hợp lệ." };
        return StatusCode switch
        {
            StatusCodes.Status400BadRequest => new BadRequestObjectResult(errorPayload),
            StatusCodes.Status404NotFound => new NotFoundObjectResult(errorPayload),
            StatusCodes.Status409Conflict => new ConflictObjectResult(errorPayload),
            _ => new ObjectResult(errorPayload) { StatusCode = StatusCode }
        };
    }
}
