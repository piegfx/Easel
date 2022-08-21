using System.Numerics;
using Easel.Graphics;
using Pie;

namespace Easel.Renderers;

public struct Renderable
{
    public GraphicsBuffer VertexBuffer;
    public GraphicsBuffer IndexBuffer;
    public uint IndicesLength;
    public TextureObject Texture;

    public Matrix4x4 ModelMatrix;

    public Renderable(GraphicsBuffer vertexBuffer, GraphicsBuffer indexBuffer, uint indicesLength, TextureObject texture, Matrix4x4 modelMatrix)
    {
        VertexBuffer = vertexBuffer;
        IndexBuffer = indexBuffer;
        IndicesLength = indicesLength;
        Texture = texture;
        ModelMatrix = modelMatrix;
    }
}