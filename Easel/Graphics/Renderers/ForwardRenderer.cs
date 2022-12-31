using System.Collections.Generic;
using System.Numerics;
using Easel.Graphics.Renderers.Structs;
using Easel.Math;
using Pie;

namespace Easel.Graphics.Renderers;

public class ForwardRenderer : IRenderer
{
    private List<TransformedRenderable> _opaques;

    public ForwardRenderer(EaselGraphics graphics, Size initialResolution)
    {
        _opaques = new List<TransformedRenderable>();

        MainTarget = new RenderTarget(initialResolution);
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
        
        Camera.Skybox?.Draw(Camera.Projection, Camera.View);
    }

    public void Perform3DPass()
    {
        GraphicsDevice device = EaselGame.Instance.GraphicsInternal.PieGraphics;
        
        
    }

    public void Perform2DPass()
    {
        throw new System.NotImplementedException();
    }
}