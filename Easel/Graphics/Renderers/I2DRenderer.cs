using Easel.Entities;
using Easel.Scenes;

namespace Easel.Graphics.Renderers;

public interface I2DRenderer : I2DDrawMethods
{
    /// <summary>
    /// Clear all draw lists and prepare the renderer for a new frame.
    /// </summary>
    public void ClearAll();
    
    public void Render(Camera camera, World world);
}