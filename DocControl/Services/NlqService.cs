using System.Text.Json;
using DocControl.AI;
using DocControl.Models;

namespace DocControl.Services;

public sealed class NlqService
{
    private readonly AiOrchestrator orchestrator;

    public NlqService(AiOrchestrator orchestrator)
    {
        this.orchestrator = orchestrator;
    }

    public async Task<NlqResponse?> InterpretAsync(string query, CancellationToken cancellationToken = default)
    {
        var schema = JsonDocument.Parse("""
{
  "type": "object",
  "properties": {
    "documentType": { "type": "string" },
    "owner": { "type": "string" },
    "level1": { "type": "string" },
    "level2": { "type": "string" },
    "level3": { "type": "string" },
    "level4": { "type": "string" },
    "freeText": { "type": "string" }
  },
  "required": ["level1", "level2", "level3", "freeText"]
}
""").RootElement.Clone();

        var request = new AiStructuredRequest
        {
            Prompt = query,
            ResponseSchema = schema,
            SystemInstruction = "Return a strict JSON object with documentType, owner, level1, level2, level3, optional level4, and freeText. Do not return natural language."
        };

        var result = await orchestrator.ExecuteAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (!result.IsSuccess || result.ParsedPayload is null) return null;

        try
        {
            var payload = result.ParsedPayload.Value;
            return new NlqResponse
            {
                DocumentType = payload.TryGetProperty("documentType", out var dt) ? dt.GetString() : null,
                Owner = payload.TryGetProperty("owner", out var ow) ? ow.GetString() : null,
                Level1 = payload.GetProperty("level1").GetString() ?? string.Empty,
                Level2 = payload.GetProperty("level2").GetString() ?? string.Empty,
                Level3 = payload.GetProperty("level3").GetString() ?? string.Empty,
                Level4 = payload.TryGetProperty("level4", out var l4) ? l4.GetString() : null,
                FreeText = payload.GetProperty("freeText").GetString() ?? string.Empty
            };
        }
        catch
        {
            return null;
        }
    }
}
