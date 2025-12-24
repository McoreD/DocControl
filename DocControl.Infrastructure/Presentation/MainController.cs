using System.Text.RegularExpressions;
using DocControl.AI;
using DocControl.Core.Configuration;
using DocControl.Infrastructure.Data;
using DocControl.Core.Models;
using DocControl.Infrastructure.Services;

namespace DocControl.Infrastructure.Presentation;

public sealed class MainController
{
    private const string AlphanumericPattern = "^[A-Za-z0-9_-]+$";
    private static readonly Regex Alphanumeric = new(AlphanumericPattern, RegexOptions.Compiled);

    private readonly DocumentService documentService;
    private readonly ImportService importService;
    private readonly NlqService nlqService;
    private readonly ConfigService configService;
    private readonly DocumentConfig documentConfig;
    private readonly AiSettings aiSettings;
    private readonly AiClientOptions aiOptions;
    private readonly AuditRepository auditRepository;
    private readonly RecommendationService recommendationService;
    private readonly DocumentRepository documentRepository;
    private readonly CodeImportService codeImportService;
    private readonly CodeSeriesRepository codeSeriesRepository;
    private readonly DocumentImportService documentImportService;

    public MainController(
        DocumentService documentService,
        ImportService importService,
        NlqService nlqService,
        ConfigService configService,
        DocumentConfig documentConfig,
        AiSettings aiSettings,
        AiClientOptions aiOptions,
        AuditRepository auditRepository,
        RecommendationService recommendationService,
        DocumentRepository documentRepository,
        CodeImportService codeImportService,
        CodeSeriesRepository codeSeriesRepository,
        DocumentImportService documentImportService)
    {
        this.documentService = documentService;
        this.importService = importService;
        this.nlqService = nlqService;
        this.configService = configService;
        this.documentConfig = documentConfig;
        this.aiSettings = aiSettings;
        this.aiOptions = aiOptions;
        this.auditRepository = auditRepository;
        this.recommendationService = recommendationService;
        this.documentRepository = documentRepository;
        this.codeImportService = codeImportService;
        this.codeSeriesRepository = codeSeriesRepository;
        this.documentImportService = documentImportService;
    }

    public async Task<IReadOnlyList<string>> GetLevel1CodesAsync(CancellationToken cancellationToken = default)
        => await codeSeriesRepository.GetLevel1CodesAsync(cancellationToken).ConfigureAwait(false);

    public async Task<IReadOnlyList<string>> GetLevel2CodesAsync(string? level1 = null, CancellationToken cancellationToken = default)
        => await codeSeriesRepository.GetLevel2CodesAsync(level1, cancellationToken).ConfigureAwait(false);

    public async Task<IReadOnlyList<string>> GetLevel3CodesAsync(string? level1 = null, string? level2 = null, CancellationToken cancellationToken = default)
        => await codeSeriesRepository.GetLevel3CodesAsync(level1, level2, cancellationToken).ConfigureAwait(false);

    public async Task<IReadOnlyList<string>> GetLevel4CodesAsync(string? level1 = null, string? level2 = null, string? level3 = null, CancellationToken cancellationToken = default)
        => await codeSeriesRepository.GetLevel4CodesAsync(level1, level2, level3, cancellationToken).ConfigureAwait(false);

    public LevelValidationResult ValidateLevels(CodeSeriesKey key, bool enableLevel4)
    {
        if (string.IsNullOrWhiteSpace(key.Level1) || string.IsNullOrWhiteSpace(key.Level2) || string.IsNullOrWhiteSpace(key.Level3))
        {
            return new LevelValidationResult(false, "Level1-3 are required.");
        }

        if (!IsAlphanumeric(key.Level1) || !IsAlphanumeric(key.Level2) || !IsAlphanumeric(key.Level3))
        {
            return new LevelValidationResult(false, "Levels must be alphanumeric (A-Z, 0-9, _ or -).");
        }

        if (enableLevel4)
        {
            if (string.IsNullOrWhiteSpace(key.Level4))
            {
                return new LevelValidationResult(false, "Level4 is enabled but empty.");
            }

            if (!IsAlphanumeric(key.Level4))
            {
                return new LevelValidationResult(false, "Levels must be alphanumeric (A-Z, 0-9, _ or -).");
            }
        }

        return new LevelValidationResult(true, string.Empty);
    }

    public Task<DocumentCreationResult> GenerateDocumentAsync(CodeSeriesKey key, string freeText, string createdBy, string? extension, CancellationToken cancellationToken = default)
        => documentService.CreateAsync(key, freeText, createdBy, null, extension, cancellationToken);

    public Task<RecommendationResult> RecommendAsync(CodeSeriesKey key, CancellationToken cancellationToken = default)
        => recommendationService.RecommendAsync(key, cancellationToken);

    public Task<NlqResponse?> InterpretAsync(string query, CancellationToken cancellationToken = default)
        => nlqService.InterpretAsync(query, cancellationToken);

