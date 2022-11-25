using Pie.Audio;

namespace Easel.Mixr;

public class WavPlayer : IAudioPlayer
{
    private int _buffer;

    public readonly byte[] Data;
    public readonly uint SampleRate;

    public WavPlayer(AudioSystem device, byte[] file)
    {
        _buffer = device.CreateBuffer();
        Data = AudioHelper.LoadWav(file, out SampleRate, out Pie.Audio.AudioFormat format);
        device.UpdateBuffer(_buffer, Data, new AudioFormat(format, (int) SampleRate));
    }

    public void Play(AudioSystem device, ushort channel, float volume, float pitch, bool loop)
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
        // _buffer.Dispose();
    }
}