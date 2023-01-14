using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Easel.Math;
using Easel.Utilities;
using Pie;
using Pie.ShaderCompiler;
using Color = Easel.Math.Color;

namespace Easel.Graphics.Renderers;

/// <summary>
/// Efficiently batches and renders 2D sprites.
/// </summary>
public sealed class SpriteRenderer : IDisposable
{
    private const uint NumVertices = 4;
    private const uint NumIndices = 6;

    public const uint MaxSprites = 512;
    private const uint VertexSizeInBytes = NumVertices * SpriteVertex.SizeInBytes;
    private const uint IndicesSizeInBytes = NumIndices * sizeof(uint);

    private const uint MaxVertices = NumVertices * MaxSprites;
    private const uint MaxIndices = NumIndices * MaxSprites;

    private uint _totalVertices;
    private uint _totalIndices;

    private SpriteVertex[] _vertices;

    private uint[] _indices;

    private SpriteVertex[] _verticesCache;
    private uint[] _indicesCache;

    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _indexBuffer;
    
    private GraphicsBuffer _projViewBuffer;

    private Effect _spriteEffect;
    private Effect _roundedRectEffect;
    private Effect _effectToUse;
    private InputLayout _layout;
    private Pie.RasterizerState _rasterizerState;
    private DepthState _depthState;
    private BlendState _blendState;

    private GraphicsDevice _device;

    private bool _begun;

    private Texture _currentTexture;
    private SpriteType _currentType;

    public SpriteRenderer(GraphicsDevice device)
    {
        _device = device;

        _vertices = new SpriteVertex[MaxVertices];
        _indices = new uint[MaxIndices];
        
        _verticesCache = new SpriteVertex[NumVertices];
        _indicesCache = new uint[NumIndices];

        _vertexBuffer = _device.CreateBuffer(BufferType.VertexBuffer, MaxSprites * VertexSizeInBytes, true);
        _indexBuffer = _device.CreateBuffer(BufferType.IndexBuffer, MaxSprites * IndicesSizeInBytes, true);
        
        _projViewBuffer = _device.CreateBuffer(BufferType.UniformBuffer, Matrix4x4.Identity, true);

        //_shader = _device.CreateCrossPlatformShader(
        //    new ShaderAttachment(ShaderStage.Vertex, Utils.LoadEmbeddedString("Easel.Graphics.Shaders.SpriteRenderer.Sprite.vert")),
        //    new ShaderAttachment(ShaderStage.Fragment,
        //        Utils.LoadEmbeddedString("Easel.Graphics.Shaders.SpriteRenderer.Sprite.frag")));
        _spriteEffect = new Effect("Easel.Graphics.Shaders.SpriteRenderer.Sprite.vert", "Easel.Graphics.Shaders.SpriteRenderer.Sprite.frag");
        _roundedRectEffect = new Effect("Easel.Graphics.Shaders.SpriteRenderer.Sprite.vert", "Easel.Graphics.Shaders.SpriteRenderer.Shape.RoundedRect.frag");

        _layout = _device.CreateInputLayout(
            new InputLayoutDescription("aPosition", AttributeType.Float2, 0, 0, InputType.PerVertex),
            new InputLayoutDescription("aTexCoords", AttributeType.Float2, 8, 0, InputType.PerVertex),
            new InputLayoutDescription("aTint", AttributeType.Float4, 16, 0, InputType.PerVertex),
            new InputLayoutDescription("aRotation", AttributeType.Float, 32, 0, InputType.PerVertex),
            new InputLayoutDescription("aOrigin", AttributeType.Float2, 36, 0, InputType.PerVertex),
            new InputLayoutDescription("aScale", AttributeType.Float2, 44, 0, InputType.PerVertex),
            new InputLayoutDescription("aMeta1", AttributeType.Float4, 52, 0, InputType.PerVertex),
            new InputLayoutDescription("aMeta2", AttributeType.Float4, 68, 0, InputType.PerVertex));

        _rasterizerState = _device.CreateRasterizerState(RasterizerStateDescription.CullNone);
        _depthState = _device.CreateDepthState(DepthStateDescription.Disabled);
        _blendState = _device.CreateBlendState(BlendStateDescription.NonPremultiplied);
    }

    public void Begin(Matrix4x4? transform = null, Matrix4x4? projection = null, Effect effect = null)
    {
        if (_begun)
            throw new EaselException("SpriteRenderer session is already active.");
        _begun = true;

        Rectangle viewport = EaselGame.Instance.GraphicsInternal.Viewport;
        projection ??= Matrix4x4.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, -1f, 1f);
        transform ??= Matrix4x4.Identity;
        _effectToUse = effect;
        
