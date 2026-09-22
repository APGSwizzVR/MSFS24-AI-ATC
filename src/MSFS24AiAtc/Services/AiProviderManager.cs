using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MSFS24AiAtc.Core;

namespace MSFS24AiAtc.Services;

public sealed class AiProviderManager
{
    private readonly SecureSettings settings;
    private readonly HttpClient http = new() { Timeout = TimeSpan.FromSeconds(30) };

    public IReadOnlyList<AiModel> Models { get; } =
    [
        new("OpenAI", "gpt-live-1"),
        new("OpenAI", "gpt-live-1-mini"),
        new("OpenAI", "gpt-5.6"),
        new("Custom", "OpenAI-compatible endpoint")
    ];

    public AiProviderManager(SecureSettings settings) => this.settings = settings;

    public AiModel GetModel(ControllerType type)
    {
        var value = settings.Get("model." + type);
        return Models.FirstOrDefault(x => $"{x.Provider}/{x.Model}".Equals(value, StringComparison.OrdinalIgnoreCase))
            ?? new AiModel("OpenAI", "gpt-live-1");
    }

    public void SetModel(ControllerType type, AiModel model) =>
        settings.Set("model." + type, $"{model.Provider}/{model.Model}");

    public string BuildSystemPrompt(ControllerType type, AirportInfo airport) =>
        "You are a professional " + type + " air traffic controller at " + airport.Icao + ", " + airport.Name + ". " +
        "Use concise realistic aviation radio phraseology. Never invent runway, frequency, procedure, weather, traffic, aircraft or clearance data. " +
        "Live simulator and facility data are authoritative. Do not discuss unrelated subjects. " +
        "Return only the controller transmission.";

    public async Task<string> GenerateAsync(string prompt, ControllerType type, CancellationToken ct)
    {
        var model = GetModel(type);
        var key = settings.Get("api.openai");

        if (model.Provider == "OpenAI" && string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("OpenAI API key is not configured. Open Settings > API & AI.");

        if (model.Provider == "Custom")
        {
            var endpoint = settings.Get("api.custom.endpoint");
            var customKey = settings.Get("api.custom.key");
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new InvalidOperationException("Custom AI endpoint is not configured.");
            return await CallCompatibleAsync(endpoint, customKey, model.Model, prompt, ct);
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", key);
        request.Content = new StringContent(JsonSerializer.Serialize(new
        {
            model = model.Model,
            input = prompt,
            max_output_tokens = 180
        }), Encoding.UTF8, "application/json");

        using var response = await http.SendAsync(request, ct);
        var json = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException("OpenAI request failed: " + json);

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.TryGetProperty("output_text", out var text)
            ? text.GetString() ?? ""
            : "";
    }

    private async Task<string> CallCompatibleAsync(string endpoint, string key, string model, string prompt, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        if (!string.IsNullOrWhiteSpace(key))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", key);

        request.Content = new StringContent(JsonSerializer.Serialize(new
        {
            model,
            messages = new[] { new { role = "user", content = prompt } }
        }), Encoding.UTF8, "application/json");

        using var response = await http.SendAsync(request, ct);
        var json = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException(json);

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "";
    }
}
