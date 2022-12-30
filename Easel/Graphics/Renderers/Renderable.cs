using System.Collections.Generic;
using Easel.Utilities;
using Pie;

namespace Easel.Graphics.Renderers;

public struct Renderable
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

    public static Renderable CreateFromMesh(in Mesh mesh)
    {
        GraphicsDevice device = EaselGame.Instance.GraphicsInternal.PieGraphics;

        GraphicsBuffer vertexBuffer = device.CreateBuffer(BufferType.VertexBuffer, mesh.Vertices);
        GraphicsBuffer indexBuffer = device.CreateBuffer(BufferType.IndexBuffer, mesh.Indices);

        return new Renderable(vertexBuffer, indexBuffer, (uint) mesh.Indices.Length, mesh.Material);
    }

    public static Renderable[] CreateFromMeshes(Mesh[] meshes)
    {
        List<Renderable> renderables = new List<Renderable>(meshes.Length);
        
        for (int i = 0; i < meshes.Length; i++)
            renderables.Add(CreateFromMesh(meshes[i]));

        return renderables.ToArray();
    }
}