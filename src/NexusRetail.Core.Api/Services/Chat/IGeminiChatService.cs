namespace NexusRetail.Core.Api.Services.Chat
{
    public interface IGeminiChatService
    {
        Task<(string Reply, string Source, string? Note)> GenerateConsultationAsync(string systemPrompt, string userMessage);
        string GenerateSmartRecommendation(string message);
    }
}
