using System;
using System.IO;
using Easel.Audio;
using Easel.Scenes;

namespace Easel.Tests.TestScenes;

public class TestAudio : Scene
{
    protected override unsafe void Initialize()
    {
        base.Initialize();

        Sound sound = new Sound("/home/ollie/Music/LevelSelect2.wav");
        sound.Play(speed: 1.15f, loop: true);
    }
}