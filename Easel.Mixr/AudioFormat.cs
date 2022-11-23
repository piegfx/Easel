using System.Runtime.InteropServices;

namespace Easel.Mixr;

[StructLayout(LayoutKind.Sequential)]
public struct AudioFormat
{
    public byte Channels;
    public int SampleRate;
    public byte BitsPerSample;

    public AudioFormat(byte channels, int sampleRate, byte bitsPerSample)
    {
        Channels = channels;
        SampleRate = sampleRate;
        BitsPerSample = bitsPerSample;
    }
}