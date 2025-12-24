using DocControl.Core.Models;
using Microsoft.Data.Sqlite;

namespace DocControl.Infrastructure.Data;

public sealed class CodeSeriesRepository
{
    private readonly DbConnectionFactory factory;
    private readonly CodeCatalogStore codeStore;
    private bool initialized;

    public CodeSeriesRepository(DbConnectionFactory factory, CodeCatalogStore codeStore)
    {
        this.factory = factory;
        this.codeStore = codeStore;
    }

    public async Task SeedNextNumbersAsync(IEnumerable<(CodeSeriesKey key, string description, int maxNumber)> seriesMax, CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync(cancellationToken).ConfigureAwait(false);

        // Update JSON catalog for single-level codes
        var catalogUpserts = new List<CodeCatalogEntry>();
        foreach (var (key, description, _) in seriesMax)
        {
            if (TryGetSingleLevel(key, out var level, out var code))
            {
                catalogUpserts.Add(new CodeCatalogEntry(level, code, string.IsNullOrWhiteSpace(description) ? null : description));
            }
        }
        if (catalogUpserts.Count > 0)
        {
            await codeStore.UpsertAsync(catalogUpserts, cancellationToken).ConfigureAwait(false);
        }

        // Maintain SQLite NextNumber for document numbering (unchanged behavior)
        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var tx = (SqliteTransaction)await conn.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        foreach (var (key, description, max) in seriesMax)
        {
            var upsert = conn.CreateCommand();
            upsert.Transaction = tx;
            upsert.CommandText = @"
                INSERT INTO CodeSeries (Level1, Level2, Level3, Level4, Description, NextNumber)
                VALUES ($l1, $l2, $l3, $l4, $desc, $next)
                ON CONFLICT(Level1, Level2, Level3, Level4)
                DO UPDATE SET 
                    Description = COALESCE(excluded.Description, CodeSeries.Description),
                    NextNumber = CASE WHEN excluded.NextNumber > CodeSeries.NextNumber THEN excluded.NextNumber ELSE CodeSeries.NextNumber END;
            ";
            upsert.Parameters.AddWithValue("$l1", key.Level1);
            upsert.Parameters.AddWithValue("$l2", key.Level2);
            upsert.Parameters.AddWithValue("$l3", key.Level3);
            upsert.Parameters.AddWithValue("$l4", (object?)key.Level4 ?? DBNull.Value);
            upsert.Parameters.AddWithValue("$desc", (object?)description ?? DBNull.Value);
            upsert.Parameters.AddWithValue("$next", max + 1);
            await upsert.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        await tx.CommitAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<string>> GetLevel1CodesAsync(CancellationToken cancellationToken = default)
    {
        var codes = await GetCatalogAsync(cancellationToken).ConfigureAwait(false);
        return codes.Where(c => c.Level == 1).Select(c => c.Code).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(c => c).ToList();
    }

    public async Task<List<(string Code, string? Description)>> GetLevel1CodesWithDescriptionAsync(CancellationToken cancellationToken = default)
    {
        var codes = await GetCatalogAsync(cancellationToken).ConfigureAwait(false);
        return codes.Where(c => c.Level == 1)
            .OrderBy(c => c.Code)
            .Select(c => (c.Code, c.Description))
            .ToList();
    }

    public async Task<List<string>> GetLevel2CodesAsync(string? level1Filter = null, CancellationToken cancellationToken = default)
    {
        var codes = await GetCatalogAsync(cancellationToken).ConfigureAwait(false);
        return codes.Where(c => c.Level == 2).Select(c => c.Code).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(c => c).ToList();
    }

    public async Task<List<(string Code, string? Description)>> GetLevel2CodesWithDescriptionAsync(string? level1Filter = null, CancellationToken cancellationToken = default)
    {
        var codes = await GetCatalogAsync(cancellationToken).ConfigureAwait(false);
        return codes.Where(c => c.Level == 2)
            .OrderBy(c => c.Code)
            .Select(c => (c.Code, c.Description))
            .ToList();
    }

    public async Task<List<string>> GetLevel3CodesAsync(string? level1Filter = null, string? level2Filter = null, CancellationToken cancellationToken = default)
    {
        var codes = await GetCatalogAsync(cancellationToken).ConfigureAwait(false);
        return codes.Where(c => c.Level == 3).Select(c => c.Code).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(c => c).ToList();
    }

    public async Task<List<(string Code, string? Description)>> GetLevel3CodesWithDescriptionAsync(string? level1Filter = null, string? level2Filter = null, CancellationToken cancellationToken = default)
    {
        var codes = await GetCatalogAsync(cancellationToken).ConfigureAwait(false);
        return codes.Where(c => c.Level == 3)
            .OrderBy(c => c.Code)
            .Select(c => (c.Code, c.Description))
            .ToList();
    }

    public async Task<List<string>> GetLevel4CodesAsync(string? level1Filter = null, string? level2Filter = null, string? level3Filter = null, CancellationToken cancellationToken = default)
    {
        var codes = await GetCatalogAsync(cancellationToken).ConfigureAwait(false);
        return codes.Where(c => c.Level == 4).Select(c => c.Code).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(c => c).ToList();
    }

    public async Task<List<(string Code, string? Description)>> GetLevel4CodesWithDescriptionAsync(string? level1Filter = null, string? level2Filter = null, string? level3Filter = null, CancellationToken cancellationToken = default)
    {
        var codes = await GetCatalogAsync(cancellationToken).ConfigureAwait(false);
        return codes.Where(c => c.Level == 4)
            .OrderBy(c => c.Code)
            .Select(c => (c.Code, c.Description))
            .ToList();
    }

    public async Task DeleteCodeAsync(int level, string code, CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync(cancellationToken).ConfigureAwait(false);

        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

        var checkCmd = conn.CreateCommand();
        checkCmd.CommandText = level switch
        {
            1 => "SELECT COUNT(*) FROM Documents WHERE Level1 = $code",
            2 => "SELECT COUNT(*) FROM Documents WHERE Level2 = $code",
            3 => "SELECT COUNT(*) FROM Documents WHERE Level3 = $code",
            4 => "SELECT COUNT(*) FROM Documents WHERE Level4 = $code",
            _ => throw new ArgumentException($"Invalid level: {level}")
        };
        checkCmd.Parameters.AddWithValue("$code", code);
        var documentCount = Convert.ToInt32(await checkCmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false));
        if (documentCount > 0)
        {
            throw new InvalidOperationException($"Cannot delete Level {level} code '{code}' because there are {documentCount} document(s) that reference it.");
        }

        await codeStore.DeleteAsync(level, code, cancellationToken).ConfigureAwait(false);

        var deleteCmd = conn.CreateCommand();
        deleteCmd.CommandText = level switch
        {
            1 => "DELETE FROM CodeSeries WHERE Level1 = $code AND Level2 = '' AND Level3 = '' AND Level4 IS NULL",
            2 => "DELETE FROM CodeSeries WHERE Level2 = $code AND Level1 = '' AND Level3 = '' AND Level4 IS NULL",
            3 => "DELETE FROM CodeSeries WHERE Level3 = $code AND Level1 = '' AND Level2 = '' AND Level4 IS NULL",
            4 => "DELETE FROM CodeSeries WHERE Level4 = $code AND Level1 = '' AND Level2 = '' AND Level3 = ''",
            _ => throw new ArgumentException($"Invalid level: {level}")
        };
        deleteCmd.Parameters.AddWithValue("$code", code);
        await deleteCmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task PurgeAllCodesAsync(CancellationToken cancellationToken = default)
    {
        await EnsureInitializedAsync(cancellationToken).ConfigureAwait(false);

        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

        var checkCmd = conn.CreateCommand();
        checkCmd.CommandText = "SELECT COUNT(*) FROM Documents";
        var documentCount = Convert.ToInt32(await checkCmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false));

        if (documentCount > 0)
        {
            throw new InvalidOperationException($"Cannot purge codes because there are {documentCount} document(s) that reference them. Please delete all documents first before purging codes.");
        }

        await codeStore.ClearAsync(cancellationToken).ConfigureAwait(false);

        var deleteCmd = conn.CreateCommand();
        deleteCmd.CommandText = "DELETE FROM CodeSeries WHERE (Level1 != '' AND Level2 = '' AND Level3 = '' AND Level4 IS NULL) OR (Level1 = '' AND Level2 != '' AND Level3 = '' AND Level4 IS NULL) OR (Level1 = '' AND Level2 = '' AND Level3 != '' AND Level4 IS NULL) OR (Level1 = '' AND Level2 = '' AND Level3 = '' AND Level4 IS NOT NULL AND Level4 != '')";
        await deleteCmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task<IReadOnlyList<CodeCatalogEntry>> GetCatalogAsync(CancellationToken cancellationToken)
    {
        await EnsureInitializedAsync(cancellationToken).ConfigureAwait(false);
        return await codeStore.LoadAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task EnsureInitializedAsync(CancellationToken cancellationToken)
    {
        if (initialized) return;
        initialized = true;

        // If JSON already has content, nothing to do.
        var existing = await codeStore.LoadAsync(cancellationToken).ConfigureAwait(false);
        if (existing.Count > 0) return;

        // Attempt to migrate single-level codes from SQLite (if any)
        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Level1, Level2, Level3, Level4, Description FROM CodeSeries";
        var entries = new List<CodeCatalogEntry>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            var l1 = reader.GetString(0);
            var l2 = reader.GetString(1);
            var l3 = reader.GetString(2);
            var l4 = reader.IsDBNull(3) ? null : reader.GetString(3);
            var desc = reader.IsDBNull(4) ? null : reader.GetString(4);

            if (!string.IsNullOrWhiteSpace(l1) && string.IsNullOrEmpty(l2) && string.IsNullOrEmpty(l3) && l4 is null)
            {
                entries.Add(new CodeCatalogEntry(1, l1, desc));
            }
            else if (string.IsNullOrEmpty(l1) && !string.IsNullOrWhiteSpace(l2) && string.IsNullOrEmpty(l3) && l4 is null)
            {
                entries.Add(new CodeCatalogEntry(2, l2, desc));
            }
            else if (string.IsNullOrEmpty(l1) && string.IsNullOrEmpty(l2) && !string.IsNullOrWhiteSpace(l3) && l4 is null)
            {
                entries.Add(new CodeCatalogEntry(3, l3, desc));
            }
            else if (string.IsNullOrEmpty(l1) && string.IsNullOrEmpty(l2) && string.IsNullOrEmpty(l3) && !string.IsNullOrWhiteSpace(l4))
            {
                entries.Add(new CodeCatalogEntry(4, l4, desc));
            }
        }

        if (entries.Count > 0)
        {
            await codeStore.ReplaceAsync(entries, cancellationToken).ConfigureAwait(false);
        }
    }

    private static bool TryGetSingleLevel(CodeSeriesKey key, out int level, out string code)
    {
        var isLevel1 = !string.IsNullOrWhiteSpace(key.Level1) && string.IsNullOrWhiteSpace(key.Level2) && string.IsNullOrWhiteSpace(key.Level3) && key.Level4 is null;
        var isLevel2 = string.IsNullOrWhiteSpace(key.Level1) && !string.IsNullOrWhiteSpace(key.Level2) && string.IsNullOrWhiteSpace(key.Level3) && key.Level4 is null;
        var isLevel3 = string.IsNullOrWhiteSpace(key.Level1) && string.IsNullOrWhiteSpace(key.Level2) && !string.IsNullOrWhiteSpace(key.Level3) && key.Level4 is null;
        var isLevel4 = string.IsNullOrWhiteSpace(key.Level1) && string.IsNullOrWhiteSpace(key.Level2) && string.IsNullOrWhiteSpace(key.Level3) && !string.IsNullOrWhiteSpace(key.Level4);

        if (isLevel1)
        {
            level = 1;
            code = key.Level1;
            return true;
        }
        if (isLevel2)
        {
            level = 2;
            code = key.Level2;
            return true;
        }
        if (isLevel3)
        {
            level = 3;
            code = key.Level3;
            return true;
        }
        if (isLevel4)
        {
            level = 4;
            code = key.Level4!;
            return true;
        }

        level = 0;
        code = string.Empty;
        return false;
    }
}
