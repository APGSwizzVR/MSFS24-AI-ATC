using MSFS24AiAtc.Core;

namespace MSFS24AiAtc.Services;

public sealed class AiProviderManager
{
    public IReadOnlyList<AiModel> Models { get; } =
    [
        new("OpenAI", "gpt-live-1"),
        new("OpenAI", "gpt-live-1-mini"),
        new("OpenAI", "gpt-5.6"),
        new("Local", "Custom endpoint")
    ];

    public AiModel Clearance { get; set; } = new("OpenAI", "gpt-live-1");
    public AiModel Ground { get; set; } = new("OpenAI", "gpt-live-1");
    public AiModel Tower { get; set; } = new("OpenAI", "gpt-live-1");
    public AiModel Approach { get; set; } = new("OpenAI", "gpt-live-1");
    public AiModel Center { get; set; } = new("OpenAI", "gpt-live-1");

    public string BuildSystemPrompt(ControllerType type, AirportInfo airport)
    {
        return $"""
        You are the {type} controller at {airport.Icao}, {airport.Name}.
        Act as realistic aviation radio ATC. Use concise standard phraseology.
        Never invent runway, frequency, procedure, aircraft, weather, traffic, or clearance data.
        Use the supplied simulator state and airport data as authoritative.
        Do not discuss unrelated topics. If the pilot asks an unrelated question,
        briefly redirect them to aviation.
        """;
    }
}
