using System.Numerics;
using Easel.Core;
using Easel.Graphics.Structs;

namespace Easel.Graphics.Renderers;

public class DeferredRenderer : IRenderer
{
    private bool _inPass;

    public bool InPass => _inPass;
    
    public void Begin3DPass(in Matrix4x4 projection, in Matrix4x4 view, in SceneInfo sceneInfo)
    {
        if (_inPass)
            throw new EaselException("Renderer is already in a pass!");

        _inPass = true;
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
}