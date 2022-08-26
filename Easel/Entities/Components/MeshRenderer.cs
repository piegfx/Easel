using System;
using System.Numerics;
using Easel.Graphics;
using Easel.Primitives;
using Easel.Renderers;
using Pie;
using Pie.Utils;

namespace Easel.Entities.Components;

public class MeshRenderer : Component
{
    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _indexBuffer;

    private Renderable _renderable;

    private VertexPositionTextureNormal[] _vertices;
    private uint[] _indices;
    private Texture2D _texture;

    public MeshRenderer(IPrimitive primitive, Texture2D texture)
    {
        _vertices = primitive.Vertices;
        _indices = primitive.Indices;
        _texture = texture;
    }

    protected internal override void Initialize()
    {
        base.Initialize();

        _vertexBuffer = Graphics.PieGraphics.CreateBuffer(BufferType.VertexBuffer, _vertices);
        _indexBuffer = Graphics.PieGraphics.CreateBuffer(BufferType.IndexBuffer, _indices);

        _renderable = new Renderable(_vertexBuffer, _indexBuffer, (uint) _indices.Length, _texture, Matrix4x4.Identity);
    }

    protected internal override void Draw()
    {
        base.Draw();

        _renderable.ModelMatrix = Transform.ModelMatrix;
        ForwardRenderer.DrawOpaque(_renderable);
    }

    public override void Dispose()
    {
        base.Dispose();
        
        _indexBuffer.Dispose();
        _vertexBuffer.Dispose();
    }
}