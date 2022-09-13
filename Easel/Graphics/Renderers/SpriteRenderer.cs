using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using Easel.Graphics;
using Easel.Math;
using Easel.Utilities;
using Pie;
using Pie.ShaderCompiler;
using Pie.Utils;
using Color = Easel.Math.Color;

namespace Easel.Renderers;

/// <summary>
/// Efficiently batches and renders 2D sprites.
/// </summary>
public static class SpriteRenderer
{
    // Basic sprite renderer
    // TODO: Make a proper sprite batcher once pie is ready
    
    private static VertexPositionTexture[] _vertices = new[]
    {
        new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 1)),
        new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(1, 0)),
        new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 0)),
        new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(0, 1))
    };

    private static uint[] _indices = new[]
    {
        0u, 1u, 3u,
        1u, 2u, 3u
    };

    private const uint NumVertices = 4;
    private const uint NumIndices = 6;

    public const uint MaxSprites = 512;
    private const uint VertexSizeInBytes = NumVertices * SpriteVertex.SizeInBytes;
    private const uint IndicesSizeInBytes = NumIndices * sizeof(uint);

    private static SpriteVertex[] _verticesCache;
    private static uint[] _indicesCache;

    private static List<Sprite> _sprites;
    private static uint _spriteCount;
    private static uint _drawCount;

    private static GraphicsBuffer _vertexBuffer;
    private static GraphicsBuffer _indexBuffer;
    
    private static GraphicsBuffer _projViewBuffer;

    private static Shader _shader;
    private static InputLayout _layout;
    private static RasterizerState _rasterizerState;
    private static DepthState _depthState;
    private static BlendState _blendState;
    private static SamplerState _samplerState;

    private static GraphicsDevice _device;

    private static bool _begun;

    private static TextureObject _currentTexture;

    static SpriteRenderer()
    {
        _device = EaselGame.Instance.GraphicsInternal.PieGraphics;

        _verticesCache = new SpriteVertex[NumVertices];
        _indicesCache = new uint[NumIndices];
        _sprites = new List<Sprite>();

        _vertexBuffer = _device.CreateBuffer<SpriteVertex>(BufferType.VertexBuffer, MaxSprites * VertexSizeInBytes, null, true);
        _indexBuffer = _device.CreateBuffer<uint>(BufferType.IndexBuffer, MaxSprites * IndicesSizeInBytes, null, true);
        
        _projViewBuffer = _device.CreateBuffer(BufferType.UniformBuffer, Matrix4x4.Identity, true);

        _shader = _device.CreateCrossPlatformShader(
            new ShaderAttachment(ShaderStage.Vertex, Utils.LoadEmbeddedString("Easel.Graphics.Shaders.SpriteRenderer.Sprite.vert")),
            new ShaderAttachment(ShaderStage.Fragment,
                Utils.LoadEmbeddedString("Easel.Graphics.Shaders.SpriteRenderer.Sprite.frag")));

        _layout = _device.CreateInputLayout(
            new InputLayoutDescription("aPosition", AttributeType.Vec2),
            new InputLayoutDescription("aTexCoords", AttributeType.Vec2),
            new InputLayoutDescription("aTint", AttributeType.Vec4),
            new InputLayoutDescription("aRotation", AttributeType.Float),
            new InputLayoutDescription("aOrigin", AttributeType.Vec2),
            new InputLayoutDescription("aScale", AttributeType.Vec2));

        _rasterizerState = _device.CreateRasterizerState(RasterizerStateDescription.CullNone);
        _depthState = _device.CreateDepthState(DepthStateDescription.Disabled);
        _blendState = _device.CreateBlendState(BlendStateDescription.NonPremultiplied);
        _samplerState = _device.CreateSamplerState(SamplerStateDescription.LinearRepeat);
    }

    public static void Begin(Matrix4x4? transform = null)
    {
        if (_begun)
            throw new EaselException("SpriteRenderer session is already active.");
        _begun = true;

        Rectangle viewport = EaselGame.Instance.GraphicsInternal.Viewport;
        Matrix4x4 projection = Matrix4x4.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, -1f, 1f);
        _device.UpdateBuffer(_projViewBuffer, 0, (transform ?? Matrix4x4.Identity) * projection);
    }

    public static void Draw(TextureObject texture, Rectangle destination, Color tint)
    {
        Draw(texture, (Vector2) destination.Location, null, tint, 0, Vector2.Zero, (Vector2) destination.Size / (Vector2) texture.Size);
    }

    public static void Draw(TextureObject texture, Rectangle destination, Rectangle? source, Color tint)
    {
        Draw(texture, (Vector2) destination.Location, source, tint, 0, Vector2.Zero, (Vector2) destination.Size / (Vector2) texture.Size);
    }

    public static void Draw(TextureObject texture, Rectangle destination, Rectangle? source, Color tint, float rotation,
        Vector2 origin, SpriteFlip flip = SpriteFlip.None)
    {
        Draw(texture, (Vector2) destination.Location, source, tint, rotation, origin, (Vector2) destination.Size / (Vector2) texture.Size, flip);
    }
    
    public static void Draw(TextureObject texture, Vector2 position)
    {
        Draw(texture, position, null, Color.White, 0, Vector2.Zero, Vector2.One);
    }

    public static void Draw(TextureObject texture, Vector2 position, Color tint)
    {
        Draw(texture, position, null, tint, 0, Vector2.Zero, Vector2.One);
    }
    
    public static void Draw(TextureObject texture, Vector2 position, Rectangle? source, Color tint)
    {
        Draw(texture, position, source, tint, 0, Vector2.Zero, Vector2.One);
    }

    public static void Draw(TextureObject texture, Vector2 position, Rectangle? source, Color tint, float rotation,
        Vector2 origin, float scale, SpriteFlip flip = SpriteFlip.None)
    {
        Draw(texture, position, source, tint, rotation, origin, new Vector2(scale), flip);
    }

    public static void Draw(TextureObject texture, Vector2 position, Rectangle? source, Color tint, float rotation, Vector2 origin, Vector2 scale, SpriteFlip flip = SpriteFlip.None)
    {
        if (!_begun)
            throw new EaselException("No current active sprite renderer session.");
        _sprites.Add(new Sprite(texture, position, source, tint, rotation, origin, scale, flip));
        _spriteCount++;
    }

    public static void End()
    {
        if (!_begun)
            throw new EaselException("No current active sprite renderer session.");
        _begun = false;

        for (int i = 0; i < _spriteCount; i++)
        {
            DrawSprite(_sprites[i]);
        }

        _spriteCount = 0;
        
        _sprites.Clear();
        
        Flush();
    }

    private static void DrawSprite(Sprite sprite)
    {
        // TODO: Remove maximum sprites and implement buffer resizing
        if (sprite.Texture != _currentTexture || _drawCount >= MaxSprites)
            Flush();
        _currentTexture = sprite.Texture;

        Rectangle source = sprite.Source ?? new Rectangle(Point.Zero, sprite.Texture.Size);

        int rectX = source.X;
        int rectY = source.Y;
        int rectWidth = source.Width;
        int rectHeight = source.Height;
        
        float width = sprite.Texture.Size.Width;
        float height = sprite.Texture.Size.Height;
        
        sprite.Position -= sprite.Origin * sprite.Scale;
        sprite.Origin += sprite.Position / sprite.Scale;
        float posX = sprite.Position.X;
        float posY = sprite.Position.Y;

        float texX = rectX / width;
        float texY = rectY / height;
        float texW = rectWidth / width;
        float texH = rectHeight / height;

        bool isRenderTarget = _currentTexture is RenderTarget;
        if (isRenderTarget && sprite.Flip != SpriteFlip.FlipY)
            sprite.Flip = SpriteFlip.FlipY;
        else if (isRenderTarget && sprite.Flip == SpriteFlip.FlipY)
            sprite.Flip = SpriteFlip.None;

        switch (sprite.Flip)
        {
            case SpriteFlip.None:
                break;
            case SpriteFlip.FlipX:
                texW = -texW;
                texX = texW - texX;
                break;
            case SpriteFlip.FlipY:
                texH = -texH;
                texY = texH - texY;
                break;
            case SpriteFlip.FlipXY:
                texW = -texW;
                texX = texW - texX;
                texH = -texH;
                texY = texH - texY;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        width = rectWidth * sprite.Scale.X;
        height = rectHeight * sprite.Scale.Y;

        Color tint = sprite.Tint;
        float rotation = sprite.Rotation;
        Vector2 origin = sprite.Origin;
        Vector2 scale = sprite.Scale;

        _verticesCache[0] = new SpriteVertex(new Vector2(posX + width, posY + height), new Vector2(texX + texW, texY + texH), tint, rotation, origin, scale);
        _verticesCache[1] = new SpriteVertex(new Vector2(posX + width, posY), new Vector2(texX + texW, texY), tint, rotation, origin, scale);
        _verticesCache[2] = new SpriteVertex(new Vector2(posX, posY), new Vector2(texX, texY), tint, rotation, origin, scale);
        _verticesCache[3] = new SpriteVertex(new Vector2(posX, posY + height), new Vector2(texX, texY + texH), tint, rotation, origin, scale);

        uint dc = _drawCount * 4;
        _indicesCache[0] = 0u + dc;
        _indicesCache[1] = 1u + dc;
        _indicesCache[2] = 3u + dc;
        _indicesCache[3] = 1u + dc;
        _indicesCache[4] = 2u + dc;
        _indicesCache[5] = 3u + dc;
        
        // TODO: Implement array
        _device.UpdateBuffer(_vertexBuffer, _drawCount * VertexSizeInBytes, _verticesCache);
        _device.UpdateBuffer(_indexBuffer, _drawCount * IndicesSizeInBytes, _indicesCache);

        _drawCount++;
    }

    private static void Flush()
    {
        if (_drawCount == 0)
            return;
        
        _device.SetShader(_shader);
        _device.SetRasterizerState(_rasterizerState);
        _device.SetDepthState(_depthState);
        _device.SetBlendState(_blendState);
        _device.SetUniformBuffer(0, _projViewBuffer);
        _device.SetTexture(1, _currentTexture.PieTexture, _samplerState);
        _device.SetPrimitiveType(PrimitiveType.TriangleList);
        _device.SetVertexBuffer(_vertexBuffer, _layout);
        _device.SetIndexBuffer(_indexBuffer);
        _device.Draw(NumIndices * _drawCount);

        _drawCount = 0;
    }

    private struct Sprite
    {
        public TextureObject Texture;
        public Vector2 Position;
        public Rectangle? Source;
        public Color Tint;
        public float Rotation;
        public Vector2 Origin;
        public Vector2 Scale;
        public SpriteFlip Flip;

        public Sprite(TextureObject texture, Vector2 position, Rectangle? source, Color tint, float rotation, Vector2 origin, Vector2 scale, SpriteFlip flip)
        {
            Texture = texture;
            Position = position;
            Source = source;
            Tint = tint;
            Rotation = rotation;
            Origin = origin;
            Scale = scale;
            Flip = flip;
        }
    }

    public struct SpriteVertex
    {
        public Vector2 Position;
        public Vector2 TexCoord;
        public Color Tint;
        public float Rotation;
        public Vector2 Origin;
        public Vector2 Scale;

        public SpriteVertex(Vector2 position, Vector2 texCoord, Color tint, float rotation, Vector2 origin, Vector2 scale)
        {
            Position = position;
            TexCoord = texCoord;
            Tint = tint;
            Rotation = rotation;
            Origin = origin;
            Scale = scale;
        }

        public const uint SizeInBytes = 52;
    }
}

public enum SpriteFlip
{
    None,
    FlipX,
    FlipY,
    FlipXY
}