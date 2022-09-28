using System;
using System.IO;
using Pie.Audio;

namespace Easel.Audio;

public class WavPlayer : IAudioPlayer
{
    private AudioBuffer _buffer;

    public readonly byte[] Data;
    public readonly uint SampleRate;

    public WavPlayer(AudioDevice device, byte[] file)
    {
        _buffer = device.CreateBuffer();
        Data = AudioHelper.LoadWav(file, out SampleRate, out AudioFormat format);
        device.UpdateBuffer(_buffer, format, Data, SampleRate);
    }

    public void Play(AudioDevice device, int channel, float volume, float pitch, bool loop, Priority priority)
    {
        Console.WriteLine(device.FindFreeChannelIfAvailable());
        device.Play(channel, _buffer, volume, pitch, loop, priority);
    }

    public void Dispose()
    {
        _buffer.Dispose();
    }
}