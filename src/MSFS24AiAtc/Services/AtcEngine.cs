using MSFS24AiAtc.Core;

namespace MSFS24AiAtc.Services;

public sealed class AtcEngine
{
    private readonly AiProviderManager ai;
    public event Action<AtcMessage>? Transmission;

    public AtcEngine(AiProviderManager ai) => this.ai = ai;

    public async Task ProcessPilotTransmissionAsync(
        ControllerType controller,
        string callsign,
        string pilotText,
        AirportInfo airport,
        SimState state,
        CancellationToken ct = default)
    {
        var prompt = ai.BuildSystemPrompt(controller, airport) + "\n\n" +
            "LIVE STATE\n" +
            $"Callsign: {callsign}\nAircraft: {state.AircraftTitle}\n" +
            $"Position: {state.Latitude:F5}, {state.Longitude:F5}\n" +
            $"Altitude: {state.AltitudeFeet:F0} ft\nSpeed: {state.GroundSpeedKnots:F0} kt\n" +
            $"COM1: {state.Com1}\nCOM2: {state.Com2}\n\n" +
            "PILOT TRANSMISSION\n" + pilotText;

        var response = await ai.GenerateAsync(prompt, controller, ct);
        if (string.IsNullOrWhiteSpace(response)) return;

        Transmission?.Invoke(new AtcMessage(controller, callsign, response.Trim(), DateTimeOffset.UtcNow));
    }
}
