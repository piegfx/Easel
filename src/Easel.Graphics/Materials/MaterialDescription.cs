using Easel.Math;
using Pie;

namespace Easel.Graphics.Materials;

public struct MaterialDescription
{
    public Texture2D AlbedoTexture;
    public Texture2D NormalTexture;
    public Texture2D MetallicTexture;
    public Texture2D RoughnessTexture;
    public Texture2D AmbientOcclusionTexture;
    public Texture2D EmissiveTexture;

    public Color AlbedoColor;

    public bool CastShadows;
    public bool ReceiveShadows;

    public PrimitiveType PrimitiveType;
    public AlphaMode AlphaMode;
}