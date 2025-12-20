namespace DocControl.AI;

public sealed class AiClientOptions
{
    public AiProvider DefaultProvider { get; set; } = AiProvider.OpenAi;
    public OpenAiOptions OpenAi { get; set; } = new();
    public GeminiOptions Gemini { get; set; } = new();
}

public sealed class OpenAiOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4.1";
    public Uri Endpoint { get; set; } = new("https://api.openai.com/v1/responses");
}

public sealed class GeminiOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gemini-1.5-pro";
    public Uri Endpoint { get; set; } = new("https://generativelanguage.googleapis.com/v1beta/models/");
}
