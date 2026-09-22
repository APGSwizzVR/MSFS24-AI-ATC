using MSFS24AiAtc.Data;

namespace MSFS24AiAtc.Services;

public sealed class ModerationService
{
    private readonly AppDatabase _db;
    private static readonly string[] AviationTerms =
    [
        "atc","airport","aircraft","flight","pilot","runway","taxi","takeoff","take-off",
        "landing","approach","departure","clearance","frequency","radio","aviation",
        "airspace","altitude","heading","squawk","ils","vfr","ifr","traffic","gate"
    ];

    public ModerationService(AppDatabase db) => _db = db;

    public bool IsAviationRelevant(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return true;
        var normalized = text.ToLowerInvariant();
        return AviationTerms.Any(normalized.Contains);
    }

    public void RegisterOffTopic(string username) => _db.AddWarning(username);
}
