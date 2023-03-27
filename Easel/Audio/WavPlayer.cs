using Easel.Core;
using Pie.Audio;

namespace Easel.Audio;

public class WavPlayer : IAudioPlayer
{
    private AudioDevice _device;
    
    private AudioBuffer _buffer;

    public WavPlayer(AudioDevice device, byte[] data)
    {
        _device = device;
        
        PCM pcm = PCM.LoadWav(data);
        
        _buffer = _device.CreateBuffer(new BufferDescription(DataType.Pcm, pcm.Format), pcm.Data);
    }
    
    public ISoundInstance Play(ushort channel, in ChannelProperties properties)
    {
        _device.PlayBuffer(_buffer, channel, properties);
        return new PcmInstance(_device, channel, properties);
    }

    public void Dispose()
    {
        Logger.Debug("Disposing of PCM audio buffer...");
        _device.DeleteBuffer(_buffer);
    }
}