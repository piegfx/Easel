using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Easel.Core;
using Microsoft.VisualBasic.CompilerServices;
using Pie;
using Pie.ShaderCompiler;
using Utils = Easel.Core.Utils;

namespace Easel.Graphics;

/// <summary>
/// Represents a shader effect with a vertex and fragment/pixel shader.
/// </summary>
public class Effect : IDisposable
{
    private static bool _displayEffects;
    private static bool _hasCheckedForDisplayEffects;
    
    /// <summary>
    /// The native Pie <see cref="Shader"/> object.
    /// </summary>
    public readonly Shader PieShader;

    /*/// <summary>
    /// Create a new <see cref="Effect"/> with the given vertex and fragment/pixel shader.
    /// </summary>
    /// <param name="vertex">The vertex shader to use.</param>
    /// <param name="fragment">The fragment/pixel shader to use.</param>
    /// <param name="loadType">The <see cref="EffectLoadType"/> that both shaders will be loaded with</param>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Effect(string vertex, string fragment, EffectLoadType loadType = EffectLoadType.EmbeddedResource, params string[] defines)
    {
        GraphicsDevice device = EaselGraphics.Instance.PieGraphics;
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
                vertex = Utils.LoadEmbeddedString(Assembly.GetExecutingAssembly(), vertex);
                fragment = Utils.LoadEmbeddedString(Assembly.GetExecutingAssembly(), fragment);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(loadType), loadType, null);
        }

        StringBuilder defineBuilder = new StringBuilder("#version 450\n");
        foreach (string define in defines)
            defineBuilder.AppendLine("#define " + define);

        vertex = vertex.Insert(0, defineBuilder.ToString());
        fragment = fragment.Insert(0, defineBuilder.ToString());

        vertex = PreProcess(vertex);
        fragment = PreProcess(fragment);

#if DEBUG
        if (!_hasCheckedForDisplayEffects)
        {
            _hasCheckedForDisplayEffects = true;
            Logger.Debug($"Checking for {EnvVars.PrintEffects}...");
            string? pr = Environment.GetEnvironmentVariable(EnvVars.PrintEffects);
            if (pr is "1")
            {
                _displayEffects = true;
                Logger.Info($"{EnvVars.PrintEffects} is enabled. Effects will be printed to console.");
            }
        }

        if (_displayEffects)
        {
            LogShader(ShaderStage.Vertex, vertex);
            LogShader(ShaderStage.Fragment, fragment);
        }
#endif
        
        Logger.Debug("Compiling shader...");

        PieShader = device.CreateShader(
            new []{ new ShaderAttachment(ShaderStage.Vertex, vertex),
            new ShaderAttachment(ShaderStage.Fragment, fragment) });
    }

    public Effect(string shader, EffectLoadType loadType = EffectLoadType.EmbeddedResource, params string[] defines)
    {
        GraphicsDevice device = EaselGraphics.Instance.PieGraphics;
        switch (loadType)
        {
            case EffectLoadType.String:
                // Do nothing
                break;
            case EffectLoadType.File:
                shader = File.ReadAllText(shader);
                break;
            case EffectLoadType.EmbeddedResource:
                shader = Utils.LoadEmbeddedString(Assembly.GetExecutingAssembly(), shader);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(loadType), loadType, null);
        }
        
        StringBuilder defineBuilder = new StringBuilder();
        foreach (string define in defines)
            defineBuilder.AppendLine("#define " + define);

#if DEBUG
        if (!_hasCheckedForDisplayEffects)
        {
            _hasCheckedForDisplayEffects = true;
            Logger.Debug($"Checking for {EnvVars.PrintEffects}...");
            string? pr = Environment.GetEnvironmentVariable(EnvVars.PrintEffects);
            if (pr is "1")
            {
                _displayEffects = true;
                Logger.Info($"{EnvVars.PrintEffects} is enabled. Effects will be printed to console.");
            }
        }

        if (_displayEffects)
        {
            LogShader(ShaderStage.Vertex, shader);
        }
#endif
        
        Logger.Debug("Compiling shader...");

        PieShader = device.CreateShader(
            new []{ new ShaderAttachment(ShaderStage.Vertex, shader, Language.HLSL, "VertexShader"),
            new ShaderAttachment(ShaderStage.Pixel, shader, Language.HLSL, "PixelShader") });
    }*/

    public Effect(byte[] vertSpirv, byte[] fragSpirv, SpecializationConstant[] constants = null)
    {
        GraphicsDevice device = EaselGraphics.Instance.PieGraphics;
        
        Logger.Debug("Compiling shader...");

        PieShader = device.CreateShader(
            new[]
            {
                new ShaderAttachment(ShaderStage.Vertex, vertSpirv),
                new ShaderAttachment(ShaderStage.Fragment, fragSpirv)
            }, constants);
    }

