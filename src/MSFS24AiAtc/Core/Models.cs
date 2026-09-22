namespace MSFS24AiAtc.Core;

public enum ControllerType
{
    Clearance,
    Ground,
    Tower,
    Departure,
    Approach,
    Center,
    Information
}

public sealed record UserAccount(Guid Id, string Username, int OffTopicWarnings, bool Suspended);

public sealed record Frequency(string Name, double Mhz, ControllerType Type);

public sealed record AirportInfo(string Icao, string Name, double Latitude, double Longitude, IReadOnlyList<Frequency> Frequencies);

public sealed record SimState(
    bool Connected,
    string AircraftTitle,
    double Latitude,
    double Longitude,
    double AltitudeFeet,
    double GroundSpeedKnots,
    string Com1,
    string Com2);

public sealed record AtcMessage(
    ControllerType Controller,
    string Callsign,
    string Text,
    DateTimeOffset Timestamp);

public sealed record AiModel(string Provider, string Model);
