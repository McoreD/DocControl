using Microsoft.Data.Sqlite;

namespace DocControl.Infrastructure.Data;

public sealed class DatabaseInitializer
{
    private readonly DbConnectionFactory factory;

    public DatabaseInitializer(DbConnectionFactory factory)
    {
        this.factory = factory;
    }

    public async Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
    {
        await using var conn = factory.Create();
        await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
        PRAGMA foreign_keys = ON;

        CREATE TABLE IF NOT EXISTS Config (
            Key TEXT PRIMARY KEY,
            Value TEXT NOT NULL
        );

        CREATE TABLE IF NOT EXISTS CodeSeries (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Level1 TEXT NOT NULL,
            Level2 TEXT NOT NULL,
            Level3 TEXT NOT NULL,
            Level4 TEXT,
            Description TEXT,
            NextNumber INTEGER NOT NULL DEFAULT 1,
            UNIQUE(Level1, Level2, Level3, Level4)
        );

        CREATE TABLE IF NOT EXISTS Documents (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Level1 TEXT NOT NULL,
            Level2 TEXT NOT NULL,
            Level3 TEXT NOT NULL,
            Level4 TEXT,
            Number INTEGER NOT NULL,
            FreeText TEXT,
            FileName TEXT NOT NULL,
            CreatedBy TEXT NOT NULL,
            CreatedAt TEXT NOT NULL,
            OriginalQuery TEXT,
            CodeSeriesId INTEGER NOT NULL,
            FOREIGN KEY(CodeSeriesId) REFERENCES CodeSeries(Id),
            UNIQUE(Level1, Level2, Level3, Level4, Number)
        );

        CREATE TABLE IF NOT EXISTS Audit (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Action TEXT NOT NULL,
            Payload TEXT,
            CreatedBy TEXT NOT NULL,
            CreatedAt TEXT NOT NULL,
            DocumentId INTEGER,
            FOREIGN KEY(DocumentId) REFERENCES Documents(Id)
        );
        ";

        await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        
        // Run migrations
        await MigrateAsync(conn, cancellationToken).ConfigureAwait(false);
    }

    private async Task MigrateAsync(SqliteConnection conn, CancellationToken cancellationToken)
    {
        // Check if Description column exists in CodeSeries table
        var checkCmd = conn.CreateCommand();
        checkCmd.CommandText = "PRAGMA table_info(CodeSeries)";
        
        bool hasDescriptionColumn = false;
        await using (var reader = await checkCmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
        {
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                var columnName = reader.GetString(1); // Column name is at index 1
                if (columnName == "Description")
                {
                    hasDescriptionColumn = true;
                    break;
                }
            }
        }

        // Add Description column if it doesn't exist
        if (!hasDescriptionColumn)
        {
            var alterCmd = conn.CreateCommand();
            alterCmd.CommandText = "ALTER TABLE CodeSeries ADD COLUMN Description TEXT";
            await alterCmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
