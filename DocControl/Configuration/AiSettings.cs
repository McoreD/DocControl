using DocControl.AI;

namespace DocControl.Configuration;

public sealed class AiSettings
{
    public AiProvider Provider { get; set; } = AiProvider.OpenAi;
    public string OpenAiModel { get; set; } = "gpt-4.1";
    public string GeminiModel { get; set; } = "gemini-1.5-pro";
    public string OpenAiCredentialName { get; set; } = "DocControl:OpenAI";
    public string GeminiCredentialName { get; set; } = "DocControl:Gemini";
}