    public async Task<DocumentImportOutcome> ImportDocumentsAsync(IEnumerable<string> lines, string createdBy, CancellationToken cancellationToken = default)
    {
        var entries = documentImportService.ParseCodeAndFileLines(lines);
        var valid = 0;
        var invalid = 0;
        var errors = new List<string>();
        var toSeed = new List<(CodeSeriesKey key, string description, int maxNumber)>();
        var toImport = new List<(CodeSeriesKey key, int number, string freeText, string fileName)>();

        foreach (var entry in entries)
        {
            if (!TryParseCodeOnly(entry.Code, out var key, out var number, out var reason))
            {
                invalid++;
                errors.Add($"{entry.Code}: {reason}");
                continue;
            }

            valid++;
            toSeed.Add((key, string.Empty, number));

            var freeText = string.IsNullOrWhiteSpace(entry.FileName) ? string.Empty : entry.FileName;
            var fileName = entry.Code;
            toImport.Add((key, number, freeText, fileName));
        }

        if (toSeed.Count > 0)
        {
            await codeSeriesRepository.SeedNextNumbersAsync(toSeed, cancellationToken).ConfigureAwait(false);
        }

        foreach (var (key, number, freeText, fileName) in toImport)
        {
            await documentRepository.UpsertImportedAsync(key, number, freeText, fileName, createdBy, DateTime.UtcNow, cancellationToken).ConfigureAwait(false);
        }

        var summaries = toImport
            .GroupBy(x => x.key, new CodeSeriesKeyComparer())
            .Select(g => new ImportSeriesSummary(g.Key, g.Max(x => x.number), g.Max(x => x.number) + 1))
            .ToList();

        return new DocumentImportOutcome(valid, invalid, errors, summaries);
    }

    public async Task<int> SeedSeriesAsync(IEnumerable<ImportSeriesSummary> summaries, CancellationToken cancellationToken = default)
    {
        var list = summaries.ToList();
        if (list.Count == 0) return 0;

        await importService.SeedAsync(list, cancellationToken).ConfigureAwait(false);
        return list.Count;
    }

