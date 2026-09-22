using NAudio.Wave;

namespace MSFS24AiAtc.Services;

public sealed class RadioAudioService
{
    public float Volume { get; set; } = 0.85f;
    public bool EnableStatic { get; set; } = true;
    public bool EnableBandpass { get; set; } = true;
    public bool EnableCompression { get; set; } = true;

    public byte[] ProcessPcm16(byte[] pcm, int sampleRate)
    {
        var samples = new short[pcm.Length / 2];
        Buffer.BlockCopy(pcm, 0, samples, 0, pcm.Length);
        var previous = 0f;

        for (var i = 0; i < samples.Length; i++)
        {
            var x = samples[i] / 32768f;
            var filtered = EnableBandpass ? x - previous * 0.985f : x;
            previous = x;

            var y = filtered;
            if (EnableCompression)
                y = MathF.Sign(y) * (1f - MathF.Exp(-MathF.Abs(y) * 2.8f));

            y *= Volume;

            if (EnableStatic && i % Math.Max(1, sampleRate / 80) == 0)
                y += (Random.Shared.NextSingle() * 2f - 1f) * 0.008f;

            samples[i] = (short)Math.Clamp(y * 32767f, short.MinValue, short.MaxValue);
        }

        var output = new byte[pcm.Length];
        Buffer.BlockCopy(samples, 0, output, 0, output.Length);
        return output;
    }

    public async Task PlayFileAsync(string file, CancellationToken ct = default)
    {
        if (!File.Exists(file)) return;
        using var reader = new AudioFileReader(file);
        using var output = new WaveOutEvent();
        output.Init(new RadioWaveProvider(reader, this));
        output.Play();

        while (output.PlaybackState == PlaybackState.Playing)
        {
            ct.ThrowIfCancellationRequested();
            await Task.Delay(25, ct);
        }
    }

    private sealed class RadioWaveProvider : IWaveProvider
    {
        private readonly IWaveProvider source;
        private readonly RadioAudioService owner;
        public WaveFormat WaveFormat => source.WaveFormat;

        public RadioWaveProvider(IWaveProvider source, RadioAudioService owner)
        {
            this.source = source;
            this.owner = owner;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            var temp = new byte[count];
            var read = source.Read(temp, 0, count);
            if (read == 0) return 0;
            var processed = owner.ProcessPcm16(temp[..read], WaveFormat.SampleRate);
            Buffer.BlockCopy(processed, 0, buffer, offset, processed.Length);
            return processed.Length;
        }
    }
}
