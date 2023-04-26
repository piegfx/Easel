using System;
using System.Numerics;
using System.Reflection;
using Easel.Core;
using Easel.Graphics.Structs.Internal;
using Easel.Math;
using Pie;

namespace Easel.Graphics.Renderers;

public class SpriteRenderer : IDisposable
{
    public const uint MaxSprites = 1 << 14;

    private const uint NumVertices = 4;
    private const uint NumIndices = 6;

    private const uint MaxVertices = NumVertices * MaxSprites;
    private const uint MaxIndices = NumIndices * MaxSprites;

    private SpriteVertex[] _vertices;
    private uint[] _indices;

    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _indexBuffer;

    private CameraMatrices _cameraMatrices;
    private GraphicsBuffer _cameraMatricesBuffer;

    private Shader _shader;
    private InputLayout _inputLayout;

    private DepthStencilState _depthStencilState;
    private RasterizerState _rasterizerState;

    private Texture2D _currentTexture;
    private uint _currentSprite;

    // TODO: Texture sampler states.
    private SamplerState _ss;

    private GraphicsDevice _device;

    public SpriteRenderer(Renderer renderer)
    {
        _device = renderer.Device;
        
        _vertices = new SpriteVertex[MaxVertices];
        _indices = new uint[MaxIndices];

        _vertexBuffer = _device.CreateBuffer(BufferType.VertexBuffer, MaxVertices * SpriteVertex.SizeInBytes, true);
        _indexBuffer = _device.CreateBuffer(BufferType.IndexBuffer, MaxIndices * sizeof(uint), true);

        _cameraMatrices = new CameraMatrices();
        _cameraMatricesBuffer = _device.CreateBuffer(BufferType.UniformBuffer, _cameraMatrices, true);

        byte[] vertex = Utils.LoadEmbeddedResource(Assembly.GetExecutingAssembly(),
            Renderer.AssemblyName + ".2D.Sprite_vert.spv");
        byte[] fragment = Utils.LoadEmbeddedResource(Assembly.GetExecutingAssembly(),
            Renderer.AssemblyName + ".2D.Sprite_frag.spv");

        _shader = _device.CreateShader(new[]
            { new ShaderAttachment(ShaderStage.Vertex, vertex), new ShaderAttachment(ShaderStage.Fragment, fragment) });
        
        _inputLayout = _device.CreateInputLayout(
            new InputLayoutDescription(Format.R32G32_Float, 0, 0, InputType.PerVertex), // position
            new InputLayoutDescription(Format.R32G32_Float, 8, 0, InputType.PerVertex), // texCoord
            new InputLayoutDescription(Format.R32G32B32A32_Float, 16, 0, InputType.PerVertex) // tint
        );

        _depthStencilState = _device.CreateDepthStencilState(DepthStencilStateDescription.Disabled);
        _rasterizerState = _device.CreateRasterizerState(RasterizerStateDescription.CullClockwise);

        _ss = _device.CreateSamplerState(SamplerStateDescription.LinearClamp);

        _currentSprite = 0;
    }

    public void Begin(Matrix4x4? transform = null, Matrix4x4? projection = null)
    {
        _cameraMatrices.Projection = projection ?? Matrix4x4.CreateOrthographicOffCenter(0, _device.Viewport.Width, _device.Viewport.Height, 0, -1, 1);
        _cameraMatrices.View = transform ?? Matrix4x4.Identity;
        
        _device.UpdateBuffer(_cameraMatricesBuffer, 0, _cameraMatrices);
    }

    public void End()
    {
        Flush();
    }

    public void Draw(Texture2D texture, Vector2 position, Rectangle<int>? source, Color tint, float rotation,
        Vector2 origin, Vector2 scale)
    {
        if (texture != _currentTexture || _currentSprite >= MaxSprites)
            Flush();

        Size<int> texSize = texture.Size;

        float x = position.X;
        float y = position.Y;
        float w = texSize.Width * scale.X;
        float h = texSize.Height * scale.Y;

        uint currentVertex = _currentSprite * NumVertices;
        uint currentIndex = _currentSprite * NumIndices;

        _vertices[currentVertex + 0] = new SpriteVertex(new Vector2(x, y), new Vector2(0, 0), tint);
        _vertices[currentVertex + 1] = new SpriteVertex(new Vector2(x + w, y), new Vector2(1, 0), tint);
        _vertices[currentVertex + 2] = new SpriteVertex(new Vector2(x + w, y + h), new Vector2(1, 1), tint);
        _vertices[currentVertex + 3] = new SpriteVertex(new Vector2(x, y + h), new Vector2(0, 1), tint);

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
        // No need to waste GPU time if there is nothing to do.
        // Also weeds out the very first draw call the application makes, when `_currentTexture` is null.
        if (_currentSprite == 0)
            return;

        IntPtr vtx = _device.MapBuffer(_vertexBuffer, MapMode.Write);
        PieUtils.CopyToUnmanaged(vtx, 0, _vertices);
        _device.UnmapBuffer(_vertexBuffer);

        IntPtr idx = _device.MapBuffer(_indexBuffer, MapMode.Write);
        PieUtils.CopyToUnmanaged(idx, 0, _indices);
        _device.UnmapBuffer(_indexBuffer);
        
        _device.SetPrimitiveType(PrimitiveType.TriangleList);
        _device.SetShader(_shader);
        _device.SetDepthStencilState(_depthStencilState);
        _device.SetRasterizerState(_rasterizerState);
        _device.SetTexture(1, _currentTexture.DeviceTexture, _ss);
        _device.SetVertexBuffer(0, _vertexBuffer, SpriteVertex.SizeInBytes, _inputLayout);
        _device.SetIndexBuffer(_indexBuffer, IndexType.UInt);
        _device.DrawIndexed(_currentSprite * NumIndices);

        _currentSprite = 0;
    }
    
    public void Dispose()
    {
        _vertexBuffer.Dispose();
        _indexBuffer.Dispose();
        
        _shader.Dispose();
        _inputLayout.Dispose();
        
        _depthStencilState.Dispose();
        _rasterizerState.Dispose();
    }

    public struct SpriteVertex
    {
        public Vector2 Position;
        public Vector2 TexCoord;
        public Color Tint;

        public SpriteVertex(Vector2 position, Vector2 texCoord, Color tint)
        {
            Position = position;
            TexCoord = texCoord;
            Tint = tint;
        }

        public const uint SizeInBytes = 32;
    }
}