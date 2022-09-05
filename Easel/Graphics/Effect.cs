using System;
using System.IO;
using Easel.Utilities;
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
    public Effect(string vertex, string fragment, EffectLoadType loadType = EffectLoadType.EmbeddedResource)
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
                vertex = Utils.LoadEmbeddedString(vertex);
                fragment = Utils.LoadEmbeddedString(fragment);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(loadType), loadType, null);
        }

        vertex = PreProcess(vertex);
        fragment = PreProcess(fragment);

        string[] fragLines = vertex.Split('\n');
        for (int i = 0; i < fragLines.Length; i++)
            Console.WriteLine(i + 1 + ": " + fragLines[i]);

        PieShader = device.CreateCrossPlatformShader(
            new ShaderAttachment(ShaderStage.Vertex, vertex),
            new ShaderAttachment(ShaderStage.Fragment, fragment));
    }

    public void Dispose()
    {
        PieShader.Dispose();
    }

    private static string PreProcess(string shader)
    {
        string[] splitText = shader.Split('\n');

        bool hasIncluded = false;

        for (int i = 0; i < splitText.Length; i++)
        {
            string line = splitText[i];

            if (line.StartsWith("#include"))
            {
                shader = shader.Replace(line, Utils.LoadEmbeddedString(line[("#include ".Length)..].Trim('"')));
                hasIncluded = true;
            }
        }
        
        return hasIncluded ? PreProcess(shader) : shader;
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