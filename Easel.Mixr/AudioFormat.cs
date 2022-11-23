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

    public AudioFormat(Pie.Audio.AudioFormat pieFormat, int sampleRate)
    {
        SampleRate = sampleRate;
        
        switch (pieFormat)
        {
            case Pie.Audio.AudioFormat.Mono8:
                Channels = 1;
                BitsPerSample = 8;
                break;
            case Pie.Audio.AudioFormat.Mono16:
                Channels = 1;
                BitsPerSample = 16;
                break;
            case Pie.Audio.AudioFormat.Stereo8:
                Channels = 2;
                BitsPerSample = 8;
                break;
            case Pie.Audio.AudioFormat.Stereo16:
                Channels = 2;
                BitsPerSample = 16;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(pieFormat), pieFormat, null);
        }
    }

    public static readonly AudioFormat Stereo48khz = new AudioFormat(2, 48000, 16);
}