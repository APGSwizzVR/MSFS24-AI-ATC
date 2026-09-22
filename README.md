# MSFS24 AI ATC

Windows AI ATC for Microsoft Flight Simulator 2024.

The application is an out-of-process .NET executable. Microsoft documents SimConnect as the supported API for external MSFS 2024 applications and recommends out-of-process applications for stability.

Core pipeline:

MSFS 2024 -> SimConnect -> live flight/facility state -> ATC state/rules -> AI -> realtime speech -> VHF radio DSP -> audio output

## AI models

The settings architecture now separates realtime voice models from text reasoning models. The default realtime model is gpt-realtime, with gpt-realtime-mini as the lower-cost option. Text controllers can use GPT-5.6 or GPT-5.6 Luna.

OpenAI's current model catalogue lists GPT-Realtime models for realtime speech/audio workloads. The Realtime API supports speech-to-speech over low-latency transports and supports voices including marin and cedar. 

## API settings

Open Settings -> API & AI.

The OpenAI key, custom provider key and endpoint are entered there. Keys are encrypted locally with Windows DPAPI and are not written into source files.

For a public production app, do not distribute a permanent server API key inside the executable. OpenAI's documentation recommends keeping standard API keys server-side and using short-lived client credentials for client environments.

## Realism

The target is not a chatbot with an ATC skin. The controller engine must own operational decisions.

It should validate:

- callsign
- current frequency
- controller jurisdiction
- runway
- SID/STAR/approach
- altitude
- heading
- squawk
- traffic sequence
- wake category
- taxi route
- runway occupancy
- handoff state
- readback

The AI should turn an already-valid controller decision into natural radio language instead of inventing operational facts.

## Radio sound

Generated speech is passed through a dedicated VHF audio layer.

Current effects include:

- VHF bandwidth shaping
- dynamic compression
- mild nonlinear saturation
- controlled static bursts
- radio volume

The next realism layer is transmission squelch/opening/closing noise, which should be added as short non-verbal radio effects rather than permanently layered static.

## Global airport support

The engine is designed around facility data, not a hard-coded airport list. MSFS facility data is the live simulator source, while online aviation datasets can supplement metadata.

The target is worldwide airport coverage.

## Installer

The release workflow produces a self-contained Windows x64 application and an Inno Setup EXE installer.

Microsoft's official SimConnect SDK binaries are not automatically committed to the repository. Install the MSFS 2024 SDK and run scripts/prepare-simconnect.ps1 before producing a distributable build.

## Important

This is an original implementation. It does not copy proprietary code, assets, databases, prompts or other protected material from competing ATC products.
