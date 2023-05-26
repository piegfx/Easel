using System;
using System.Numerics;
using System.Reflection;
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

    private Shader _shader;
    private InputLayout _layout;

    private DepthStencilState _depthStencilState;
    private RasterizerState _rasterizerState;

    // TODO: Easy to use sampler states.
    private SamplerState _samplerState;

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

        _spriteMatricesBuffer = device.CreateBuffer(BufferType.UniformBuffer, _spriteMatrices, true);

        const string vShader = Renderer.ShaderNamespace + ".Sprite.Sprite_vert.spv";
        const string fShader = Renderer.ShaderNamespace + ".Sprite.Sprite_frag.spv";
        Assembly assembly = Assembly.GetExecutingAssembly();

        _shader = device.CreateShader(new[]
        {
            new ShaderAttachment(ShaderStage.Vertex, Utils.LoadEmbeddedResource(assembly, vShader)),
            new ShaderAttachment(ShaderStage.Fragment, Utils.LoadEmbeddedResource(assembly, fShader))
        });

        _layout = device.CreateInputLayout(new[]
        {
            new InputLayoutDescription(Format.R32G32_Float, 0, 0, InputType.PerVertex), // position
            new InputLayoutDescription(Format.R32G32_Float, 8, 0, InputType.PerVertex), // texCoord
            new InputLayoutDescription(Format.R32G32B32A32_Float, 16, 0, InputType.PerVertex) // tint
        });

        _depthStencilState = device.CreateDepthStencilState(DepthStencilStateDescription.Disabled);
        _rasterizerState = device.CreateRasterizerState(RasterizerStateDescription.CullNone);

        _samplerState = device.CreateSamplerState(SamplerStateDescription.LinearClamp);
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

        _hasBegun = true;
    }

    public void End()
    {
        if (!_hasBegun)
            throw new EaselException("No batch has begun!");

        Flush();

        _hasBegun = false;
    }

    public void DrawSprite(Texture2D texture, Vector2 position, Rectangle<int>? source, Color tint, float rotation, Vector2 scale, Vector2 origin, Flip flip = Flip.None)
    {
        if (!_hasBegun)
            throw new EaselException("No batch has begun!");
        
        if (_currentTexture != texture || _currentSprite >= MaxSprites)
            Flush();

        _currentTexture = texture;

        Size<int> texSize = texture.Size;

        Rectangle<int> spriteRect = source ?? new Rectangle<int>(Vector2T<int>.Zero, texSize);

        float x = position.X;
        float y = position.Y;
        float w = spriteRect.Width * scale.X;
        float h = spriteRect.Height * scale.Y;

        float texX = spriteRect.X / (float) texSize.Width;
        float texY = spriteRect.Y / (float) texSize.Height;
        float texW = spriteRect.Width / (float) texSize.Width;
        float texH = spriteRect.Height / (float) texSize.Height;

        // TODO: I've just realized that these flip the entire image, and that can scr
        switch (flip)
        {
            case Flip.None:
                break;
            case Flip.FlipX:
                texX = 1 - texX;
                texW *= -1;
                break;
            case Flip.FlipY:
                texY = 1 - texY;
                texH *= -1;
                break;
            case Flip.FlipXY:
                texX = 1 - texX;
                texW *= -1;
                texY = 1 - texY;
                texH *= -1;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(flip), flip, null);
        }

        uint currentVertex = _currentSprite * NumVertices;
        uint currentIndex = _currentSprite * NumIndices;

        _vertices[currentVertex + 0] = new SpriteVertex(new Vector2T<float>(x, y), new Vector2T<float>(texX, texY), tint);
        _vertices[currentVertex + 1] = new SpriteVertex(new Vector2T<float>(x + w, y), new Vector2T<float>(texX + texW, texY), tint);
        _vertices[currentVertex + 2] = new SpriteVertex(new Vector2T<float>(x + w, y + h), new Vector2T<float>(texX + texW, texY + texH), tint);
        _vertices[currentVertex + 3] = new SpriteVertex(new Vector2T<float>(x, y + h), new Vector2T<float>(texX, texY + texH), tint);

        _indices[currentIndex + 0] = 0 + currentVertex;
        _indices[currentIndex + 1] = 1 + currentVertex;
        _indices[currentIndex + 2] = 3 + currentVertex;
        _indices[currentIndex + 3] = 1 + currentVertex;
        _indices[currentIndex + 4] = 2 + currentVertex;
        _indices[currentIndex + 5] = 3 + currentVertex;

        _currentSprite++;
    }

    private void Flush()
    {
        if (_currentSprite == 0)
            return;

        // Map the buffers - this is significantly faster than using Device.UpdateBuffer.
        IntPtr vptr = _device.MapBuffer(_vertexBuffer, MapMode.Write);
        // We set the data length to the number of sprites in memory, no more. This ensures that the data transfer is
        // as fast as possible.
        PieUtils.CopyToUnmanaged(vptr, 0, _currentSprite * NumVertices * SpriteVertex.SizeInBytes, _vertices);
        _device.UnmapBuffer(_vertexBuffer);

        IntPtr iptr = _device.MapBuffer(_indexBuffer, MapMode.Write);
        PieUtils.CopyToUnmanaged(iptr, 0, _currentSprite * NumIndices * sizeof(uint), _indices);
        _device.UnmapBuffer(_indexBuffer);
        
        _device.SetPrimitiveType(PrimitiveType.TriangleList);
        _device.SetShader(_shader);
        _device.SetDepthStencilState(_depthStencilState);
        _device.SetRasterizerState(_rasterizerState);
        _device.SetUniformBuffer(0, _spriteMatricesBuffer);
        _device.SetTexture(1, _currentTexture.PieTexture, _samplerState);
        _device.SetVertexBuffer(0, _vertexBuffer, SpriteVertex.SizeInBytes, _layout);
        _device.SetIndexBuffer(_indexBuffer, IndexType.UInt);
        _device.DrawIndexed(_currentSprite * NumIndices);

        _currentSprite = 0;
    }

    public void Dispose()
    {
        _vertexBuffer.Dispose();
        _indexBuffer.Dispose();
        _spriteMatricesBuffer.Dispose();
        
        _shader.Dispose();
        _layout.Dispose();
        
        _depthStencilState.Dispose();
        _rasterizerState.Dispose();
        
        _samplerState.Dispose();
    }

    public enum Flip
    {
        None,
        FlipX,
        FlipY,
        FlipXY
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

        public const uint SizeInBytes = 32;
    }

    private struct SpriteMatrices
    {
        public Matrix4x4 Projection;
        public Matrix4x4 Transform;
    }
}