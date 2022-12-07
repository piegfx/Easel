using System;
using System.IO;
using Pie.Audio;

namespace Easel.Audio;

public class Sound : IDisposable
{
    private int _buffer;
    
    public Sound(string path)
    {
        //using Stream stream = File.OpenRead(path);
        //using BinaryReader reader = new BinaryReader(stream);

        AudioDevice device = EaselGame.Instance.AudioInternal;
        byte[] data = AudioHelper.LoadWav(File.ReadAllBytes(path), out AudioFormat format);
        _buffer = device.CreateBuffer();
        device.UpdateBuffer(_buffer, data, format);
    }

    public ISoundInstance Play(double volume = 1, double speed = 1, double panning = 0.5f, bool loop = false)
    {
        ChannelProperties properties = new ChannelProperties()
        {
            Volume = volume,
            Speed = speed,
            Panning = panning,
            Loop = loop
        };

        AudioDevice device = EaselGame.Instance.AudioInternal;
        ushort channel = device.GetAvailableChannel();
        device.PlayBuffer(_buffer, channel, properties);

        return new PcmInstance(EaselGame.Instance.AudioInternal, channel, properties);
    }

    public void Dispose()
    {
        AudioDevice device = EaselGame.Instance.AudioInternal;
        device.DeleteBuffer(_buffer);
    }

    private bool CheckWav(BinaryReader reader)
    {
        reader.BaseStream.Position = 0;
        bool value = new string(reader.ReadChars(4)) == "RIFF";
        reader.BaseStream.Position = 0;
        return value;
    }

    private bool CheckOggVorbis(BinaryReader reader)
    {
        reader.BaseStream.Position = 0;
        if (new string(reader.ReadChars(4)) != "OggS")
        {
            reader.BaseStream.Position = 0;
            return false;
        }

        reader.ReadBytes(25);
        if (new string(reader.ReadChars(6)) != "vorbis")
        {
            reader.BaseStream.Position = 0;
            return false;
        }

        reader.BaseStream.Position = 0;
        return true;
    }

    /*private SoundType GetSoundType(BinaryReader reader)
    {
        if (CheckWav(reader))
            return SoundType.Wav;
        if (CheckOggVorbis(reader))
            return SoundType.OggVorbis;
        return SoundType.Unknown;
    }*/
}