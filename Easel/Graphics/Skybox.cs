using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Easel.Entities;
using Easel.Formats;
using Easel.Primitives;
using Easel.Utilities;
using Pie;
using Pie.ShaderCompiler;
using Pie.Utils;

namespace Easel.Graphics;

public class Skybox : IDisposable
{
    public SamplerState SamplerState;
    
    public readonly Pie.Texture PieTexture;
    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _indexBuffer;
    private GraphicsBuffer _cameraBuffer;
    
    private DepthState _depthState;
    private RasterizerState _rasterizerState;

    private Shader _shader;
    private InputLayout _inputLayout;

    private CameraInfo _cameraInfo;

    private GraphicsDevice _device;

    public Skybox(EaselTexture texture, SamplerState samplerState = null) 
        : this(texture.Cubemap[0], texture.Cubemap[1], texture.Cubemap[2],
        texture.Cubemap[3], texture.Cubemap[4], texture.Cubemap[5], samplerState)
    {
        
    }
    
    public Skybox(Bitmap right, Bitmap left, Bitmap top, Bitmap bottom, Bitmap front, Bitmap back, SamplerState samplerState = null)
    {
        _device = EaselGame.Instance.GraphicsInternal.PieGraphics;

        TextureData[] textureDatas = new TextureData[6];
        textureDatas[0] = new TextureData(right.Data);
        textureDatas[1] = new TextureData(left.Data);
        textureDatas[2] = new TextureData(top.Data);
        textureDatas[3] = new TextureData(bottom.Data);
        textureDatas[4] = new TextureData(front.Data);
        textureDatas[5] = new TextureData(back.Data);

        PieTexture =
            _device.CreateTexture(
                new TextureDescription(TextureType.Cubemap, right.Size.Width, right.Size.Height, right.Format, 1, 1,
                    TextureUsage.ShaderResource), textureDatas);

        Cube cube = new Cube();
        VertexPositionTextureNormalTangent[] vptnts = cube.Vertices;
        VertexPosition[] vps = new VertexPosition[vptnts.Length];
        for (int i = 0; i < vptnts.Length; i++)
        {
            vps[i] = new VertexPosition(vptnts[i].Position);
        }

        _vertexBuffer = _device.CreateBuffer(BufferType.VertexBuffer, vps);
        _indexBuffer = _device.CreateBuffer(BufferType.IndexBuffer, cube.Indices);

        _cameraBuffer = _device.CreateBuffer(BufferType.UniformBuffer, _cameraInfo, true);
        
        _depthState = _device.CreateDepthState(new DepthStateDescription(true, false, DepthComparison.LessEqual));
        _rasterizerState = _device.CreateRasterizerState(RasterizerStateDescription.CullCounterClockwise);

        _shader = _device.CreateCrossPlatformShader(
            new ShaderAttachment(ShaderStage.Vertex, Utils.LoadEmbeddedString("Easel.Graphics.Shaders.Skybox.Skybox.vert")),
            new ShaderAttachment(ShaderStage.Fragment, Utils.LoadEmbeddedString("Easel.Graphics.Shaders.Skybox.Skybox.frag")));

        _inputLayout =
            _device.CreateInputLayout(new InputLayoutDescription("aPosition", AttributeType.Float3, 0, 0,
                InputType.PerVertex));

        SamplerState = samplerState ?? SamplerState.LinearClamp;
    }

    internal void Draw(Camera camera)
    {
        _cameraInfo.Projection = camera.ProjectionMatrix;
        _cameraInfo.View = camera.ViewMatrix.To3x3Matrix();
        
        _device.UpdateBuffer(_cameraBuffer, 0, _cameraInfo);
        
        _device.SetShader(_shader);
        _device.SetPrimitiveType(PrimitiveType.TriangleList);
        _device.SetUniformBuffer(0, _cameraBuffer);
        _device.SetTexture(1, PieTexture, SamplerState.PieSamplerState);
        _device.SetDepthState(_depthState);
        _device.SetRasterizerState(_rasterizerState);
        _device.SetVertexBuffer(0, _vertexBuffer, VertexPosition.SizeInBytes, _inputLayout);
        _device.SetIndexBuffer(_indexBuffer, IndexType.UInt);
        // 36 because cube
        _device.DrawIndexed(36);
    }
    
    [StructLayout(LayoutKind.Sequential)]
    private struct CameraInfo
    {
        public Matrix4x4 Projection;
        public Matrix4x4 View;
    }

    public void Dispose()
    {
        PieTexture.Dispose();
        _vertexBuffer.Dispose();
        _indexBuffer.Dispose();
        _cameraBuffer.Dispose();
        _depthState.Dispose();
        _rasterizerState.Dispose();
        _shader.Dispose();
        _inputLayout.Dispose();
    }
}