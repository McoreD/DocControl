using DocControl.Core.Models;
using Microsoft.Data.Sqlite;

namespace DocControl.Infrastructure.Data;

public sealed class CodeSeriesRepository
{
    private readonly DbConnectionFactory factory;

    public CodeSeriesRepository(DbConnectionFactory factory)
    {
        this.factory = factory;
    }

    public async Task SeedNextNumbersAsync(IEnumerable<(CodeSeriesKey key, int maxNumber)> seriesMax, CancellationToken cancellationToken = default)
    {
        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var tx = (SqliteTransaction)await conn.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        foreach (var (key, max) in seriesMax)
        {
            var upsert = conn.CreateCommand();
            upsert.Transaction = tx;
            upsert.CommandText = @"
                INSERT INTO CodeSeries (Level1, Level2, Level3, Level4, NextNumber)
                VALUES ($l1, $l2, $l3, $l4, $next)
                ON CONFLICT(Level1, Level2, Level3, Level4)
                DO UPDATE SET NextNumber = CASE WHEN excluded.NextNumber > CodeSeries.NextNumber THEN excluded.NextNumber ELSE CodeSeries.NextNumber END;
            ";
            upsert.Parameters.AddWithValue("$l1", key.Level1);
            upsert.Parameters.AddWithValue("$l2", key.Level2);
            upsert.Parameters.AddWithValue("$l3", key.Level3);
            upsert.Parameters.AddWithValue("$l4", (object?)key.Level4 ?? DBNull.Value);
            upsert.Parameters.AddWithValue("$next", max + 1);
            await upsert.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        await tx.CommitAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<string>> GetLevel1CodesAsync(CancellationToken cancellationToken = default)
    {
        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT DISTINCT Level1 FROM CodeSeries WHERE Level1 != '' ORDER BY Level1";

        var codes = new List<string>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            codes.Add(reader.GetString(0));
        }

        return codes;
    }

    public async Task<List<string>> GetLevel2CodesAsync(string? level1Filter = null, CancellationToken cancellationToken = default)
    {
        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

        var cmd = conn.CreateCommand();
        if (string.IsNullOrWhiteSpace(level1Filter))
        {
            cmd.CommandText = "SELECT DISTINCT Level2 FROM CodeSeries WHERE Level2 != '' ORDER BY Level2";
        }
        else
        {
            cmd.CommandText = "SELECT DISTINCT Level2 FROM CodeSeries WHERE Level1 = $l1 AND Level2 != '' ORDER BY Level2";
            cmd.Parameters.AddWithValue("$l1", level1Filter);
        }

        var codes = new List<string>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            codes.Add(reader.GetString(0));
        }

        return codes;
    }

    public async Task<List<string>> GetLevel3CodesAsync(string? level1Filter = null, string? level2Filter = null, CancellationToken cancellationToken = default)
    {
        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

        var cmd = conn.CreateCommand();
        var whereConditions = new List<string> { "Level3 != ''" };
        
        if (!string.IsNullOrWhiteSpace(level1Filter))
        {
            whereConditions.Add("Level1 = $l1");
            cmd.Parameters.AddWithValue("$l1", level1Filter);
        }
        
        if (!string.IsNullOrWhiteSpace(level2Filter))
        {
            whereConditions.Add("Level2 = $l2");
            cmd.Parameters.AddWithValue("$l2", level2Filter);
        }

        cmd.CommandText = $"SELECT DISTINCT Level3 FROM CodeSeries WHERE {string.Join(" AND ", whereConditions)} ORDER BY Level3";

        var codes = new List<string>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            codes.Add(reader.GetString(0));
        }

        return codes;
    }

    public async Task<List<string>> GetLevel4CodesAsync(string? level1Filter = null, string? level2Filter = null, string? level3Filter = null, CancellationToken cancellationToken = default)
    {
        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

        var cmd = conn.CreateCommand();
        var whereConditions = new List<string> { "Level4 IS NOT NULL AND Level4 != ''" };
        
        if (!string.IsNullOrWhiteSpace(level1Filter))
        {
            whereConditions.Add("Level1 = $l1");
            cmd.Parameters.AddWithValue("$l1", level1Filter);
        }
        
        if (!string.IsNullOrWhiteSpace(level2Filter))
        {
            whereConditions.Add("Level2 = $l2");
            cmd.Parameters.AddWithValue("$l2", level2Filter);
        }
        
        if (!string.IsNullOrWhiteSpace(level3Filter))
        {
            whereConditions.Add("Level3 = $l3");
            cmd.Parameters.AddWithValue("$l3", level3Filter);
        }

        cmd.CommandText = $"SELECT DISTINCT Level4 FROM CodeSeries WHERE {string.Join(" AND ", whereConditions)} ORDER BY Level4";

        var codes = new List<string>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            var value = reader.GetValue(0);
            if (value != DBNull.Value)
            {
                codes.Add(reader.GetString(0));
            }
        }

        return codes;
    }
}
