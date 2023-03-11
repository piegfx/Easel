using System;
using System.IO;
using Pie.Audio;

namespace Easel.Audio;

public class Sound : IDisposable
{
    public IAudioPlayer AudioPlayer;
    public SoundType SoundType;
    
    public Sound(string path)
    {
        using Stream stream = File.OpenRead(path);
        using BinaryReader reader = new BinaryReader(stream);

        EaselAudio device = EaselGame.Instance.AudioInternal;

        SoundType = GetSoundType(reader);
        switch (SoundType)
        {
            case SoundType.Wav:
                AudioPlayer = new WavPlayer(device.PieAudio, reader.ReadBytes((int) reader.BaseStream.Length));
                break;
            case SoundType.Vorbis:
                AudioPlayer = new VorbisPlayer(device.PieAudio, reader.ReadBytes((int) reader.BaseStream.Length));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public ISoundInstance Play(double volume = 1, double speed = 1, double panning = 0.5f, bool loop = false)
    {
        ChannelProperties properties = new ChannelProperties()
        {
            Volume = volume,
            Speed = speed,
            Panning = panning,
            Looping = SoundType != SoundType.Vorbis && loop,
            Interpolation = InterpolationType.Linear,
            LoopStart = 0,
            LoopEnd = -1
        };

        EaselAudio device = EaselGame.Instance.AudioInternal;
        if (!device.TryGetAvailableChannel(out ushort channel))
        {
            throw new NotImplementedException(
                "Oopsy poopsy too many sounds are playing but nO PRIORITY SYSTEM HAS BEEN IMPLEMENTED BECAUSE SKYE IS LAZY");
        }

        return AudioPlayer.Play(channel, properties);
    }

    public void Dispose()
    {
        AudioPlayer.Dispose();
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
            return SoundType.Vorbis;
        return SoundType.Unknown;
    }
}