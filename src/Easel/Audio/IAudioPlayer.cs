using System;
using Pie.Audio;

namespace Easel.Audio;

public interface IAudioPlayer : IDisposable
{
    public ISoundInstance Play(ushort channel, in ChannelProperties properties);
}