    public static Effect FromPath(string vertexPath, string fragmentPath,
        EffectLoadType loadType = EffectLoadType.EmbeddedResource, SpecializationConstant[] constants = null,
        Assembly assembly = null)
    {
        byte[] vertex, fragment;
        
        switch (loadType)
        {
            case EffectLoadType.String:
                throw new NotSupportedException("SPIR-V cannot be loaded from a string.");
            case EffectLoadType.File:
                vertex = File.ReadAllBytes(vertexPath);
                fragment = File.ReadAllBytes(fragmentPath);
                break;
            case EffectLoadType.EmbeddedResource:
                assembly ??= Assembly.GetExecutingAssembly();
                vertex = Utils.LoadEmbeddedResource(assembly, vertexPath);
                fragment = Utils.LoadEmbeddedResource(assembly, fragmentPath);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(loadType), loadType, null);
        }

        return new Effect(vertex, fragment, constants);
    }

    public static Effect FromHlsl(string hlsl, string vertexEp = "VertexShader", string pixelEp = "PixelShader",
        EffectLoadType loadType = EffectLoadType.EmbeddedResource, SpecializationConstant[] constants = null,
        Assembly assembly = null)
    {
        byte[] vertex, pixel;

        switch (loadType)
        {
            case EffectLoadType.String:
                vertex = TryCompile(ShaderStage.Vertex, hlsl, vertexEp, Language.HLSL);
                pixel = TryCompile(ShaderStage.Pixel, hlsl, pixelEp, Language.HLSL);
                break;
            case EffectLoadType.File:
                string hlslFile = File.ReadAllText(hlsl);
                vertex = TryCompile(ShaderStage.Vertex, hlslFile, vertexEp, Language.HLSL);
                pixel = TryCompile(ShaderStage.Pixel, hlslFile, pixelEp, Language.HLSL);
                break;
            case EffectLoadType.EmbeddedResource:
                string hlslResx = Utils.LoadEmbeddedString(assembly ?? Assembly.GetExecutingAssembly(), hlsl, Encoding.UTF8);
                vertex = TryCompile(ShaderStage.Vertex, hlslResx, vertexEp, Language.HLSL);
                pixel = TryCompile(ShaderStage.Pixel, hlslResx, pixelEp, Language.HLSL);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(loadType), loadType, null);
        }

        return new Effect(vertex, pixel, constants);
    }
    
    public static Effect FromGlsl(string vertex, string fragment,
        EffectLoadType loadType = EffectLoadType.EmbeddedResource, SpecializationConstant[] constants = null,
        Assembly assembly = null)
    {
        byte[] vert, frag;

        switch (loadType)
        {
            case EffectLoadType.String:
                vert = TryCompile(ShaderStage.Vertex, vertex, "main", Language.GLSL);
                frag = TryCompile(ShaderStage.Fragment, fragment, "main", Language.GLSL);
                break;
            case EffectLoadType.File:
                vert = TryCompile(ShaderStage.Vertex, File.ReadAllText(vertex), "main", Language.GLSL);
                frag = TryCompile(ShaderStage.Fragment, File.ReadAllText(fragment), "main", Language.GLSL);
                break;
            case EffectLoadType.EmbeddedResource:
                assembly ??= Assembly.GetExecutingAssembly();
                string vShader = Utils.LoadEmbeddedString(assembly, vertex, Encoding.UTF8);
                string fShader = Utils.LoadEmbeddedString(assembly, fragment, Encoding.UTF8);
                vert = TryCompile(ShaderStage.Vertex, vShader, "main", Language.GLSL);
                frag = TryCompile(ShaderStage.Fragment, fShader, "main", Language.GLSL);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(loadType), loadType, null);
        }

        return new Effect(vert, frag, constants);
    }

    public void Dispose()
    {
        PieShader.Dispose();
        Logger.Debug("Effect disposed.");
    }

    private static string PreProcess(string shader)
    {
        while (true)
        {
            string[] splitText = shader.Split('\n');

            bool hasIncluded = false;

            for (int i = 0; i < splitText.Length; i++)
            {
                string line = splitText[i];

                if (line.StartsWith("#include"))
                {
                    shader = shader.Replace(line, Utils.LoadEmbeddedString(Assembly.GetExecutingAssembly(), line[("#include ".Length)..].Trim().Trim('"')));
                    hasIncluded = true;
                }
            }

            if (hasIncluded) continue;
            return shader;
        }
    }

    private static byte[] TryCompile(ShaderStage stage, string text, string entryPoint, Language language)
    {
        CompilerResult result = Compiler.ToSpirv((Stage) stage, language, Encoding.UTF8.GetBytes(text), entryPoint);
        if (!result.IsSuccess)
            throw new EaselException(result.Error);

        return result.Result;
    }

    [Conditional("DEBUG")]
    private void LogShader(ShaderStage stage, string shader)
    {
        string[] shaderLines = shader.Split('\n');
        StringBuilder builder = new StringBuilder($"{stage} shader:\n");
        for (int i = 0; i < shaderLines.Length; i++)
            builder.AppendLine(i + 1 + ": " + shaderLines[i]);
        Logger.Debug(builder.ToString());
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