using DocControl.Core.Models;
using Microsoft.Data.Sqlite;

namespace DocControl.Infrastructure.Data;

public sealed class DocumentRepository
{
    private readonly DbConnectionFactory factory;

    public DocumentRepository(DbConnectionFactory factory)
    {
        this.factory = factory;
    }

    public async Task<int> InsertAsync(AllocatedNumber allocated, CodeSeriesKey key, string freeText, string fileName, string createdBy, DateTime createdAtUtc, string? originalQuery, CancellationToken cancellationToken = default)
    {
        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Documents (Level1, Level2, Level3, Level4, Number, FreeText, FileName, CreatedBy, CreatedAt, OriginalQuery, CodeSeriesId)
            VALUES ($l1, $l2, $l3, $l4, $num, $free, $file, $by, $at, $query, $series);
            SELECT last_insert_rowid();
        ";
        cmd.Parameters.AddWithValue("$l1", key.Level1);
        cmd.Parameters.AddWithValue("$l2", key.Level2);
        cmd.Parameters.AddWithValue("$l3", key.Level3);
        cmd.Parameters.AddWithValue("$l4", (object?)key.Level4 ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$num", allocated.Number);
        cmd.Parameters.AddWithValue("$free", (object?)freeText ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$file", fileName);
        cmd.Parameters.AddWithValue("$by", createdBy);
        cmd.Parameters.AddWithValue("$at", createdAtUtc.ToString("O"));
        cmd.Parameters.AddWithValue("$query", (object?)originalQuery ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$series", allocated.SeriesId);

        var result = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return Convert.ToInt32(result);
    }

    public async Task<int?> GetMaxNumberAsync(CodeSeriesKey key, CancellationToken cancellationToken = default)
    {
        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT MAX(Number) FROM Documents WHERE Level1 = $l1 AND Level2 = $l2 AND Level3 = $l3 AND Level4 IS $l4OrNull;";
        cmd.Parameters.AddWithValue("$l1", key.Level1);
        cmd.Parameters.AddWithValue("$l2", key.Level2);
        cmd.Parameters.AddWithValue("$l3", key.Level3);
        cmd.Parameters.AddWithValue("$l4OrNull", (object?)key.Level4 ?? DBNull.Value);
        var result = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        if (result is DBNull || result is null) return null;
        return Convert.ToInt32(result);
    }

    public async Task<bool> ExistsAsync(CodeSeriesKey key, CancellationToken cancellationToken = default)
    {
        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT 1 FROM Documents WHERE Level1=$l1 AND Level2=$l2 AND Level3=$l3 AND Level4 IS $l4OrNull LIMIT 1;";
        cmd.Parameters.AddWithValue("$l1", key.Level1);
        cmd.Parameters.AddWithValue("$l2", key.Level2);
        cmd.Parameters.AddWithValue("$l3", key.Level3);
        cmd.Parameters.AddWithValue("$l4OrNull", (object?)key.Level4 ?? DBNull.Value);
        var result = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return result != null;
    }

    public async Task<IReadOnlyList<DocumentRecord>> GetRecentAsync(int take = 50, CancellationToken cancellationToken = default)
    {
        var list = new List<DocumentRecord>();
        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT Id, Level1, Level2, Level3, Level4, Number, FileName, CreatedBy, CreatedAt FROM Documents ORDER BY Id DESC LIMIT $take;";
        cmd.Parameters.AddWithValue("$take", take);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            list.Add(new DocumentRecord
            {
                Id = reader.GetInt32(0),
                Level1 = reader.GetString(1),
                Level2 = reader.GetString(2),
                Level3 = reader.GetString(3),
                Level4 = reader.IsDBNull(4) ? null : reader.GetString(4),
                Number = reader.GetInt32(5),
                FileName = reader.GetString(6),
                CreatedBy = reader.GetString(7),
                CreatedAtUtc = DateTime.Parse(reader.GetString(8))
            });
        }
        return list;
    }

    public async Task<DocumentRecord?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT Id, Level1, Level2, Level3, Level4, Number, FileName, CreatedBy, CreatedAt FROM Documents WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$id", id);
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            return new DocumentRecord
            {
                Id = reader.GetInt32(0),
                Level1 = reader.GetString(1),
                Level2 = reader.GetString(2),
                Level3 = reader.GetString(3),
                Level4 = reader.IsDBNull(4) ? null : reader.GetString(4),
                Number = reader.GetInt32(5),
                FileName = reader.GetString(6),
                CreatedBy = reader.GetString(7),
                CreatedAtUtc = DateTime.Parse(reader.GetString(8))
            };
        }
        return null;
    }

    public async Task ClearAllAsync(CancellationToken cancellationToken = default)
    {
        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var tx = (SqliteTransaction)await conn.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        // Audit has a FK to Documents; remove Audit first.
        var delAudit = conn.CreateCommand();
        delAudit.Transaction = tx;
        delAudit.CommandText = "DELETE FROM Audit;";
        await delAudit.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

        var delDocs = conn.CreateCommand();
        delDocs.Transaction = tx;
        delDocs.CommandText = "DELETE FROM Documents;";
        await delDocs.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

        await tx.CommitAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task UpsertImportedAsync(CodeSeriesKey key, int number, string fileName, string createdBy, DateTime createdAtUtc, CancellationToken cancellationToken = default)
    {
        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Documents (Level1, Level2, Level3, Level4, Number, FreeText, FileName, CreatedBy, CreatedAt, OriginalQuery, CodeSeriesId)
            VALUES (
                $l1,
                $l2,
                $l3,
                $l4,
                $num,
                NULL,
                $file,
                $by,
                $at,
                NULL,
                (SELECT Id FROM CodeSeries WHERE Level1 = $l1 AND Level2 = $l2 AND Level3 = $l3 AND Level4 IS $l4OrNull LIMIT 1)
            )
            ON CONFLICT(Level1, Level2, Level3, Level4, Number)
            DO UPDATE SET
                FileName = excluded.FileName,
                CreatedBy = excluded.CreatedBy,
                CreatedAt = excluded.CreatedAt;
        ";

        cmd.Parameters.AddWithValue("$l1", key.Level1);
        cmd.Parameters.AddWithValue("$l2", key.Level2);
        cmd.Parameters.AddWithValue("$l3", key.Level3);
        cmd.Parameters.AddWithValue("$l4", (object?)key.Level4 ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$l4OrNull", (object?)key.Level4 ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$num", number);
        cmd.Parameters.AddWithValue("$file", fileName);
        cmd.Parameters.AddWithValue("$by", createdBy);
        cmd.Parameters.AddWithValue("$at", createdAtUtc.ToString("O"));

        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }
}
