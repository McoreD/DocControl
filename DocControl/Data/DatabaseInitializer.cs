using Microsoft.Data.Sqlite;

namespace DocControl.Data;

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
    }
}
