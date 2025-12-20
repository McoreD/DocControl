namespace DocControl.Data;

public sealed class ConfigRepository
{
    private readonly DbConnectionFactory factory;

    public ConfigRepository(DbConnectionFactory factory)
    {
        this.factory = factory;
    }

    public async Task SetAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Config (Key, Value) VALUES ($k, $v)
            ON CONFLICT(Key) DO UPDATE SET Value = excluded.Value;
        ";
        cmd.Parameters.AddWithValue("$k", key);
        cmd.Parameters.AddWithValue("$v", value);
        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<string?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Value FROM Config WHERE Key = $k";
        cmd.Parameters.AddWithValue("$k", key);

        var result = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return result?.ToString();
    }
}
