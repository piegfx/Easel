using System;
using System.IO;
using Pie.Audio;
using StbVorbisSharp;

namespace Easel.Audio;

public class OggPlayer : IAudioPlayer
{
    private AudioBuffer[] _buffers;
    private Vorbis _vorbis;
    private int _currentBuffer;
    private uint _playingChannel;

    public OggPlayer(AudioDevice device, byte[] file)
    {
        _vorbis = Vorbis.FromMemory(file);
        _buffers = new AudioBuffer[2];
        for (int i = 0; i < _buffers.Length; i++)
        {
            _buffers[i] = device.CreateBuffer();
            FillBuffer(device, i);
        }
        
        device.BufferFinished += DeviceOnBufferFinished;
    }

    private void DeviceOnBufferFinished(uint channel)
    {
        if (channel != _playingChannel)
            return;
        AudioDevice device = EaselGame.Instance.AudioInternal;
        FillBuffer(device, _currentBuffer);
        device.Queue((int) channel, _buffers[_currentBuffer]);
        _currentBuffer++;
        if (_currentBuffer >= _buffers.Length)
            _currentBuffer = 0;
    }

    public void Play(AudioDevice device, int channel, float volume, float pitch, bool loop, Priority priority)
    {
        device.Play(channel, _buffers[0], volume, pitch, false, priority);
        for (int i = 1; i < _buffers.Length; i++)
            device.Queue(channel, _buffers[i]);
        _playingChannel = (uint) channel;
    }

    private void FillBuffer(AudioDevice device, int index)
    {
        _vorbis.SubmitBuffer();
        device.UpdateBuffer(_buffers[index], AudioFormat.Stereo16, _vorbis.SongBuffer, (uint) _vorbis.SampleRate);
    }
    
    public void Dispose()
    {
        _vorbis.Dispose();
        for (int i = 0; i < _buffers.Length; i++)
            _buffers[i].Dispose();
    }
}