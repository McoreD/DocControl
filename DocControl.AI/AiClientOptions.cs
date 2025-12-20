namespace DocControl.AI;

public sealed class AiClientOptions
{
    public OpenAiOptions OpenAi { get; init; } = new();
    public GeminiOptions Gemini { get; init; } = new();
}

public sealed class OpenAiOptions
{
    public string ApiKey { get; init; } = string.Empty;
    public string Model { get; init; } = "gpt-4.1";
    public Uri Endpoint { get; init; } = new("https://api.openai.com/v1/responses");
}

public sealed class GeminiOptions
{
    public string ApiKey { get; init; } = string.Empty;
    public string Model { get; init; } = "gemini-1.5-pro";
    public Uri Endpoint { get; init; } = new("https://generativelanguage.googleapis.com/v1beta/models/");
}
