using Pie.Audio;

namespace Easel.Audio;

public class WavPlayer : IAudioPlayer
{
    private int _buffer;

    public readonly byte[] Data;
    public readonly AudioFormat Format;

    public WavPlayer(AudioDevice device, byte[] file)
    {
        _buffer = device.CreateBuffer();
        Data = AudioHelper.LoadWav(file, out Format);
        device.UpdateBuffer(_buffer, Data, Format);
    }

    public void Play(AudioDevice device, ushort channel, float volume, float pitch, bool loop)
    {
        device.PlayBuffer(_buffer, channel, new ChannelProperties()
        {
            Volume = volume,
            Speed = pitch,
            Loop = loop
        });
    }

    public void Dispose()
    {
        EaselGame.Instance.AudioInternal.DeleteBuffer(_buffer);
    }
}