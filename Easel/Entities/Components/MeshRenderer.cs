using System;
using System.Numerics;
using Easel.Graphics;
using Easel.Primitives;
using Easel.Renderers;
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
    private Texture2D _texture;

    private Vector2 _tilingAmount;
    public Vector2 TilingAmount
    {
        get => _tilingAmount;
        set
        {
            _tilingAmount = value;
            _renderable.TilingAmount = value;
        }
    }

    /// <summary>
    /// Create a new <see cref="MeshRenderer"/> instance with the given <see cref="IPrimitive"/> and <see cref="Texture2D"/>
    /// </summary>
    /// <param name="primitive">The <see cref="IPrimitive"/> mesh.</param>
    /// <param name="texture">The <see cref="Texture2D"/> to apply to the mesh.</param>
    public MeshRenderer(IPrimitive primitive, Texture2D texture, Vector2? tilingAmount = null)
    {
        // TODO: Add materials.
        _vertices = primitive.Vertices;
        _indices = primitive.Indices;
        _texture = texture;
        _tilingAmount = tilingAmount ?? Vector2.One;
    }

    protected internal override void Initialize()
    {
        base.Initialize();

        _vertexBuffer = Graphics.PieGraphics.CreateBuffer(BufferType.VertexBuffer, _vertices);
        _indexBuffer = Graphics.PieGraphics.CreateBuffer(BufferType.IndexBuffer, _indices);

        _renderable = new Renderable(_vertexBuffer, _indexBuffer, (uint) _indices.Length, _texture, Matrix4x4.Identity, _tilingAmount);
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