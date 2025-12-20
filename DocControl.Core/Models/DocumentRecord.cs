namespace DocControl.Core.Models;

public sealed class DocumentRecord
{
    public int Id { get; init; }
    public string Level1 { get; init; } = string.Empty;
    public string Level2 { get; init; } = string.Empty;
    public string Level3 { get; init; } = string.Empty;
    public string? Level4 { get; init; }
    public int Number { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string CreatedBy { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
}