    public async Task<AuditPageResult> LoadAuditPageAsync(int page, int pageSize, string? action = null, string? user = null, CancellationToken cancellationToken = default)
    {
        if (page < 1)
        {
            page = 1;
        }

        var skip = (page - 1) * pageSize;
        var totalCount = await auditRepository.GetCountAsync(action, user, cancellationToken).ConfigureAwait(false);
        var entries = await auditRepository.GetPagedAsync(pageSize, skip, action, user, cancellationToken).ConfigureAwait(false);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));

        return new AuditPageResult(entries, totalPages, page);
    }

    public Task<IReadOnlyList<DocumentRecord>> LoadRecentDocumentsAsync(CancellationToken cancellationToken = default)
        => documentRepository.GetRecentAsync(cancellationToken: cancellationToken);

    public Task<IReadOnlyList<DocumentRecord>> LoadFilteredDocumentsAsync(string? level1Filter = null, string? level2Filter = null, string? level3Filter = null, string? fileNameFilter = null, CancellationToken cancellationToken = default)
    {
        // If no filters provided, return recent documents
        if (string.IsNullOrWhiteSpace(level1Filter) && string.IsNullOrWhiteSpace(level2Filter) && string.IsNullOrWhiteSpace(level3Filter) && string.IsNullOrWhiteSpace(fileNameFilter))
        {
            return documentRepository.GetRecentAsync(cancellationToken: cancellationToken);
        }
        
        return documentRepository.GetFilteredAsync(level1Filter, level2Filter, level3Filter, fileNameFilter, cancellationToken: cancellationToken);
    }

    public Task<DocumentRecord?> GetDocumentAsync(int id, CancellationToken cancellationToken = default)
        => documentRepository.GetByIdAsync(id, cancellationToken);

    public Task ClearDocumentsAsync(CancellationToken cancellationToken = default)
        => documentRepository.ClearAllAsync(cancellationToken);

    public async Task<IReadOnlyList<CodeDisplayItem>> LoadCodesDisplayAsync(bool includeLevel4, CancellationToken cancellationToken = default)
    {
        var items = new List<CodeDisplayItem>();

        var level1Codes = await codeSeriesRepository.GetLevel1CodesWithDescriptionAsync(cancellationToken).ConfigureAwait(false);
        items.AddRange(level1Codes.Select(c => new CodeDisplayItem(1, c.Code, c.Description ?? "")));

        var level2Codes = await codeSeriesRepository.GetLevel2CodesWithDescriptionAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        items.AddRange(level2Codes.Select(c => new CodeDisplayItem(2, c.Code, c.Description ?? "")));

        var level3Codes = await codeSeriesRepository.GetLevel3CodesWithDescriptionAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        items.AddRange(level3Codes.Select(c => new CodeDisplayItem(3, c.Code, c.Description ?? "")));

        if (includeLevel4 && documentConfig.EnableLevel4)
        {
            var level4Codes = await codeSeriesRepository.GetLevel4CodesWithDescriptionAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            items.AddRange(level4Codes.Select(c => new CodeDisplayItem(4, c.Code, c.Description ?? "")));
        }

        return items;
    }

    public Task<CodeImportResult> ImportCodesAsync(string csvContent, CancellationToken cancellationToken = default)
        => codeImportService.ImportCodesFromCsvAsync(csvContent, cancellationToken);

    public Task DeleteCodeAsync(int level, string code, CancellationToken cancellationToken = default)
        => codeSeriesRepository.DeleteCodeAsync(level, code, cancellationToken);

    public Task PurgeAllCodesAsync(CancellationToken cancellationToken = default)
        => codeSeriesRepository.PurgeAllCodesAsync(cancellationToken);

    public SettingsState LoadSettings()
    {
        return new SettingsState(
            documentConfig.PaddingLength,
            documentConfig.Separator,
            documentConfig.EnableLevel4,
            aiSettings.Provider,
            aiSettings.OpenAiModel,
            aiSettings.GeminiModel);
    }

    public async Task SaveSettingsAsync(SettingsState state, string openAiKey, string geminiKey, CancellationToken cancellationToken = default)
    {
        documentConfig.PaddingLength = state.PaddingLength;
        documentConfig.Separator = state.Separator;
        documentConfig.EnableLevel4 = state.EnableLevel4;
        documentConfig.LevelCount = state.EnableLevel4 ? 4 : 3;

        aiSettings.Provider = state.Provider;
        aiSettings.OpenAiModel = state.OpenAiModel;
        aiSettings.GeminiModel = state.GeminiModel;

        // Save config and keys first
        await configService.SaveDocumentConfigAsync(documentConfig, cancellationToken).ConfigureAwait(false);
        await configService.SaveAiSettingsAsync(aiSettings, openAiKey, geminiKey, cancellationToken).ConfigureAwait(false);

        // Rebuild options to pick up newly saved keys
        var refreshed = await configService.BuildAiOptionsAsync(aiSettings, cancellationToken).ConfigureAwait(false);
        aiOptions.DefaultProvider = refreshed.DefaultProvider;
        aiOptions.OpenAi.ApiKey = refreshed.OpenAi.ApiKey;
        aiOptions.OpenAi.Model = refreshed.OpenAi.Model;
        aiOptions.OpenAi.Endpoint = refreshed.OpenAi.Endpoint;
        aiOptions.Gemini.ApiKey = refreshed.Gemini.ApiKey;
        aiOptions.Gemini.Model = refreshed.Gemini.Model;
        aiOptions.Gemini.Endpoint = refreshed.Gemini.Endpoint;
    }

    public async Task<NlqResponse?> RecommendWithAiAsync(string query, CancellationToken cancellationToken = default)
    {
        var codes = await LoadCodesDisplayAsync(includeLevel4: documentConfig.EnableLevel4, cancellationToken).ConfigureAwait(false);
        var catalog = codes.Select(c => (c.Level, c.Code, c.Description)).ToList();
        return await nlqService.RecommendCodeAsync(query, catalog, cancellationToken).ConfigureAwait(false);
    }

    public Task<int> GetDocumentsTotalCountAsync(CancellationToken cancellationToken = default)
        => documentRepository.GetTotalCountAsync(cancellationToken);

    private bool TryParseCodeOnly(string code, out CodeSeriesKey key, out int number, out string reason)
    {
        key = new CodeSeriesKey { Level1 = string.Empty, Level2 = string.Empty, Level3 = string.Empty, Level4 = null };
        number = 0;
        reason = string.Empty;

        if (string.IsNullOrWhiteSpace(code))
        {
            reason = "Empty code";
            return false;
        }

        var parts = code.Trim().Split(documentConfig.Separator, StringSplitOptions.RemoveEmptyEntries);
        if (documentConfig.EnableLevel4)
        {
            if (parts.Length != 5)
            {
                reason = "Expected Level1-4 and number";
                return false;
            }

            if (!int.TryParse(parts[4], out number))
            {
                reason = "Number not numeric";
                return false;
            }

            key = new CodeSeriesKey { Level1 = parts[0], Level2 = parts[1], Level3 = parts[2], Level4 = parts[3] };
            return true;
        }

        if (parts.Length != 4)
        {
            reason = "Expected Level1-3 and number";
            return false;
        }

        if (!int.TryParse(parts[3], out number))
        {
            reason = "Number not numeric";
            return false;
        }

        key = new CodeSeriesKey { Level1 = parts[0], Level2 = parts[1], Level3 = parts[2], Level4 = null };
        return true;
    }

    private static bool IsAlphanumeric(string? value)
        => !string.IsNullOrWhiteSpace(value) && Alphanumeric.IsMatch(value);
}

public sealed record LevelValidationResult(bool IsValid, string Message);

public sealed record DocumentImportOutcome(int ValidCount, int InvalidCount, IReadOnlyList<string> Errors, IReadOnlyList<ImportSeriesSummary> Summaries);

public sealed record AuditPageResult(IReadOnlyList<AuditEntry> Entries, int TotalPages, int Page);

public sealed record CodeDisplayItem(int Level, string Code, string Description);

public sealed record SettingsState(int PaddingLength, string Separator, bool EnableLevel4, AiProvider Provider, string OpenAiModel, string GeminiModel);
