using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Easel.Graphics.Renderers.Structs;
using Easel.Math;
using Pie;

namespace Easel.Graphics.Renderers;

public sealed class ForwardRenderer : IRenderer
{
    private List<TransformedRenderable> _opaques;

    private ProjViewModel _projViewModel;
    private GraphicsBuffer _projViewModelBuffer;

    public ForwardRenderer(EaselGraphics graphics, Size initialResolution)
    {
        _opaques = new List<TransformedRenderable>();
        _projViewModel = new ProjViewModel();

        MainTarget = new RenderTarget(initialResolution);
        
        graphics.SwapchainResized += GraphicsOnSwapchainResized;

        GraphicsDevice device = graphics.PieGraphics;

        _projViewModelBuffer = device.CreateBuffer(BufferType.UniformBuffer, _projViewModel, true);
    }

    private void GraphicsOnSwapchainResized(Size size)
    {
        MainTarget.Dispose();
        MainTarget = new RenderTarget(size);
    }

    public CameraInfo Camera { get; set; }
    public RenderTarget MainTarget { get; set; }

    public void AddOpaque(in Renderable renderable, in Matrix4x4 world)
    {
        _opaques.Add(new TransformedRenderable(renderable, world));
    }

    public void NewFrame()
    {
        _opaques.Clear();
        
        EaselGraphics graphics = EaselGame.Instance.GraphicsInternal;
        graphics.SetRenderTarget(MainTarget);
        graphics.Clear(Camera.ClearColor);
    }

    public void Perform3DPass()
    {
        GraphicsDevice device = EaselGame.Instance.GraphicsInternal.PieGraphics;
        
        Camera.Skybox?.Draw(Camera.Projection, Camera.View);
        _projViewModel.Projection = Camera.Projection;
        _projViewModel.View = Camera.View;
        
        device.SetPrimitiveType(PrimitiveType.TriangleList);
        
        // Draw front-to-back for opaques.
        // This is to save a bit of GPU time so it doesn't process fragments that are covered by objects in front.
        foreach (TransformedRenderable renderable in _opaques.OrderBy(renderable => Vector3.Distance(renderable.Transform.Translation, Camera.Position)))
            DrawRenderable(device, renderable);
    }

    private void DrawRenderable(GraphicsDevice device, in TransformedRenderable renderable)
    {
        _projViewModel.Model = renderable.Transform;
        device.UpdateBuffer(_projViewModelBuffer, 0, _projViewModel);
        
        device.SetShader(renderable.Renderable.Material.EffectLayout.Effect.PieShader);
        device.SetUniformBuffer(0, _projViewModelBuffer);
        renderable.Renderable.Material.Apply(device);
        device.SetRasterizerState(renderable.Renderable.Material.RasterizerState.PieRasterizerState);

        device.SetVertexBuffer(0, renderable.Renderable.VertexBuffer,
            renderable.Renderable.Material.EffectLayout.Stride,
            renderable.Renderable.Material.EffectLayout.Layout);
        device.SetIndexBuffer(renderable.Renderable.IndexBuffer, IndexType.UInt);
        device.DrawIndexed(renderable.Renderable.NumIndices);
    }

    public void Perform2DPass()
    {
        throw new System.NotImplementedException();
    }

    public void Dispose()
    {
        MainTarget.Dispose();
    }
}