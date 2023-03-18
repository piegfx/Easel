using System;
using Easel.Core;
using Pie.Audio;
using StbVorbisSharp;

namespace Easel.Audio;

public class VorbisPlayer : IAudioPlayer
{
    private AudioDevice _device;
    private Vorbis _vorbis;

    public const int NumBuffers = 2;
    private AudioBuffer[] _buffers;
    private AudioFormat _format;
    private int _currentBuffer;

    private ushort _channel;
    
    public VorbisPlayer(AudioDevice device, byte[] data)
    {
        _device = device;
        
        _vorbis = Vorbis.FromMemory(data);

        _format = new AudioFormat((byte) _vorbis.Channels, _vorbis.SampleRate, FormatType.I16);
        
        _buffers = new AudioBuffer[NumBuffers];
        for (int i = 0; i < NumBuffers; i++)
            _buffers[i] = _device.CreateBuffer(new BufferDescription(DataType.Pcm, _format), GetNextVorbisData());

        _device.BufferFinished += DeviceOnBufferFinished;
    }

    private void DeviceOnBufferFinished(AudioSystem system, ushort channel, AudioBuffer buffer)
    {
        if (channel != _channel)
            return;
        
        _device.UpdateBuffer(_buffers[_currentBuffer], GetNextVorbisData());
        _device.QueueBuffer(_buffers[_currentBuffer++], _channel);
        if (_currentBuffer >= NumBuffers)
            _currentBuffer = 0;
    }

    private short[] GetNextVorbisData()
    {
        _vorbis.SubmitBuffer();
        if (_vorbis.Decoded * _vorbis.Channels < _vorbis.SongBuffer.Length)
            _vorbis.Restart();

        short[] data = new short[_vorbis.Decoded * _vorbis.Channels];
        Array.Copy(_vorbis.SongBuffer, data, _vorbis.Decoded * _vorbis.Channels);

        return data;
    }
    
    public ISoundInstance Play(ushort channel, in ChannelProperties properties)
    {
        _channel = channel;
        _device.PlayBuffer(_buffers[0], channel, properties);
        for (int i = 1; i < NumBuffers; i++)
            _device.QueueBuffer(_buffers[i], channel);
        return new PcmInstance(_device, channel, properties);
    }
    
    public void Dispose()
    {
        _device.BufferFinished -= DeviceOnBufferFinished;
        
        Logger.Debug($"Disposing {NumBuffers} buffers...");
        
        for (int i = 0; i < NumBuffers; i++)
            _device.DeleteBuffer(_buffers[i]);
        _vorbis.Dispose();
    }
}