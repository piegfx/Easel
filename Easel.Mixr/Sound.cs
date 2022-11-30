using Pie.Audio;

namespace Easel.Mixr;

public class Sound : IDisposable
{
    public readonly SoundType Type;

    public readonly IAudioPlayer Player;

    private AudioSystem _system;

    public Sound(AudioSystem system, string path)
    {
        using Stream stream = File.OpenRead(path);
        using BinaryReader reader = new BinaryReader(stream);
        Type = GetSoundType(reader);

        _system = system;
        switch (Type)
        {
            case SoundType.Unknown:
                throw new Exception("Given sound file is not a supported format.");
            case SoundType.Wav:
                Player = new WavPlayer(system, reader.ReadBytes((int) reader.BaseStream.Length));
                break;
            case SoundType.OggVorbis:
                Player = new OggPlayer(system, reader.ReadBytes((int) reader.BaseStream.Length));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Play(ushort channel, float volume = 1, float pitch = 1, bool loop = false)
    {
        // TODO: Volume and stuff
        Player.Play(_system, channel, volume, pitch, loop);
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