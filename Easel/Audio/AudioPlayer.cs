using System;
using Pie.Audio;

namespace Easel.Audio;

public interface IAudioPlayer : IDisposable
{
    public void Play(AudioDevice device, int channel, float volume, float pitch, bool loop, Priority priority);
}