using Easel.Graphics.Materials;
using Pie;

namespace Easel.Graphics.Renderers;

public class Renderable
{
    public GraphicsBuffer VertexBuffer;
    public GraphicsBuffer IndexBuffer;

    public uint NumIndices;

    public Material Material;

    public Renderable(GraphicsBuffer vertexBuffer, GraphicsBuffer indexBuffer, uint numIndices, Material material)
    {
        VertexBuffer = vertexBuffer;
        IndexBuffer = indexBuffer;
        NumIndices = numIndices;
        Material = material;
    }

    public static Renderable Create<TVertex>(TVertex[] vertices, uint[] indices, uint numIndices, Material material) where TVertex : unmanaged
    {
        GraphicsDevice device = Renderer.Instance.Device;

        GraphicsBuffer vertexBuffer = device.CreateBuffer(BufferType.VertexBuffer, vertices);
        GraphicsBuffer indexBuffer = device.CreateBuffer(BufferType.IndexBuffer, indices);

        return new Renderable(vertexBuffer, indexBuffer, numIndices, material);
    }
}