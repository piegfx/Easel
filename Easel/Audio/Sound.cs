using System;
using System.Collections.Generic;
using System.IO;
using Pie.Audio;

namespace Easel.Audio;

public class Sound : IDisposable
{
    public readonly SoundType Type;

    public readonly IAudioPlayer Player;

    public Sound(string path)
    {
        using Stream stream = File.OpenRead(path);
        using BinaryReader reader = new BinaryReader(stream);
        Type = GetSoundType(reader);
        AudioDevice device = EaselGame.Instance.AudioInternal;
        switch (Type)
        {
            case SoundType.Unknown:
                throw new EaselException("Given sound file is not a supported format.");
            case SoundType.Wav:
                Player = new WavPlayer(device, reader.ReadBytes((int) reader.BaseStream.Length));
                break;
            case SoundType.OggVorbis:
                Player = new OggPlayer(device, reader.ReadBytes((int) reader.BaseStream.Length));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public int Play(int channel = -1, float volume = 1, float pitch = 1, bool loop = false, Priority priority = Priority.Low)
    {
        // TODO: Volume and stuff
        AudioDevice device = EaselGame.Instance.AudioInternal;
        channel = channel < 0 ? device.FindChannel() : channel;
        Player.Play(device, channel, volume, pitch, loop, priority);
        return channel;
    }

    public void Dispose()
    {
        Player.Dispose();
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

    private SoundType GetSoundType(BinaryReader reader)
    {
        if (CheckWav(reader))
            return SoundType.Wav;
        if (CheckOggVorbis(reader))
            return SoundType.OggVorbis;
        return SoundType.Unknown;
    }
}