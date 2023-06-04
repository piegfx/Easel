using System.Numerics;

namespace Easel.Graphics.Models;

public struct Mesh
{
    public VertexPositionTextureNormalTangent[] Vertices;
    public uint[] Indices;
    public Matrix4x4 Transform;

    public Mesh(VertexPositionTextureNormalTangent[] vertices, uint[] indices, Matrix4x4 transform)
    {
        Vertices = vertices;
        Indices = indices;
        Transform = transform;
    }
}