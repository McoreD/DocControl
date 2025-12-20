namespace DocControl.Models;

public sealed class AuditEntry
{
    public int Id { get; init; }
    public string Action { get; init; } = string.Empty;
    public string? Payload { get; init; }
    public string CreatedBy { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public int? DocumentId { get; init; }
}
