using System;
using System.Numerics;
using Easel.Graphics.Renderers.Structs;

namespace Easel.Graphics.Renderers;

public interface IRenderer : IDisposable
{
    public CameraInfo Camera { get; set; }
    
    public RenderTarget MainTarget { get; set; }
    
    /// <summary>
    /// Add a single opaque object instance that will be rendered in the scene. These objects are drawn front-to-back.
    /// </summary>
    /// <param name="renderable">The renderable instance to draw.</param>
    /// <param name="world">Its world transform.</param>
    public void AddOpaque(in Renderable renderable, in Matrix4x4 world);

    /// <summary>
    /// Prepare the renderer for a new frame of objects.
    /// </summary>
    public void NewFrame();
    
    /// <summary>
    /// Render all objects added to the current render frame.
    /// </summary>
    public void Perform3DPass();

    public void Perform2DPass();
}