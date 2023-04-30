using Easel.Audio;
using Easel.Headless.Scenes;

namespace Easel.Tests.TestScenes;

public class TestAudio : Scene
{
    protected override unsafe void Initialize()
    {
        base.Initialize();

        Sound sound = new Sound("/home/ollie/Music/r-59.ogg");
        sound.Play(speed: 1.0, loop: true);
    }
}