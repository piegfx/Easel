using System.Runtime.InteropServices;

namespace Easel.Mixr;

public static unsafe class MixrNative
{
    private const string MixrName = "mixr";

    [DllImport(MixrName)]
    public static extern IntPtr mxCreateSystem(AudioFormat format, ushort channels);

    [DllImport(MixrName)]
    public static extern void mxDeleteSystem(IntPtr system);

    [DllImport(MixrName)]
    public static extern int mxCreateBuffer(IntPtr system);

    [DllImport(MixrName)]
    public static extern void mxUpdateBuffer(IntPtr system, int buffer, byte* data, nuint dataLength,
        AudioFormat format);

    [DllImport(MixrName)]
    public static extern void mxPlayBuffer(IntPtr system, int buffer, ushort channel, ChannelProperties properties);

    [DllImport(MixrName)]
    public static extern void mxSetChannelProperties(IntPtr system, ushort channel, ChannelProperties properties);

    [DllImport(MixrName)]
    public static extern void mxPlay(IntPtr system, ushort channel);
    
    [DllImport(MixrName)]
    public static extern void mxPause(IntPtr system, ushort channel);
    
    [DllImport(MixrName)]
    public static extern void mxStop(IntPtr system, ushort channel);

    [DllImport(MixrName)]
    public static extern short mxAdvance(IntPtr system);
}