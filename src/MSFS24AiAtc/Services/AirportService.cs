using System.Net.Http;
using System.Text.Json;
using MSFS24AiAtc.Core;

namespace MSFS24AiAtc.Services;

public sealed class AirportService
{
    private readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(10) };

    public async Task<AirportInfo?> FindNearestAsync(double latitude, double longitude, CancellationToken ct = default)
    {
        // OpenFlights/OurAirports-style public data can be plugged in here. The live
        // position itself comes from MSFS/SimConnect rather than a web service.
        await Task.CompletedTask;
        return null;
    }

    public AirportInfo FromSimConnect(string icao, string name, double lat, double lon)
        => new(icao, name, lat, lon, []);
}
