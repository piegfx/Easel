using System.Numerics;
using Easel.Graphics.Renderers.Structs;

namespace Easel.Graphics.Renderers;

public interface IRenderer
{
    /// <summary>
    /// Add a single opaque object instance that will be rendered in the scene. These objects are drawn front-to-back.
    /// </summary>
    /// <param name="renderable">The renderable instance to draw.</param>
    /// <param name="world">Its world transform.</param>
    public void AddOpaque(in Renderable renderable, in Matrix4x4 world);

    /// <summary>
    /// Clear all objects, lights, etc in the current frame.
    /// </summary>
    public void ClearAll();
    
    // I could use a camera as a parameter instead, but I'd rather separate the rendering part of the engine
    // from the other parts of the engine, to keep things modular.
    /// <summary>
    /// Render all objects added to the current render frame.
    /// </summary>
    /// <param name="cameraInfo"></param>
    public void Render(in CameraInfo cameraInfo);
}