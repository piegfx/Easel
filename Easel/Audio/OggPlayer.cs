using System;
using StbVorbisSharp;

namespace Easel.Audio;

/// <summary>
/// Streams & plays Ogg Vorbis audio files.
/// </summary>
public class OggPlayer : IAudioPlayer
{
    private int[] _buffers;
    private Vorbis _vorbis;
    private int _currentBuffer;
    private ushort _playingChannel;
    private bool _loop;

    private ChannelProperties _properties;

    public OggPlayer(AudioDevice device, byte[] file)
    {
        _vorbis = Vorbis.FromMemory(file);
        _buffers = new int[2];
        for (int i = 0; i < _buffers.Length; i++)
            _buffers[i] = device.CreateBuffer();

        device.BufferFinished += DeviceOnBufferFinished;
    }

    private void DeviceOnBufferFinished(AudioDevice device, ushort channel, int buffer)
    {
        if (channel != _playingChannel)
            return;
        FillBuffer(device, _currentBuffer);
        device.QueueBuffer(_buffers[_currentBuffer], channel);
        _currentBuffer++;
        if (_currentBuffer >= _buffers.Length)
            _currentBuffer = 0;
    }
    
    /// <inheritdoc />
    public void Play(AudioDevice device, ushort channel, float volume, float pitch, bool loop)
    {
        _vorbis.Restart();
        device.Stop(_playingChannel);
        for (int i = 0; i < _buffers.Length; i++)
            FillBuffer(device, i);
        _currentBuffer = 0;
        _properties = new ChannelProperties()
        {
            Volume = volume,
            Speed = pitch
        };
        device.PlayBuffer(_buffers[0], channel, _properties);
        for (int i = 1; i < _buffers.Length; i++)
            device.QueueBuffer(_buffers[i], channel);
        _playingChannel = channel;
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
                device.Stop(_playingChannel);
        }

        byte[] bData = new byte[data.Length * 2];
        for (int i = 0; i < data.Length; i++)
        {
            bData[i * 2] = (byte) (data[i] & 0xFF);
            bData[i * 2 + 1] = (byte) (data[i] >> 8);
        }

        device.UpdateBuffer(_buffers[index], bData, new AudioFormat(2, _vorbis.SampleRate, 16));
    }
    
    public void Dispose()
    {
        _vorbis.Dispose();
        for (int i = 0; i < _buffers.Length; i++)
            EaselGame.Instance.AudioInternal.DeleteBuffer(_buffers[i]);
    }
}