using System.Numerics;

namespace Easel.Graphics.Structs;

public struct VertexPositionTextureNormalTangent
{
    public Vector3 Position;
    public Vector2 TexCoord;
    public Vector3 Normal;
    public Vector3 Tangent;

    public VertexPositionTextureNormalTangent(Vector3 position, Vector2 texCoord, Vector3 normal, Vector3 tangent)
    {
        Position = position;
        TexCoord = texCoord;
        Normal = normal;
        Tangent = tangent;
    }
}