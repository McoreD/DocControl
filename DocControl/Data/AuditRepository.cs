using DocControl.Models;
using Microsoft.Data.Sqlite;

namespace DocControl.Data;

public sealed class AuditRepository
{
    private readonly DbConnectionFactory factory;

    public AuditRepository(DbConnectionFactory factory)
    {
        this.factory = factory;
    }

    public async Task InsertAsync(string action, string payload, string createdBy, DateTime createdAtUtc, int? documentId = null, CancellationToken cancellationToken = default)
    {
        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Audit (Action, Payload, CreatedBy, CreatedAt, DocumentId)
            VALUES ($action, $payload, $by, $at, $docId);
        ";
        cmd.Parameters.AddWithValue("$action", action);
        cmd.Parameters.AddWithValue("$payload", (object?)payload ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$by", createdBy);
        cmd.Parameters.AddWithValue("$at", createdAtUtc.ToString("O"));
        cmd.Parameters.AddWithValue("$docId", (object?)documentId ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<AuditEntry>> GetRecentAsync(int take = 50, CancellationToken cancellationToken = default)
    {
        var list = new List<AuditEntry>();
        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT Id, Action, Payload, CreatedBy, CreatedAt, DocumentId FROM Audit ORDER BY Id DESC LIMIT $take;";
        cmd.Parameters.AddWithValue("$take", take);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            list.Add(new AuditEntry
            {
                Id = reader.GetInt32(0),
                Action = reader.GetString(1),
                Payload = reader.IsDBNull(2) ? null : reader.GetString(2),
                CreatedBy = reader.GetString(3),
                CreatedAtUtc = DateTime.Parse(reader.GetString(4)),
                DocumentId = reader.IsDBNull(5) ? null : reader.GetInt32(5)
            });
        }
        return list;
    }

    public async Task<IReadOnlyList<AuditEntry>> GetRecentAsync(int take = 50, string? containsAction = null, string? containsUser = null, CancellationToken cancellationToken = default)
    {
        var list = new List<AuditEntry>();
        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT Id, Action, Payload, CreatedBy, CreatedAt, DocumentId FROM Audit
                             WHERE (@action IS NULL OR Action LIKE '%' || @action || '%')
                               AND (@user IS NULL OR CreatedBy LIKE '%' || @user || '%')
                             ORDER BY Id DESC LIMIT @take;";
        cmd.Parameters.AddWithValue("@take", take);
        cmd.Parameters.AddWithValue("@action", (object?)containsAction ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@user", (object?)containsUser ?? DBNull.Value);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            list.Add(new AuditEntry
            {
                Id = reader.GetInt32(0),
                Action = reader.GetString(1),
                Payload = reader.IsDBNull(2) ? null : reader.GetString(2),
                CreatedBy = reader.GetString(3),
                CreatedAtUtc = DateTime.Parse(reader.GetString(4)),
                DocumentId = reader.IsDBNull(5) ? null : reader.GetInt32(5)
            });
        }
        return list;
    }

    public async Task<IReadOnlyList<AuditEntry>> GetPagedAsync(int take = 50, int skip = 0, string? containsAction = null, string? containsUser = null, CancellationToken cancellationToken = default)
    {
        var list = new List<AuditEntry>();
        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT Id, Action, Payload, CreatedBy, CreatedAt, DocumentId FROM Audit
                             WHERE (@action IS NULL OR Action LIKE '%' || @action || '%')
                               AND (@user IS NULL OR CreatedBy LIKE '%' || @user || '%')
                             ORDER BY Id DESC LIMIT @take OFFSET @skip;";
        cmd.Parameters.AddWithValue("@take", take);
        cmd.Parameters.AddWithValue("@skip", skip);
        cmd.Parameters.AddWithValue("@action", (object?)containsAction ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@user", (object?)containsUser ?? DBNull.Value);

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            list.Add(new AuditEntry
            {
                Id = reader.GetInt32(0),
                Action = reader.GetString(1),
                Payload = reader.IsDBNull(2) ? null : reader.GetString(2),
                CreatedBy = reader.GetString(3),
                CreatedAtUtc = DateTime.Parse(reader.GetString(4)),
                DocumentId = reader.IsDBNull(5) ? null : reader.GetInt32(5)
            });
        }
        return list;
    }

    public async Task<int> GetCountAsync(string? containsAction = null, string? containsUser = null, CancellationToken cancellationToken = default)
    {
        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT COUNT(1) FROM Audit WHERE (@action IS NULL OR Action LIKE '%' || @action || '%') AND (@user IS NULL OR CreatedBy LIKE '%' || @user || '%');";
        cmd.Parameters.AddWithValue("@action", (object?)containsAction ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@user", (object?)containsUser ?? DBNull.Value);
        var result = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return Convert.ToInt32(result);
    }
}
