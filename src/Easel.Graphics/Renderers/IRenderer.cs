using System.Numerics;
using Easel.Graphics.Structs;

namespace Easel.Graphics.Renderers;

public interface IRenderer
{
    public bool InPass { get; }

    public void Begin3DPass(in Matrix4x4 projection, in Matrix4x4 view, in SceneInfo sceneInfo);

    public void End3DPass();

    public void DrawRenderable(in Renderable renderable, in Matrix4x4 world);
}