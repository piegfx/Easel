using System;
using System.Numerics;
using Pie;

namespace Easel.Graphics.Renderers;

/// <summary>
/// Represents a renderable object that can be used in various renderers.
/// </summary>
public struct Renderable : IDisposable
{
    /// <summary>
    /// The vertex buffer of this renderable.
    /// </summary>
    public GraphicsBuffer VertexBuffer;
    
    /// <summary>
    /// The index buffer of this renderable.
    /// </summary>
    public GraphicsBuffer IndexBuffer;
    
    /// <summary>
    /// The number of indices in the <see cref="IndexBuffer"/>.
    /// </summary>
    public uint IndicesLength;

    /// <summary>
    /// The model matrix for this renderable.
    /// </summary>
    public Matrix4x4 ModelMatrix;

    /// <summary>
    /// The material of this renderable.
    /// </summary>
    public Material Material;

    /// <summary>
    /// Create a new renderable object.
    /// </summary>
    /// <param name="vertexBuffer">The vertex buffer of this renderable.</param>
    /// <param name="indexBuffer">The index buffer of this renderable.</param>
    /// <param name="indicesLength">The number of indices in the <see cref="IndexBuffer"/>.</param>
    /// <param name="material">The material of this renderable.</param>
    /// <param name="modelMatrix">The model matrix for this renderable.</param>
    public Renderable(GraphicsBuffer vertexBuffer, GraphicsBuffer indexBuffer, uint indicesLength, Matrix4x4 modelMatrix, Material material)
    {
        VertexBuffer = vertexBuffer;
        IndexBuffer = indexBuffer;
        IndicesLength = indicesLength;
        ModelMatrix = modelMatrix;
        Material = material;
    }

    public void Dispose()
    {
        VertexBuffer.Dispose();
        IndexBuffer.Dispose();
    }
}