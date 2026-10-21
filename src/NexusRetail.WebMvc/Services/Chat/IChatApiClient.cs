namespace NexusRetail.WebMvc.Services.Chat
{
    public class ChatApiResponse
    {
        public string Reply { get; set; } = string.Empty;
        public object[] Items { get; set; } = Array.Empty<object>();
        public string Source { get; set; } = string.Empty;
        public string? Note { get; set; }
    }

    public interface IChatApiClient
    {
        Task<string> SendMessageJsonAsync(string message);
    }
}
