namespace DocControl.Core.Models;

public sealed class CodeSeriesKey
{
    public required string Level1 { get; init; }
    public required string Level2 { get; init; }
    public required string Level3 { get; init; }
    public string? Level4 { get; init; }
}
