using System;
using Easel.Graphics.Materials;
using Pie;

namespace Easel.Graphics.Renderers;

public sealed class Renderable : IDisposable
{
    public GraphicsBuffer VertexBuffer;
    public GraphicsBuffer IndexBuffer;
    public uint NumIndices;

    public Material Material;

    public Renderable(VertexPositionTextureNormalTangent[] vertices, uint[] indices, Material material)
    {
        GraphicsDevice device = Renderer.Instance.Device;

        VertexBuffer = device.CreateBuffer(BufferType.VertexBuffer, vertices);
        IndexBuffer = device.CreateBuffer(BufferType.IndexBuffer, indices);
        NumIndices = (uint) indices.Length;

        Material = material;
    }

    public Renderable(GraphicsBuffer vertexBuffer, GraphicsBuffer indexBuffer, uint numIndices, Material material)
    {
        VertexBuffer = vertexBuffer;
        IndexBuffer = indexBuffer;
        NumIndices = numIndices;
        Material = material;
    }

    public void Dispose()
    {
        VertexBuffer.Dispose();
        IndexBuffer.Dispose();
    }
}