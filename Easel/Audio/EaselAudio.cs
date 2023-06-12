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
        for (ushort v = 0; v < PieAudio.NumVoices; v++)
        {
            // Paused voices should not be overwritten either.
            if (PieAudio.GetVoiceState(v) == PlayState.Stopped)
            {
                availableChannel = v;
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