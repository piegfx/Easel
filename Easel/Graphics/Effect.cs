using System;
using System.IO;
using Pie;
using Pie.ShaderCompiler;

namespace Easel.Graphics;

/// <summary>
/// Represents a shader effect with a vertex and fragment/pixel shader.
/// </summary>
public class Effect : IDisposable
{
    /// <summary>
    /// The native Pie <see cref="Shader"/> object.
    /// </summary>
    public Shader PieShader;

    /// <summary>
    /// Create a new <see cref="Effect"/> with the given vertex and fragment/pixel shader.
    /// </summary>
    /// <param name="vertex">The vertex shader to use.</param>
    /// <param name="fragment">The fragment/pixel shader to use.</param>
    /// <param name="loadType">The <see cref="EffectLoadType"/> that both shaders will be loaded with</param>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
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

/// <summary>
/// Represents the possible ways an effect can load its given shaders.
/// </summary>
public enum EffectLoadType
{
    /// <summary>
    /// The shader is stored as a string. (Provide shader code).
    /// </summary>
    String,
    
    /// <summary>
    /// The shader is stored as a file. (Provide path to shader file).
    /// </summary>
    File,
    
    /// <summary>
    /// The shader is stored as an embedded resource. (Provide an assembly namespace).
    /// </summary>
    EmbeddedResource
}