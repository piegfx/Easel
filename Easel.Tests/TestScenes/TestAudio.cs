using System;
using System.IO;
using Easel.Audio;
using Easel.Scenes;
using Pie.Audio;

namespace Easel.Tests.TestScenes;

public class TestAudio : Scene
{
    protected override unsafe void Initialize()
    {
        base.Initialize();

        AudioDevice device = Audio.PieAudio;

        PCM pcm = PCM.LoadWav("/home/ollie/Music/strange airy.wav");

        AudioBuffer buffer = device.CreateBuffer(new BufferDescription(DataType.Pcm, pcm.Format), pcm.Data);

        const int numIterations = 256;
        
        ChannelProperties properties = new ChannelProperties()
        {
            Volume = 1.0 / numIterations
        };
        
        for (ushort i = 0; i < numIterations; i++)
            device.PlayBuffer(buffer, i, properties);
    }
}