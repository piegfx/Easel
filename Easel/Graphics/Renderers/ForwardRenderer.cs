using System.Collections.Generic;
using System.Numerics;
using Easel.Graphics.Renderers.Structs;
using Easel.Math;
using Pie;

namespace Easel.Graphics.Renderers;

public class ForwardRenderer : IRenderer
{
    private List<TransformedRenderable> _opaques;

    private RenderTarget _target;

    public ForwardRenderer(EaselGraphics graphics, Size initialResolution)
    {
        _opaques = new List<TransformedRenderable>();

        _target = new RenderTarget(initialResolution);
    }

    public void AddOpaque(in Renderable renderable, in Matrix4x4 world)
    {
        _opaques.Add(new TransformedRenderable(renderable, world));
    }

    public void Render(in CameraInfo info)
    {
        EaselGraphics graphics = EaselGame.Instance.GraphicsInternal;
        GraphicsDevice device = graphics.PieGraphics;
        
        graphics.SetRenderTarget(_target);
        graphics.Clear(info.ClearColor);
        
        
    }
}