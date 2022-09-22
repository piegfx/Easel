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
    private bool _loop;

    public OggPlayer(AudioDevice device, byte[] file)
    {
        _vorbis = Vorbis.FromMemory(file);
        _buffers = new AudioBuffer[2];
        for (int i = 0; i < _buffers.Length; i++)
            _buffers[i] = device.CreateBuffer();

        device.BufferFinished += DeviceOnBufferFinished;
    }

    private void DeviceOnBufferFinished(AudioDevice device, uint channel)
    {
        if (channel != _playingChannel)
            return;
        FillBuffer(device, _currentBuffer);
        device.Queue((int) channel, _buffers[_currentBuffer]);
        _currentBuffer++;
        if (_currentBuffer >= _buffers.Length)
            _currentBuffer = 0;
    }

    public void Play(AudioDevice device, int channel, float volume, float pitch, bool loop, Priority priority)
    {
        _vorbis.Restart();
        device.Stop((int) _playingChannel);
        for (int i = 0; i < _buffers.Length; i++)
            FillBuffer(device, i);
        _currentBuffer = 0;
        device.Play(channel, _buffers[0], volume, pitch, false, priority);
        for (int i = 1; i < _buffers.Length; i++)
            device.Queue(channel, _buffers[i]);
        _playingChannel = (uint) channel;
        _loop = loop;
    }

    private void FillBuffer(AudioDevice device, int index)
    {
        _vorbis.SubmitBuffer();
        short[] data = _vorbis.SongBuffer;
        if (_vorbis.Decoded * _vorbis.Channels < _vorbis.SampleRate)
        {
            Array.Resize(ref data, _vorbis.Decoded * _vorbis.Channels);
            if (_loop)
                _vorbis.Restart();
            else if (_vorbis.Decoded == 0)
                device.Stop((int) _playingChannel);
        }

        device.UpdateBuffer(_buffers[index], AudioFormat.Stereo16, data, (uint) _vorbis.SampleRate);
    }
    
    public void Dispose()
    {
        _vorbis.Dispose();
        for (int i = 0; i < _buffers.Length; i++)
            _buffers[i].Dispose();
    }
}