using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Reflection;
using Easel.Graphics;
using Pie;
using Pie.ShaderCompiler;
using Pie.Utils;

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

    public const uint MaxSprites = 512;
    public const uint VertexSizeInBytes = SpriteVertex.SizeInBytes * 4;
    public const uint IndicesSizeInBytes = 6 * sizeof(uint);

    private static Sprite[] _sprites;
    private static uint _spriteCount;
    private static uint _drawCount;

    private static GraphicsBuffer _vertexBuffer;
    private static GraphicsBuffer _indexBuffer;

    private static Matrix4x4 _projection;
    private static GraphicsBuffer _projViewBuffer;

    private static Shader _shader;
    private static InputLayout _layout;
    private static RasterizerState _rasterizerState;
    private static DepthState _depthState;
    private static BlendState _blendState;

    private static GraphicsDevice _device;

    private static bool _begun;

    private static TextureObject _currentTexture;

    static SpriteRenderer()
    {
        _device = EaselGame.Instance.GraphicsInternal.PieGraphics;
        
        _sprites = new Sprite[MaxSprites];

        _vertexBuffer = _device.CreateBuffer<SpriteVertex>(BufferType.VertexBuffer, MaxSprites * VertexSizeInBytes, null, true);
        _indexBuffer = _device.CreateBuffer<uint>(BufferType.IndexBuffer, MaxSprites * IndicesSizeInBytes, null, true);

        _projection = Matrix4x4.CreateOrthographicOffCenter(0, 1280, 720, 0, -1, 1);
        _projViewBuffer = _device.CreateBuffer(BufferType.UniformBuffer, _projection, true);

        _shader = _device.CreateCrossPlatformShader(
            new ShaderAttachment(ShaderStage.Vertex, File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Graphics/Shaders/SpriteRenderer/Sprite.vert"))),
            new ShaderAttachment(ShaderStage.Fragment,
                File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Graphics/Shaders/SpriteRenderer/Sprite.frag"))));

        _layout = _device.CreateInputLayout(new InputLayoutDescription("aPosition", AttributeType.Vec2),
            new InputLayoutDescription("aTexCoords", AttributeType.Vec2));

        _rasterizerState = _device.CreateRasterizerState(RasterizerStateDescription.CullNone);
        _depthState = _device.CreateDepthState(DepthStateDescription.Disabled);
        _blendState = _device.CreateBlendState(BlendStateDescription.NonPremultiplied);
    }

    public static void Begin(Matrix4x4? transform = null)
    {
        if (_begun)
            throw new EaselException("SpriteRenderer session is already active.");
        _begun = true;

        _device.UpdateBuffer(_projViewBuffer, 0, transform ?? Matrix4x4.Identity * _projection);
    }

    public static void Draw(TextureObject texture, Vector2 position)
    {
        if (!_begun)
            throw new EaselException("No current active sprite renderer session.");
        _sprites[_spriteCount++] = new Sprite(position, texture);
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
        
        Flush();
    }

    private static void DrawSprite(Sprite sprite)
    {
        _currentTexture = sprite.Texture;

        int width = sprite.Texture.Size.Width;
        int height = sprite.Texture.Size.Height;
        float posX = sprite.Position.X;
        float posY = sprite.Position.Y;
        
        SpriteVertex[] vertices = new SpriteVertex[]
        {
            new SpriteVertex(new Vector2(posX + width, posY + height), new Vector2(1, 1)),
            new SpriteVertex(new Vector2(posX + width, posY), new Vector2(1, 0)),
            new SpriteVertex(new Vector2(posX, posY), new Vector2(0, 0)),
            new SpriteVertex(new Vector2(posX, posY + height), new Vector2(0, 1))
        };

        uint dc = _drawCount * 4;
        uint[] indices = new uint[]
        {
            0u + dc, 1u + dc, 3u + dc,
            1u + dc, 2u + dc, 3u + dc
        };
        
        _device.UpdateBuffer(_vertexBuffer, _drawCount * VertexSizeInBytes, vertices);
        _device.UpdateBuffer(_indexBuffer, _drawCount * IndicesSizeInBytes, indices);

        _drawCount++;
    }

    private static void Flush()
    {
        _device.SetShader(_shader);
        _device.SetRasterizerState(_rasterizerState);
        _device.SetDepthState(_depthState);
        _device.SetBlendState(_blendState);
        _device.SetUniformBuffer(0, _projViewBuffer);
        _device.SetTexture(1, _currentTexture.PieTexture);
        _device.SetPrimitiveType(PrimitiveType.TriangleList);
        _device.SetVertexBuffer(_vertexBuffer, _layout);
        _device.SetIndexBuffer(_indexBuffer);
        _device.Draw(6 * _drawCount);

        _drawCount = 0;
    }

    private struct Sprite
    {
        public Vector2 Position;
        public TextureObject Texture;

        public Sprite(Vector2 position, TextureObject texture)
        {
            Position = position;
            Texture = texture;
        }
    }

    private struct SpriteVertex
    {
        public Vector2 Position;
        public Vector2 TexCoord;

        public SpriteVertex(Vector2 position, Vector2 texCoord)
        {
            Position = position;
            TexCoord = texCoord;
        }

        public const uint SizeInBytes = 16;
    }
}