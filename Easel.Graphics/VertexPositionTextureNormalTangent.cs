using System.Numerics;
using Easel.Math;

namespace Easel.Graphics;

public struct VertexPositionTextureNormalTangent
{
    public Vector3T<float> Position;
    public Vector2T<float> TexCoords;
    public Vector3T<float> Normals;
    public Vector3T<float> Tangents;

    public VertexPositionTextureNormalTangent(Vector3T<float> position, Vector2T<float> texCoords, Vector3T<float> normals, Vector3T<float> tangents)
    {
        Position = position;
        TexCoords = texCoords;
        Normals = normals;
        Tangents = tangents;
    }

    public const int SizeInBytes = 44;
}