        _device.UpdateBuffer(_projViewBuffer, 0, transform.Value * projection.Value);
    }

    public void DrawRectangle(Vector2 position, Size size, int borderWidth, float radius, Color color,
        Color borderColor, float rotation, Vector2 origin)
    {
        DrawRectangle(Texture2D.Blank, position, size, borderWidth, radius, color, borderColor, rotation, origin);
    }
    
    public void DrawRectangle(Texture texture, Vector2 position, Size size, int borderWidth, float radius, 
        Color color, Color borderColor, float rotation, Vector2 origin)
    {
        if (!_begun)
            throw new EaselException("No current active sprite renderer session.");
        // We need to adjust the size and position as for some reason the rectangle is one pixel off position wise
        // however removing the offset in the shader doesn't look right...
        if (_currentTexture != texture || _currentType != SpriteType.RoundedRect || _totalVertices >= MaxVertices || _totalIndices >= MaxIndices)
            Flush();
        _currentType = SpriteType.RoundedRect;
        _currentTexture = texture;

        float width = size.Width;
        float height = size.Height;
        
        position -= origin;
        origin += position;
        float posX = position.X;
        float posY = position.Y;
        
        Vector4 meta1 = new Vector4(borderWidth, radius, size.Width, size.Height);
        Vector4 meta2 = (Vector4) borderColor;
        
        _verticesCache[0] = new SpriteVertex(new Vector2(posX + width, posY + height), new Vector2(1, 1), color, rotation, origin, Vector2.One, meta1, meta2);
        _verticesCache[1] = new SpriteVertex(new Vector2(posX + width, posY), new Vector2(1, 0), color, rotation, origin, Vector2.One, meta1, meta2);
        _verticesCache[2] = new SpriteVertex(new Vector2(posX, posY), new Vector2(0, 0), color, rotation, origin, Vector2.One, meta1, meta2);
        _verticesCache[3] = new SpriteVertex(new Vector2(posX, posY + height), new Vector2(0, 1), color, rotation, origin, Vector2.One, meta1, meta2);

        uint dc = _totalVertices;
        _indicesCache[0] = 0u + dc;
        _indicesCache[1] = 1u + dc;
        _indicesCache[2] = 3u + dc;
        _indicesCache[3] = 1u + dc;
        _indicesCache[4] = 2u + dc;
        _indicesCache[5] = 3u + dc;
        
        Array.Copy(_verticesCache, 0, _vertices, _totalVertices, NumVertices);
        Array.Copy(_indicesCache, 0, _indices, _totalIndices, NumIndices);

        _totalVertices += NumVertices;
        _totalIndices += NumIndices;
    }

    public void End()
    {
        if (!_begun)
            throw new EaselException("No current active sprite renderer session.");
        _begun = false;

        Flush();
    }

    public void Draw(Texture texture, Vector2 position, Rectangle? source, Color tint, float rotation, Vector2 origin, Vector2 scale, SpriteFlip flip = SpriteFlip.None, Vector4 meta1 = default, Vector4 meta2 = default)
    {
        // TODO: Remove maximum sprites and implement buffer resizing
        if (texture != _currentTexture || _currentType != SpriteType.Bitmap || _totalVertices >= MaxVertices || _totalIndices >= MaxIndices)
            Flush();
        if (EaselGame.Instance.AllowMissing)
            texture ??= Texture2D.Missing;
        _currentTexture = texture;
        _currentType = SpriteType.Bitmap;

        Rectangle src = source ?? new Rectangle(Point.Zero, texture.Size);

        int rectX = src.X;
        int rectY = src.Y;
        int rectWidth = src.Width;
        int rectHeight = src.Height;
        
        float width = texture.Size.Width;
        float height = texture.Size.Height;
        
        position -= origin * scale;
        origin += position / scale;
        float posX = position.X;
        float posY = position.Y;

        float texX = rectX / width;
        float texY = rectY / height;
        float texW = rectWidth / width;
        float texH = rectHeight / height;

        bool isRenderTarget = _currentTexture is RenderTarget && _device.Api == GraphicsApi.OpenGl33;
        if (isRenderTarget && flip != SpriteFlip.FlipY)
            flip = SpriteFlip.FlipY;
        else if (isRenderTarget && flip == SpriteFlip.FlipY)
            flip = SpriteFlip.None;

        switch (flip)
        {
            case SpriteFlip.None:
                break;
            case SpriteFlip.FlipX:
                //texW = -texW;
                //texX = texW - texX;
                texX = 1 - texX;
                texW = -texW;
                break;
            case SpriteFlip.FlipY:
                //float tempTex = texH;
                //texH = texY;
                //texY = tempTex;
                texY = 1 - texY;
                texH = -texH;
                break;
            case SpriteFlip.FlipXY:
                //texW = -texW;
                //texX = texW - texX;
                //texH = -texH;
                //texY = texH - texY;
                texX = 1 - texX;
                texW = -texW;
                texY = 1 - texY;
                texH = -texH;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        width = rectWidth * scale.X;
        height = rectHeight * scale.Y;

        _verticesCache[0] = new SpriteVertex(new Vector2(posX + width, posY + height), new Vector2(texX + texW, texY + texH), tint, rotation, origin, scale, meta1, meta2);
        _verticesCache[1] = new SpriteVertex(new Vector2(posX + width, posY), new Vector2(texX + texW, texY), tint, rotation, origin, scale, meta1, meta2);
        _verticesCache[2] = new SpriteVertex(new Vector2(posX, posY), new Vector2(texX, texY), tint, rotation, origin, scale, meta1, meta2);
        _verticesCache[3] = new SpriteVertex(new Vector2(posX, posY + height), new Vector2(texX, texY + texH), tint, rotation, origin, scale, meta1, meta2);

        uint dc = _totalVertices;
        _indicesCache[0] = 0u + dc;
        _indicesCache[1] = 1u + dc;
        _indicesCache[2] = 3u + dc;
        _indicesCache[3] = 1u + dc;
        _indicesCache[4] = 2u + dc;
        _indicesCache[5] = 3u + dc;
        
        Array.Copy(_verticesCache, 0, _vertices, _totalVertices, NumVertices);
        Array.Copy(_indicesCache, 0, _indices, _totalIndices, NumIndices);

        _totalVertices += NumVertices;
        _totalIndices += NumIndices;
    }

    public void DrawVertices(Texture texture, SpriteVertex[] vertices, uint[] indices)
    {
        // TODO: Check for vertex & index buffer overflows instead of checking for draw count
        // TODO: DAMN AUTO SUPER DUPER BUFFER RESIZING GET ON IT
        if (texture != _currentTexture || _currentType != SpriteType.Bitmap || _totalVertices >= MaxVertices || _totalIndices >= MaxIndices)
            Flush();
        if (EaselGame.Instance.AllowMissing)
            texture ??= Texture2D.Missing;
        _currentTexture = texture;
        _currentType = SpriteType.Bitmap;

        uint dc = _totalVertices;
        for (int i = 0; i < indices.Length; i++)
            indices[i] += dc;

        Array.Copy(vertices, 0, _vertices, _totalVertices, vertices.Length);
        Array.Copy(indices, 0, _indices, _totalIndices, indices.Length);

        _totalVertices += (uint) vertices.Length;
        _totalIndices += (uint) indices.Length;
    }

    private void Flush()
    {
        if (_totalIndices == 0)
            return;
        
        _device.UpdateBuffer(_vertexBuffer, 0, _vertices);
        _device.UpdateBuffer(_indexBuffer, 0, _indices);

        Effect effect = _effectToUse;
        if (effect == null)
        {
            effect = _currentType switch
            {
                SpriteType.Bitmap => _spriteEffect,
                SpriteType.RoundedRect => _roundedRectEffect,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        _device.SetShader(effect.PieShader);
        _device.SetRasterizerState(_rasterizerState);
        _device.SetDepthState(_depthState);
        _device.SetBlendState(_blendState);
        _device.SetUniformBuffer(0, _projViewBuffer);
        _device.SetTexture(1, _currentTexture.PieTexture, _currentTexture.SamplerState.PieSamplerState);
        _device.SetPrimitiveType(PrimitiveType.TriangleList);
        _device.SetVertexBuffer(0, _vertexBuffer, SpriteVertex.SizeInBytes, _layout);
        _device.SetIndexBuffer(_indexBuffer, IndexType.UInt);
        _device.DrawIndexed(_totalIndices);

        _totalVertices = 0;
        _totalIndices = 0;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SpriteVertex
    {
        public Vector2 Position;
        public Vector2 TexCoord;
        public Color Tint;
        public float Rotation;
        public Vector2 Origin;
        public Vector2 Scale;
        public Vector4 Meta1;
        public Vector4 Meta2;
        
        public SpriteVertex(Vector2 position, Vector2 texCoord, Color tint, float rotation, Vector2 origin, Vector2 scale, Vector4 meta1, Vector4 meta2)
        {
            Position = position;
            TexCoord = texCoord;
            Tint = tint;
            Rotation = rotation;
            Origin = origin;
            Scale = scale;
            Meta1 = meta1;
            Meta2 = meta2;
        }

        public SpriteVertex(Vector2 position, Vector2 texCoord, Color tint) : this()
        {
            Position = position;
            TexCoord = texCoord;
            Tint = tint;
        }

        public const uint SizeInBytes = 84;
    }

    private enum SpriteType
    {
        Bitmap,
        RoundedRect
    }

    public void Dispose()
    {
        _vertexBuffer.Dispose();
        _indexBuffer.Dispose();
        _projViewBuffer.Dispose();
        _spriteEffect.Dispose();
        _roundedRectEffect.Dispose();
        _layout.Dispose();
        _rasterizerState.Dispose();
        _depthState.Dispose();
        _blendState.Dispose();
        _currentTexture.Dispose();
    }
}

public enum SpriteFlip
{
    None,
    FlipX,
    FlipY,
    FlipXY
}