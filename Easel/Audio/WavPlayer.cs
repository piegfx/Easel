using System;
using System.IO;
using Pie.Audio;

namespace Easel.Audio;

public class WavPlayer : IAudioPlayer
{
    private AudioBuffer _buffer;
    
    public WavPlayer(AudioDevice device, byte[] file)
    {
        _buffer = device.CreateBuffer();
        byte[] data = AudioHelper.LoadWav(file, out uint sampleRate, out AudioFormat format);
        device.UpdateBuffer(_buffer, format, data, sampleRate);
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