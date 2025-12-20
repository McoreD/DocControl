using DocControl.Infrastructure.Data;
using DocControl.Core.Models;

namespace DocControl.Infrastructure.Services;

public sealed class ImportService
{
    private readonly FileNameParser parser;
    private readonly CodeSeriesRepository seriesRepo;

    public ImportService(FileNameParser parser, CodeSeriesRepository seriesRepo)
    {
        this.parser = parser;
        this.seriesRepo = seriesRepo;
    }

    public Task<ImportResult> ImportFilesAsync(IEnumerable<string> filePaths, bool seedCounters = true, CancellationToken cancellationToken = default)
    {
        var fileNames = filePaths.Select(f => Path.GetFileName(f) ?? string.Empty);
        return ImportFileNamesAsync(fileNames, seedCounters, cancellationToken);
    }

    public async Task<ImportResult> ImportFileNamesAsync(IEnumerable<string> fileNames, bool seedCounters = true, CancellationToken cancellationToken = default)
    {
        var valid = new List<ParsedFileName>();
        var invalid = new List<InvalidImportEntry>();

        foreach (var file in fileNames)
        {
            if (string.IsNullOrWhiteSpace(file)) continue;
            if (parser.TryParse(file.Trim(), out var parsed, out var reason))
            {
                valid.Add(parsed!);
            }
            else
            {
                invalid.Add(new InvalidImportEntry(file.Trim(), reason));
            }
        }

        var summaries = valid
            .GroupBy(v => v.SeriesKey, new CodeSeriesKeyComparer())
            .Select(g => new ImportSeriesSummary(g.Key, g.Max(x => x.Number), g.Max(x => x.Number) + 1))
            .ToList();

        if (seedCounters && summaries.Count > 0)
        {
            var grouped = summaries.Select(s => (s.SeriesKey, max: s.MaxNumber)).ToList();
            await seriesRepo.SeedNextNumbersAsync(grouped, cancellationToken).ConfigureAwait(false);
        }

        return new ImportResult(valid, invalid, summaries);
    }

    public async Task SeedAsync(IEnumerable<ImportSeriesSummary> summaries, CancellationToken cancellationToken = default)
    {
        var grouped = summaries.Select(s => (s.SeriesKey, max: s.MaxNumber)).ToList();
        if (grouped.Count == 0) return;
        await seriesRepo.SeedNextNumbersAsync(grouped, cancellationToken).ConfigureAwait(false);
    }
}

public sealed record ImportResult(IReadOnlyList<ParsedFileName> Valid, IReadOnlyList<InvalidImportEntry> Invalid, IReadOnlyList<ImportSeriesSummary> Summaries);

public sealed record InvalidImportEntry(string FileName, string Reason);

public sealed record ImportSeriesSummary(CodeSeriesKey SeriesKey, int MaxNumber, int NextNumber);

internal sealed class CodeSeriesKeyComparer : IEqualityComparer<CodeSeriesKey>
{
    public bool Equals(CodeSeriesKey? x, CodeSeriesKey? y)
    {
        if (x is null || y is null) return false;
        return string.Equals(x.Level1, y.Level1, StringComparison.OrdinalIgnoreCase)
            && string.Equals(x.Level2, y.Level2, StringComparison.OrdinalIgnoreCase)
            && string.Equals(x.Level3, y.Level3, StringComparison.OrdinalIgnoreCase)
            && string.Equals(x.Level4 ?? string.Empty, y.Level4 ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(CodeSeriesKey obj)
    {
        var l4 = obj.Level4 ?? string.Empty;
        return HashCode.Combine(obj.Level1.ToLowerInvariant(), obj.Level2.ToLowerInvariant(), obj.Level3.ToLowerInvariant(), l4.ToLowerInvariant());
    }
}
