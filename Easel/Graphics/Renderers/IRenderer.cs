using System.Numerics;
using Easel.Graphics.Renderers.Structs;

namespace Easel.Graphics.Renderers;

public interface IRenderer
{
    public void AddOpaque(in Renderable renderable, in Matrix4x4 world);

    // I could use a camera as a parameter instead, but I'd rather separate the rendering part of the engine
    // from the other parts of the engine, to keep things modular.
    public void Render(in CameraInfo cameraInfo);
}