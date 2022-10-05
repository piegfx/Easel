using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Easel.Entities;
using Easel.Primitives;
using Easel.Utilities;
using Pie;
using Pie.ShaderCompiler;
using Pie.Utils;

namespace Easel.Graphics;

public class Skybox : IDisposable
{
    public Pie.Texture PieTexture;
    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _indexBuffer;
    private GraphicsBuffer _cameraBuffer;

    private SamplerState _samplerState;
    private DepthState _depthState;
    private RasterizerState _rasterizerState;

    private Shader _shader;
    private InputLayout _inputLayout;

    private CameraInfo _cameraInfo;

    private GraphicsDevice _device;
    
    public Skybox(Bitmap right, Bitmap left, Bitmap top, Bitmap bottom, Bitmap front, Bitmap back)
    {
        _device = EaselGame.Instance.GraphicsInternal.PieGraphics;

        Pie.Texture.CubemapData[] cubemapDatas = new Pie.Texture.CubemapData[6];
        cubemapDatas[0] = new Pie.Texture.CubemapData(right.Data);
        cubemapDatas[1] = new Pie.Texture.CubemapData(left.Data);
        cubemapDatas[2] = new Pie.Texture.CubemapData(top.Data);
        cubemapDatas[3] = new Pie.Texture.CubemapData(bottom.Data);
        cubemapDatas[4] = new Pie.Texture.CubemapData(front.Data);
        cubemapDatas[5] = new Pie.Texture.CubemapData(back.Data);

        PieTexture =
            _device.CreateTexture(
                new TextureDescription(TextureType.Cubemap, right.Size.Width, right.Size.Height, right.Format, false, 1,
                    TextureUsage.ShaderResource), cubemapDatas);

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

        _samplerState = _device.CreateSamplerState(SamplerStateDescription.LinearClamp);
        _depthState = _device.CreateDepthState(new DepthStateDescription(true, false, DepthComparison.LessEqual));
        _rasterizerState = _device.CreateRasterizerState(RasterizerStateDescription.CullCounterClockwise);

        _shader = _device.CreateCrossPlatformShader(
            new ShaderAttachment(ShaderStage.Vertex, Utils.LoadEmbeddedString("Easel.Graphics.Shaders.Skybox.Skybox.vert")),
            new ShaderAttachment(ShaderStage.Fragment, Utils.LoadEmbeddedString("Easel.Graphics.Shaders.Skybox.Skybox.frag")));

        _inputLayout = _device.CreateInputLayout(VertexPosition.SizeInBytes,
            new InputLayoutDescription("aPosition", AttributeType.Float3));
    }

    internal void Draw(Camera camera)
    {
        _cameraInfo.Projection = camera.ProjectionMatrix;
        _cameraInfo.View = camera.ViewMatrix.To3x3Matrix();
        
        _device.UpdateBuffer(_cameraBuffer, 0, _cameraInfo);
        
        _device.SetShader(_shader);
        _device.SetPrimitiveType(PrimitiveType.TriangleList);
        _device.SetUniformBuffer(0, _cameraBuffer);
        _device.SetTexture(1, PieTexture, _samplerState);
        _device.SetDepthState(_depthState);
        _device.SetRasterizerState(_rasterizerState);
        _device.SetVertexBuffer(_vertexBuffer, _inputLayout);
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
        _samplerState.Dispose();
        _depthState.Dispose();
        _rasterizerState.Dispose();
        _shader.Dispose();
        _inputLayout.Dispose();
    }
}