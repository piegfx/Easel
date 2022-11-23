using System.Runtime.InteropServices;
using Silk.NET.SDL;
using static Easel.Mixr.MixrNative;

namespace Easel.Mixr;

public unsafe class AudioSystem : IDisposable
{
    private IntPtr _system;
    
    // Use SDL for audio.
    private Sdl _sdl;
    private uint _device;
    private AudioSpec _spec;

    public readonly ushort NumChannels;
    
    public AudioSystem(AudioFormat format, ushort channels)
    {
        NumChannels = channels;
        
        _system = mxCreateSystem(format, channels);
        
        _sdl = Sdl.GetApi();
        if (_sdl.Init(Sdl.InitAudio) != 0)
            throw new Exception("SDL could not initialize: " + Marshal.PtrToStringAnsi((IntPtr) _sdl.GetError()));
            
        _spec.Freq = format.SampleRate;
        _spec.Format = format.BitsPerSample switch
        {
            //8 => Sdl.AudioU8,
            16 => Sdl.AudioS16,
            _ => throw new NotSupportedException("Currently, only 16 bit audio is supported.")
        };
        _spec.Channels = format.Channels;
        _spec.Samples = 512;

        _spec.Callback = new PfnAudioCallback(AudioCallback);

        fixed (AudioSpec* spec = &_spec)
            _device = _sdl.OpenAudioDevice((byte*) null, 0, spec, null, 0);
        _sdl.PauseAudioDevice(_device, 0);
    }

    public int CreateBuffer() => mxCreateBuffer(_system);

    public void UpdateBuffer(int buffer, byte[] data, AudioFormat format)
    {
        fixed (byte* buf = data)
            mxUpdateBuffer(_system, buffer, buf, (nuint) data.Length, format);
    }

    public void PlayBuffer(ushort channel, int buffer, double volume = 1.0, double speed = 1.0, double panning = 0.5) =>
        mxPlayBuffer(_system, channel, buffer, volume, speed, panning);

    private void AudioCallback(void* arg0, byte* bData, int len)
    {
        for (int i = 0; i < len; i += 2)
        {
            short advance = mxAdvance(_system);
            bData[i] = (byte) (advance & 0xFF);
            bData[i + 1] = (byte) (advance >> 8);
        }
    }

    public void Dispose()
    {
        mxDeleteSystem(_system);
        _sdl.CloseAudioDevice(_device);
        _sdl.Quit();
    }
}