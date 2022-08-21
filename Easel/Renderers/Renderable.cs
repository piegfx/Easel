using Easel.Graphics;
using Pie;

namespace Easel.Renderers;

public struct Renderable
{
    public GraphicsBuffer VertexBuffer;
    public GraphicsBuffer IndexBuffer;
    public uint IndicesLength;
    public TextureObject Texture;

    public Renderable(GraphicsBuffer vertexBuffer, GraphicsBuffer indexBuffer, uint indicesLength, TextureObject texture)
    {
        VertexBuffer = vertexBuffer;
        IndexBuffer = indexBuffer;
        IndicesLength = indicesLength;
        Texture = texture;
    }
}