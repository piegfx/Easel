using System;
using Easel.Entities;
using Easel.Math;

namespace Easel.Graphics.Renderers;

/// <summary>
/// The base interface of Easel's 3D renderers. Use the built in renderers, or create your own.
/// </summary>
public interface I3DRenderer : IDisposable
{
    public PostProcessor PostProcessor { get; }
    
    /// <summary>
    /// Draw a translucent object. These objects are drawn back-to-front to allow transparency to work.
    /// </summary>
    /// <param name="renderable">The renderable object.</param>
    public void DrawTranslucent(Renderable renderable);
    
    /// <summary>
    /// Draw an opaque object. These objects are draw front-to-back so the GPU won't process fragments that are covered
    /// by other fragments.
    /// </summary>
    /// <param name="renderable"></param>
    public void DrawOpaque(Renderable renderable);

    /// <summary>
    /// Clear all draw lists and prepare the renderer for a new frame.
    /// </summary>
    public void ClearAll();

    /// <summary>
    /// Render all draw lists and perform post-processing.
    /// </summary>
    public void Render(Camera camera, Color clearColor);
}