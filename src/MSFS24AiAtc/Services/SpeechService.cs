namespace MSFS24AiAtc.Services;

public sealed class SpeechService
{
    public string Provider { get; set; } = "OpenAI Realtime";
    public string Voice { get; set; } = "Default";

    public Task SpeakAsync(string text, CancellationToken cancellationToken = default)
    {
        // Realtime voice transport is intentionally isolated here so the UI and ATC
        // state machine do not depend on a particular speech provider.
        return Task.CompletedTask;
    }
}
