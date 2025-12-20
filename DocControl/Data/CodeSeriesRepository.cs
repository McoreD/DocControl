using DocControl.Models;
using Microsoft.Data.Sqlite;

namespace DocControl.Data;

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
}
