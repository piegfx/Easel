using System;
using System.Numerics;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.Primitives;
using Pie;
using Pie.Utils;

namespace Easel.Entities.Components;

/// <summary>
/// The bog-standard 3D mesh renderer for an entity.
/// </summary>
public class MeshRenderer : Component
{
    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _indexBuffer;

    private Renderable _renderable;

    private VertexPositionTextureNormal[] _vertices;
    private uint[] _indices;

    public readonly Material Material;

    /// <summary>
    /// Create a new <see cref="MeshRenderer"/> instance with the given <see cref="IPrimitive"/> and <see cref="Texture2D"/>
    /// </summary>
    /// <param name="primitive">The <see cref="IPrimitive"/> mesh.</param>
    /// <param name="texture">The <see cref="Texture2D"/> to apply to the mesh.</param>
    public MeshRenderer(IPrimitive primitive, Material material)
    {
        // TODO: Add materials.
        _vertices = primitive.Vertices;
        _indices = primitive.Indices;
        Material = material;
    }

    protected internal override void Initialize()
    {
        base.Initialize();

        _vertexBuffer = Graphics.PieGraphics.CreateBuffer(BufferType.VertexBuffer, _vertices);
        _indexBuffer = Graphics.PieGraphics.CreateBuffer(BufferType.IndexBuffer, _indices);

        _renderable = new Renderable(_vertexBuffer, _indexBuffer, (uint) _indices.Length, Matrix4x4.Identity, Material);
    }

    protected internal override void Draw()
    {
        base.Draw();

        _renderable.ModelMatrix = Transform.ModelMatrix * (Entity.Parent?.Transform.ModelMatrix ?? Matrix4x4.Identity);
        ForwardRenderer.DrawOpaque(_renderable);
    }

    public override void Dispose()
    {
        base.Dispose();
        
        _indexBuffer.Dispose();
        _vertexBuffer.Dispose();
    }
}