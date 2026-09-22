using System.Security.Cryptography;
using System.Text;
using MSFS24AiAtc.Data;
using MSFS24AiAtc.Core;

namespace MSFS24AiAtc.Services;

public sealed class AccountService
{
    private readonly AppDatabase _db;
    public AccountService(AppDatabase db) => _db = db;

    public UserAccount Create(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || username.Length < 3) throw new ArgumentException("Username must contain at least 3 characters.");
        if (password.Length < 8) throw new ArgumentException("Password must contain at least 8 characters.");

        var id = Guid.NewGuid();
        _db.CreateUser(id, username.Trim(), Hash(password));
        return new UserAccount(id, username.Trim(), 0, false);
    }

    public UserAccount Login(string username, string password)
    {
        var user = _db.GetUser(username.Trim()) ?? throw new UnauthorizedAccessException("Account not found.");
        if (!CryptographicOperations.FixedTimeEquals(Convert.FromHexString(user.Value.Hash), Convert.FromHexString(Hash(password))))
            throw new UnauthorizedAccessException("Incorrect password.");
        return new UserAccount(user.Value.Id, username.Trim(), user.Value.Warnings, user.Value.Suspended);
    }

    private static string Hash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }
}
