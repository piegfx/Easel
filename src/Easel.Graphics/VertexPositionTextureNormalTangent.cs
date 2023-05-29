using Easel.Math;

namespace Easel.Graphics;

public struct VertexPositionTextureNormalTangent
{
    public Vector3T<float> Position;
    public Vector2T<float> TexCoord;
    public Vector3T<float> Normal;
    public Vector3T<float> Tangent;

    public VertexPositionTextureNormalTangent(Vector3T<float> position, Vector2T<float> texCoord, Vector3T<float> normal, Vector3T<float> tangent)
    {
        Position = position;
        TexCoord = texCoord;
        Normal = normal;
        Tangent = tangent;
    }

    public const uint SizeInBytes = 44;
}