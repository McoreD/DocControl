using Microsoft.Data.Sqlite;

namespace DocControl.Infrastructure.Data;

public sealed class DbConnectionFactory
{
    private readonly string connectionString;

    public DbConnectionFactory(string databasePath)
    {
        if (string.IsNullOrWhiteSpace(databasePath)) throw new ArgumentException("Database path required", nameof(databasePath));
        connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared
        }.ToString();
    }

    public SqliteConnection Create() => new(connectionString);
}
