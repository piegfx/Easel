using System.Numerics;
using Easel.Core;
using Easel.Graphics.Structs;
using Easel.Math;

namespace Easel.Graphics.Renderers;

public class ForwardRenderer : IRenderer
{
    private bool _inPass;
    private Renderer _renderer;

    public RenderTarget2D MainTarget { get; private set; }
    
    public bool InPass => _inPass;

    public ForwardRenderer(Size<int> size, Renderer renderer)
    {
        Logger.Debug("Creating forward renderer.");
        _renderer = renderer;
        
        Logger.Debug("Creating main target.");
        MainTarget = new RenderTarget2D(size);
    }
    
    public void Begin3DPass(in Matrix4x4 projection, in Matrix4x4 view, in Vector3 cameraPosition, in SceneInfo sceneInfo)
    {
        if (_inPass)
            throw new EaselException("Renderer is already in a pass!");

        _inPass = true;
        
        _renderer.SetRenderTarget(MainTarget);
    }

    public void End3DPass()
    {
        if (!_inPass)
            throw new EaselException("Renderer is not in a pass!");

        _inPass = false;
    }

    public void DrawRenderable(in Renderable renderable, in Matrix4x4 world)
    {
        throw new System.NotImplementedException();
    }

    public void Resize(Size<int> newSize)
    {
        MainTarget.Dispose();
        MainTarget = new RenderTarget2D(newSize);
    }
}