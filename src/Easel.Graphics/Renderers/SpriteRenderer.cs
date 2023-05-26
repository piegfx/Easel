using System;
using System.Numerics;
using Easel.Core;
using Easel.Math;
using Pie;

namespace Easel.Graphics.Renderers;

/// <summary>
/// Provides fast sprite batching.
/// </summary>
public sealed class SpriteRenderer : IDisposable
{
    /// <summary>
    /// The maximum number of sprites the <see cref="SpriteRenderer"/> can render in a single draw call.
    /// Sprites above this number will be drawn, however will be split into a new draw call.
    /// </summary>
    public const uint MaxSprites = 1 << 14;

    private const uint NumVertices = 4;
    private const uint NumIndices = 6;

    private const uint MaxVertices = NumVertices * MaxSprites;
    private const uint MaxIndices = NumIndices * MaxSprites;
    
    private GraphicsDevice _device;

    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _indexBuffer;

    private SpriteMatrices _spriteMatrices;
    private GraphicsBuffer _spriteMatricesBuffer;

    private SpriteVertex[] _vertices;
    private uint[] _indices;

    private DepthStencilState _depthStencilState;
    private RasterizerState _rasterizerState;

    private uint _currentSprite;
    private Texture2D _currentTexture;

    private bool _hasBegun;

    public bool HasBegun => _hasBegun;

    /// <summary>
    /// Create a new <see cref="SpriteRenderer"/>.
    /// </summary>
    /// <param name="device">The <see cref="GraphicsDevice"/> to use.</param>
    public SpriteRenderer(GraphicsDevice device)
    {
        _device = device;

        _vertices = new SpriteVertex[MaxVertices];
        _indices = new uint[MaxIndices];

        _vertexBuffer = device.CreateBuffer(BufferType.VertexBuffer, _vertices, true);
        _indexBuffer = device.CreateBuffer(BufferType.IndexBuffer, _indices, true);

        _spriteMatrices = new SpriteMatrices()
        {
            Projection = Matrix4x4.Identity,
            Transform = Matrix4x4.Identity
        };

        _depthStencilState = device.CreateDepthStencilState(DepthStencilStateDescription.Disabled);
        _rasterizerState = device.CreateRasterizerState(RasterizerStateDescription.CullNone);
    }

    public void Begin(Matrix4x4? transform = null, Matrix4x4? projection = null)
    {
        if (_hasBegun)
            throw new EaselException("A batch has already begun!");

        Rectangle<int> viewport = (Rectangle<int>) _device.Viewport;
        _spriteMatrices.Projection = projection ??
                                     Matrix4x4.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, -1, 1);

        _spriteMatrices.Transform = transform ?? Matrix4x4.Identity;
        
        _device.UpdateBuffer(_spriteMatricesBuffer, 0, _spriteMatrices);
    }

    public void End()
    {
        if (!_hasBegun)
            throw new EaselException("No batch has begun!");

        Flush();
    }

    public void DrawSprite(Texture2D texture, Vector2 position, Rectangle<int>? source, Color tint)
    {
        if (_currentTexture != texture || _currentSprite >= MaxSprites)
            Flush();

        _currentTexture = texture;
        
        
    }

    private void Flush()
    {
        
    }

    public void Dispose()
    {
        
    }

    public struct SpriteVertex
    {
        public Vector2T<float> Position;
        public Vector2T<float> TexCoord;
        public Color Tint;

        public SpriteVertex(Vector2T<float> position, Vector2T<float> texCoord, Color tint)
        {
            Position = position;
            TexCoord = texCoord;
            Tint = tint;
        }
    }

    private struct SpriteMatrices
    {
        public Matrix4x4 Projection;
        public Matrix4x4 Transform;
    }
}