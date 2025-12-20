using DocControl.Data;
using DocControl.Models;

namespace DocControl.Services;

public sealed class CodeImportService
{
    private readonly CodeSeriesRepository codeSeriesRepository;

    public CodeImportService(CodeSeriesRepository codeSeriesRepository)
    {
        this.codeSeriesRepository = codeSeriesRepository;
    }

    public async Task<CodeImportResult> ImportCodesFromCsvAsync(string csvContent, CancellationToken cancellationToken = default)
    {
        var result = new CodeImportResult();
        var lines = csvContent.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        
        if (lines.Length == 0)
        {
            result.AddError("CSV file is empty");
            return result;
        }

        // Skip header row if it exists
        var dataLines = lines.Skip(1);
        var seriesToSeed = new List<(CodeSeriesKey key, int maxNumber)>();
        
        foreach (var line in dataLines)
        {
            var parts = ParseCsvLine(line);
            if (parts.Length < 3) continue;

            if (!int.TryParse(parts[0].Trim(), out var level) || level < 1 || level > 4)
            {
                result.AddError($"Invalid level '{parts[0]}' in line: {line}");
                continue;
            }

            var code = parts[1].Trim();
            var description = parts[2].Trim();

            if (string.IsNullOrWhiteSpace(code))
            {
                result.AddError($"Empty code in line: {line}");
                continue;
            }

            try
            {
                var codeSeriesKey = CreateCodeSeriesKey(level, code);
                seriesToSeed.Add((codeSeriesKey, 0)); // Start with next number 1
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                result.AddError($"Failed to process code '{code}': {ex.Message}");
            }
        }

        // Seed all code series at once
        if (seriesToSeed.Count > 0)
        {
            await codeSeriesRepository.SeedNextNumbersAsync(seriesToSeed, cancellationToken);
        }

        return result;
    }

    private static CodeSeriesKey CreateCodeSeriesKey(int level, string code)
    {
        return level switch
        {
            1 => new CodeSeriesKey { Level1 = code, Level2 = "", Level3 = "", Level4 = null },
            2 => new CodeSeriesKey { Level1 = "", Level2 = code, Level3 = "", Level4 = null },
            3 => new CodeSeriesKey { Level1 = "", Level2 = "", Level3 = code, Level4 = null },
            4 => new CodeSeriesKey { Level1 = "", Level2 = "", Level3 = "", Level4 = code },
            _ => throw new ArgumentException($"Invalid level: {level}")
        };
    }

    private static string[] ParseCsvLine(string line)
    {
        var result = new List<string>();
        var current = new System.Text.StringBuilder();
        var inQuotes = false;
        
        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];
            
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }
        
        result.Add(current.ToString());
        return result.ToArray();
    }
}

public sealed class CodeImportResult
{
    private readonly List<string> errors = [];
    
    public int SuccessCount { get; set; }
    public IReadOnlyList<string> Errors => errors;
    public bool HasErrors => errors.Count > 0;
    
    public void AddError(string error) => errors.Add(error);
}