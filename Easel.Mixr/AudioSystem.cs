using static Easel.Mixr.MixrNative;

namespace Easel.Mixr;

public unsafe class AudioSystem : IDisposable
{
    private IntPtr _system;
    
    public AudioSystem(AudioFormat format, ushort channels)
    {
        _system = mxCreateSystem(format, channels);
    }

    public int CreateBuffer() => mxCreateBuffer(_system);

    public void UpdateBuffer(int buffer, byte[] data, AudioFormat format)
    {
        fixed (byte* buf = data)
            mxUpdateBuffer(_system, buffer, buf, (nuint) data.Length, format);
    }

    public void PlayBuffer(ushort channel, int buffer, double volume = 1.0, double speed = 1.0, double panning = 0.5) =>
        mxPlayBuffer(_system, channel, buffer, volume, speed, panning);

    public short Advance() => mxAdvance(_system);

    public void Dispose() => mxDeleteSystem(_system);
}