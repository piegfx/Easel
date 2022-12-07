using System;
using System.Runtime.InteropServices;

namespace Easel.Audio;

public static unsafe class MixrNative
{
    private const string MixrName = "mixr";

    [DllImport(MixrName)]
    public static extern IntPtr mxCreateSystem(AudioFormat format, ushort channels);

    [DllImport(MixrName)]
    public static extern void mxDeleteSystem(IntPtr system);
    
    [DllImport(MixrName)]
    public static extern void mxSetBufferFinishedCallback(IntPtr system, BufferFinishedCallback callback);

    [DllImport(MixrName)]
    public static extern int mxCreateBuffer(IntPtr system);

    [DllImport(MixrName)]
    public static extern AudioResult mxDeleteBuffer(IntPtr system, int buffer);

    [DllImport(MixrName)]
    public static extern AudioResult mxUpdateBuffer(IntPtr system, int buffer, byte* data, nuint dataLength,
        AudioFormat format);

    [DllImport(MixrName)]
    public static extern AudioResult mxPlayBuffer(IntPtr system, int buffer, ushort channel, ChannelProperties properties);

    [DllImport(MixrName)]
    public static extern AudioResult mxQueueBuffer(IntPtr system, int buffer, ushort channel);

    [DllImport(MixrName)]
    public static extern AudioResult mxSetChannelProperties(IntPtr system, ushort channel, ChannelProperties properties);

    [DllImport(MixrName)]
    public static extern AudioResult mxPlay(IntPtr system, ushort channel);
    
    [DllImport(MixrName)]
    public static extern AudioResult mxPause(IntPtr system, ushort channel);
    
    [DllImport(MixrName)]
    public static extern AudioResult mxStop(IntPtr system, ushort channel);

    [DllImport(MixrName)]
    public static extern short mxAdvance(IntPtr system);

    [DllImport(MixrName)]
    public static extern ushort mxGetNumChannels(IntPtr system);

    [DllImport(MixrName)]
    public static extern bool mxIsPlaying(IntPtr system, ushort channel);

    [DllImport(MixrName)]
    public static extern ushort mxGetAvailableChannel(IntPtr system);

    public delegate void BufferFinishedCallback(ushort channel, int buffer);

    public enum AudioResult
    {
        Ok,
        InvalidBuffer,
        InvalidChannels,
        NoChannels
    }
}