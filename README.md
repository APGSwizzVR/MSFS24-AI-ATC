# MSFS24 AI ATC

A Windows desktop AI ATC client for Microsoft Flight Simulator 2024.

## Planned capabilities
- MSFS 2024 SimConnect connection.
- Launch the ATC client before MSFS and automatically detect the simulator.
- Airport/frequency/controller management.
- Ground, clearance, tower, departure, approach and center controller profiles.
- Per-controller AI model selection.
- Real-time speech provider abstraction.
- Account creation/login with local SQLite storage in the MVP.
- Aviation-context moderation with escalating restrictions.
- Online airport/aviation data with MSFS telemetry as the authoritative live-flight source.
- Windows installer/build scripts.

## Important
This repository does not redistribute Microsoft/Asobo SimConnect binaries. Microsoft documents SimConnect as the supported API for external MSFS 2024 applications and recommends out-of-process applications for stability. Copy the matching MSFS 2024 SDK files into `lib/SimConnect/` before building.

Required files:
- `Microsoft.FlightSimulator.SimConnect.dll`
- `SimConnect.dll`

Source: MSFS 2024 SDK -> SimConnect SDK -> lib/managed and lib.

## Build
1. Install the .NET 8 SDK.
2. Install the MSFS 2024 SDK.
3. Run `scripts/prepare-simconnect.ps1`.
4. Run `dotnet restore`.
5. Run `dotnet build src/MSFS24AiAtc/MSFS24AiAtc.csproj -c Release`.
6. Run the resulting x64 executable.

## Installer
Install Inno Setup 6, build Release, then run:
`iscc installer/MSFS24AiAtc.iss`

The installer checks that the application has the required SimConnect runtime files.

## Architecture
The application is intentionally split into:
- UI
- account service
- SimConnect bridge
- airport/frequency service
- ATC state engine
- AI provider abstraction
- speech provider abstraction
- moderation service

This keeps the simulator connection independent from the AI provider.

## AI providers
The settings screen contains provider/model selection. OpenAI Realtime is the intended high-quality real-time voice path, while the provider interface allows additional providers to be added without rewriting the ATC engine.

Never commit API keys. Use environment variables or the app's encrypted local credential store in a production implementation.

## Scope
This is an original implementation. It does not copy BeyondATC, SayIntentions, or their proprietary code/assets.
