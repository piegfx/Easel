using Pie.Audio;

namespace Easel.Mixr;

/// <summary>
/// Implement custom audio players for playing different formats.
/// </summary>
public interface IAudioPlayer : IDisposable
{
    /// <summary>
    /// Play the sound from the player stream.
    /// </summary>
    /// <param name="device">The audio device to play to.</param>
    /// <param name="channel">The channel to play on.</param>
    /// <param name="volume">The volume to play at.</param>
    /// <param name="pitch">The pitch to play at.</param>
    /// <param name="loop">Loop the audio?</param>
    /// <param name="priority">The priority of the audio.</param>
    public void Play(AudioSystem device, ushort channel, float volume, float pitch, bool loop);
}