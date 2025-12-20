using DocControl.Models;
using Microsoft.Data.Sqlite;

namespace DocControl.Data;

public sealed class NumberAllocator
{
    private readonly DbConnectionFactory factory;

    public NumberAllocator(DbConnectionFactory factory)
    {
        this.factory = factory;
    }

    public async Task<AllocatedNumber> AllocateAsync(CodeSeriesKey key, CancellationToken cancellationToken = default)
    {
        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

        await using var tx = (SqliteTransaction)await conn.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        // Ensure series exists
        var ensureCmd = conn.CreateCommand();
        ensureCmd.Transaction = tx;
        ensureCmd.CommandText = @"
            INSERT INTO CodeSeries (Level1, Level2, Level3, Level4, NextNumber)
            VALUES ($l1, $l2, $l3, $l4, 1)
            ON CONFLICT(Level1, Level2, Level3, Level4) DO NOTHING;
        ";
        ensureCmd.Parameters.AddWithValue("$l1", key.Level1);
        ensureCmd.Parameters.AddWithValue("$l2", key.Level2);
        ensureCmd.Parameters.AddWithValue("$l3", key.Level3);
        ensureCmd.Parameters.AddWithValue("$l4", (object?)key.Level4 ?? DBNull.Value);
        await ensureCmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

        // Select current next number and id
        var selectCmd = conn.CreateCommand();
        selectCmd.Transaction = tx;
        selectCmd.CommandText = @"
            SELECT Id, NextNumber FROM CodeSeries
            WHERE Level1 = $l1 AND Level2 = $l2 AND Level3 = $l3 AND Level4 IS $l4OrNull AND ($l4OrNull IS NOT NULL OR Level4 IS NULL);
        ";
        selectCmd.Parameters.AddWithValue("$l1", key.Level1);
        selectCmd.Parameters.AddWithValue("$l2", key.Level2);
        selectCmd.Parameters.AddWithValue("$l3", key.Level3);
        selectCmd.Parameters.AddWithValue("$l4OrNull", (object?)key.Level4 ?? DBNull.Value);

        int seriesId;
        int nextNumber;
        await using (var reader = await selectCmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
        {
            if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                throw new InvalidOperationException("Failed to load code series after insert.");
            }
            seriesId = reader.GetInt32(0);
            nextNumber = reader.GetInt32(1);
        }

        var updateCmd = conn.CreateCommand();
        updateCmd.Transaction = tx;
        updateCmd.CommandText = @"
            UPDATE CodeSeries SET NextNumber = NextNumber + 1 WHERE Id = $id;
        ";
        updateCmd.Parameters.AddWithValue("$id", seriesId);
        await updateCmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

        await tx.CommitAsync(cancellationToken).ConfigureAwait(false);

        return new AllocatedNumber(seriesId, nextNumber);
    }
}
