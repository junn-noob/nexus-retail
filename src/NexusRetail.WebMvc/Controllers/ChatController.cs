using Microsoft.AspNetCore.Mvc;
using NexusRetail.WebMvc.Services.Chat;

namespace NexusRetail.WebMvc.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IChatApiClient _chatClient;

        public ChatController(IChatApiClient chatClient)
        {
            _chatClient = chatClient;
        }

        public class ChatRequest
        {
            public string Message { get; set; } = "";
            public string? SessionId { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] ChatRequest req)
        {
            string sessionId = !string.IsNullOrWhiteSpace(req?.SessionId)
                ? req.SessionId.Trim()
                : HttpContext.Session.Id;
            var json = await _chatClient.SendMessageJsonAsync(req?.Message ?? "", sessionId);
            return Content(json, "application/json");
        }
    }
}
