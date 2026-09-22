using System.Security.Cryptography;
using System.Text.Json;

namespace MSFS24AiAtc.Services;

public sealed class SecureSettings
{
    private readonly string _path;
    private readonly object _gate = new();
    private Dictionary<string, string> _values = new(StringComparer.OrdinalIgnoreCase);

    public SecureSettings()
    {
        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MSFS24-AI-ATC");
        Directory.CreateDirectory(dir);
        _path = Path.Combine(dir, "settings.dat");
        Load();
    }

    public string Get(string key, string fallback = "")
    {
        lock (_gate) return _values.TryGetValue(key, out var value) ? value : fallback;
    }

    public void Set(string key, string value)
    {
        lock (_gate) { _values[key] = value; Save(); }
    }

    private void Load()
    {
        lock (_gate)
        {
            if (!File.Exists(_path)) return;
            try
            {
                var encrypted = File.ReadAllBytes(_path);
                var plain = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
                _values = JsonSerializer.Deserialize<Dictionary<string, string>>(plain) ??
                    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
            catch { _values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); }
        }
    }

    private void Save()
    {
        var plain = JsonSerializer.SerializeToUtf8Bytes(_values);
        var encrypted = ProtectedData.Protect(plain, null, DataProtectionScope.CurrentUser);
        File.WriteAllBytes(_path, encrypted);
    }
}
