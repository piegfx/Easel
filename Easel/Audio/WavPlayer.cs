using Pie.Audio;

namespace Easel.Audio;

public class WavPlayer : IAudioPlayer
{
    private AudioDevice _device;
    
    private int _buffer;

    public WavPlayer(AudioDevice device, byte[] data)
    {
        _device = device;
        _buffer = _device.CreateBuffer();
        
        _device.UpdateBuffer(_buffer, AudioHelper.LoadWav(data, out AudioFormat format), format);
    }
    
    public ISoundInstance Play(ushort channel, in ChannelProperties properties)
    {
        _device.PlayBuffer(_buffer, channel, properties);
        return new PcmInstance(_device, channel, properties);
    }

    public void Dispose()
    {
        _device.DeleteBuffer(_buffer);
    }
}