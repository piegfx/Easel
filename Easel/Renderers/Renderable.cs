using System.Numerics;
using Easel.Graphics;
using Pie;

namespace Easel.Renderers;

/// <summary>
/// Represents a renderable object that can be used in various renderers.
/// </summary>
public struct Renderable
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
    /// The texture of this renderable.
    /// </summary>
    public TextureObject Texture;

    /// <summary>
    /// The model matrix for this renderable.
    /// </summary>
    public Matrix4x4 ModelMatrix;

    // TODO: Materials
    public Vector2 TilingAmount;

    /// <summary>
    /// Create a new renderable object.
    /// </summary>
    /// <param name="vertexBuffer">The vertex buffer of this renderable.</param>
    /// <param name="indexBuffer">The index buffer of this renderable.</param>
    /// <param name="indicesLength">The number of indices in the <see cref="IndexBuffer"/>.</param>
    /// <param name="texture">The texture of this renderable.</param>
    /// <param name="modelMatrix">The model matrix for this renderable.</param>
    public Renderable(GraphicsBuffer vertexBuffer, GraphicsBuffer indexBuffer, uint indicesLength, TextureObject texture, Matrix4x4 modelMatrix, Vector2 tilingAmount)
    {
        VertexBuffer = vertexBuffer;
        IndexBuffer = indexBuffer;
        IndicesLength = indicesLength;
        Texture = texture;
        ModelMatrix = modelMatrix;
        TilingAmount = tilingAmount;
    }
}