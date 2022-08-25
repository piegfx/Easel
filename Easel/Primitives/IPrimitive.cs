using Pie.Utils;

namespace Easel.Primitives;

public interface IPrimitive
{
    public VertexPositionTextureNormal[] Vertices { get; }
    
    public uint[] Indices { get; }
}