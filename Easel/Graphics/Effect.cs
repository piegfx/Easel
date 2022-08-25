using System;
using System.IO;
using Pie;
using Pie.ShaderCompiler;

namespace Easel.Graphics;

public class Effect : IDisposable
{
    public Shader PieShader;

    public Effect(string vertex, string fragment, EffectLoadType loadType = EffectLoadType.String)
    {
        GraphicsDevice device = EaselGame.Instance.GraphicsInternal.PieGraphics;
        switch (loadType)
        {
            case EffectLoadType.String:
                // Do nothing
                break;
            case EffectLoadType.File:
                vertex = File.ReadAllText(vertex);
                fragment = File.ReadAllText(fragment);
                break;
            case EffectLoadType.EmbeddedResource:
                throw new NotImplementedException();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(loadType), loadType, null);
        }

        PieShader = device.CreateCrossPlatformShader(
            new ShaderAttachment(ShaderStage.Vertex, vertex),
            new ShaderAttachment(ShaderStage.Fragment, fragment));
    }

    public void Dispose()
    {
        PieShader.Dispose();
    }
}

public enum EffectLoadType
{
    String,
    File,
    EmbeddedResource
}