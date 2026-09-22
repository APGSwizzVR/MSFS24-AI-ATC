# MSFS24 AI ATC

Windows AI ATC for Microsoft Flight Simulator 2024.

The application is being built as an out-of-process .NET executable. Microsoft documents SimConnect as the supported way for external applications to communicate with MSFS 2024 and recommends out-of-process applications for stability.

Core pipeline:

MSFS 2024 -> SimConnect -> live flight/facility state -> ATC rules/state engine -> AI -> speech -> VHF radio DSP -> audio output

## Target features

- Launch the ATC application before MSFS 2024.
- Automatically detect MSFS 2024.
- Global airport/facility coverage through simulator facility data plus online aviation data.
- Clearance, Ground, Tower, Departure, Approach, Center and Information.
- Real controller state rather than free-form chatbot behaviour.
- Per-controller AI model selection.
- OpenAI and OpenAI-compatible providers.
- Secure API key settings.
- Windows DPAPI encryption for stored keys.
- VHF band limiting.
- Compression.
- Static/noise bursts.
- Future squelch/opening/closing sounds.
- Flight-specific controller state.
- Windows self-contained EXE.
- Inno Setup EXE installer.
- GitHub Actions release build.

## API settings

Open Settings -> API & AI.

The OpenAI key, custom provider key and endpoint are entered there. They are stored encrypted for the current Windows user and are never written into the source tree.

For production deployment, a backend-issued short-lived realtime credential is preferable to distributing a permanent API key to every client.

OpenAI's Realtime API supports low-latency speech-to-speech over WebRTC/WebSocket and provides built-in audio input/output. Current documentation recommends the marin and cedar voices for best quality.

## Radio realism

The radio layer is deliberately separate from the AI. Generated speech should be processed as VHF radio audio rather than played as clean desktop TTS.

Planned chain:

- VHF bandwidth restriction
- dynamic compression
- mild saturation
- controlled static
- transmission start/end noise
- optional squelch
- cockpit radio volume

The effects should remain subtle enough to preserve phraseology intelligibility.

## Airport coverage

The ATC engine should not contain a fixed airport list. MSFS facility data is the source for live simulator facilities. Online aviation datasets can supplement metadata such as frequencies, procedures and airport information.

The goal is to support airports globally rather than maintaining a manually coded list.

## Important

This repository is an original implementation and does not copy proprietary code, assets, databases, prompts or other protected material from competing ATC products.
