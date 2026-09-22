using Microsoft.Data.Sqlite;

namespace MSFS24AiAtc.Data;

public sealed class AppDatabase
{
    private readonly string _path;
    public AppDatabase(string path) => _path = path;

    private SqliteConnection Open()
    {
        var c = new SqliteConnection($"Data Source={_path}");
        c.Open();
        return c;
    }

    public void Initialize()
    {
        using var c = Open();
        using var cmd = c.CreateCommand();
        cmd.CommandText = """
        CREATE TABLE IF NOT EXISTS Users (
            Id TEXT PRIMARY KEY,
            Username TEXT NOT NULL UNIQUE,
            PasswordHash TEXT NOT NULL,
            OffTopicWarnings INTEGER NOT NULL DEFAULT 0,
            Suspended INTEGER NOT NULL DEFAULT 0
        );
        """;
        cmd.ExecuteNonQuery();
    }

    public void CreateUser(Guid id, string username, string passwordHash)
    {
        using var c = Open();
        using var cmd = c.CreateCommand();
        cmd.CommandText = "INSERT INTO Users(Id, Username, PasswordHash) VALUES($id,$username,$hash)";
        cmd.Parameters.AddWithValue("$id", id.ToString());
        cmd.Parameters.AddWithValue("$username", username);
        cmd.Parameters.AddWithValue("$hash", passwordHash);
        cmd.ExecuteNonQuery();
    }

    public (Guid Id, string Hash, int Warnings, bool Suspended)? GetUser(string username)
    {
        using var c = Open();
        using var cmd = c.CreateCommand();
        cmd.CommandText = "SELECT Id,PasswordHash,OffTopicWarnings,Suspended FROM Users WHERE Username=$username";
        cmd.Parameters.AddWithValue("$username", username);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        return (Guid.Parse(r.GetString(0)), r.GetString(1), r.GetInt32(2), r.GetBoolean(3));
    }

    public void AddWarning(string username)
    {
        using var c = Open();
        using var cmd = c.CreateCommand();
        cmd.CommandText = """
        UPDATE Users
        SET OffTopicWarnings = OffTopicWarnings + 1,
            Suspended = CASE WHEN OffTopicWarnings + 1 >= 5 THEN 1 ELSE Suspended END
        WHERE Username=$username;
        """;
        cmd.Parameters.AddWithValue("$username", username);
        cmd.ExecuteNonQuery();
    }
}
