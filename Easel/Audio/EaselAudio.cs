using System;
using Pie.Audio;

namespace Easel.Audio;

public class EaselAudio : IDisposable
{
    public readonly AudioDevice PieAudio;

    public EaselAudio()
    {
        PieAudio = new AudioDevice(48000, 256);
    }

    public bool TryGetAvailableChannel(out ushort availableChannel)
    {
        for (ushort c = 0; c < PieAudio.NumChannels; c++)
        {
            if (!PieAudio.IsPlaying(c))
            {
                availableChannel = c;
                return true;
            }
        }

        availableChannel = ushort.MaxValue;
        return false;
    }

    public void Dispose()
    {
        PieAudio?.Dispose();
    }
}