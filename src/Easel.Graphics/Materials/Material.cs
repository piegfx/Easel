using System;
using Easel.Math;

namespace Easel.Graphics.Materials;

public class Material : IDisposable
{
    public Texture2D AlbedoTexture;
    public Texture2D NormalTexture;
    public Texture2D MetallicTexture;
    public Texture2D RoughnessTexture;
    public Texture2D AmbientOcclusionTexture;
    public Texture2D EmissiveTexture;

    public Color AlbedoColor;

    public Material(in MaterialDescription description)
    {
        AlbedoTexture = description.AlbedoTexture;
        NormalTexture = description.NormalTexture;
        MetallicTexture = description.MetallicTexture;
        RoughnessTexture = description.RoughnessTexture;
        AmbientOcclusionTexture = description.AmbientOcclusionTexture;
        EmissiveTexture = description.EmissiveTexture;

        AlbedoColor = description.AlbedoColor;
    }

    internal Renderers.Structs.Material ShaderMaterial => new Renderers.Structs.Material()
    {
        AlbedoMultiplier = AlbedoColor
    };

    public void Dispose()
    {
        
    }
}