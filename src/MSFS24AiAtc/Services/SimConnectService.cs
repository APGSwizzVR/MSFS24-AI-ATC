using MSFS24AiAtc.Core;

namespace MSFS24AiAtc.Services;

public sealed class SimConnectService : IDisposable
{
    public event Action<SimState>? StateChanged;
    public SimState State { get; private set; } = new(false, "", 0, 0, 0, 0, "", "");

    public bool TryConnect(IntPtr windowHandle)
    {
        // The production bridge is designed around the official MSFS 2024 managed
        // SimConnect wrapper. The wrapper and native runtime are deliberately not
        // redistributed by this repository; see README.md.
        //
        // This method reports the disconnected state until the SDK binaries are
        // installed and the concrete bridge is enabled.
        State = State with { Connected = false };
        StateChanged?.Invoke(State);
        return false;
    }

    public void Disconnect()
    {
        State = State with { Connected = false };
        StateChanged?.Invoke(State);
    }

    public void Dispose() => Disconnect();
}
