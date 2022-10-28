using Easel.Utilities;

namespace Easel.Primitives;

public struct Quad : IPrimitive
{
    public VertexPositionTextureNormalTangent[] Vertices { get; }
    public uint[] Indices { get; }
}