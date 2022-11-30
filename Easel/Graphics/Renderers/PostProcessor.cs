using System;
using System.Numerics;
using Easel.Math;

namespace Easel.Graphics.Renderers;

public class PostProcessor
{
    public RenderTarget MainTarget;
    private Effect _effect;
    private bool _shouldAutomaticallyAdjust;
    private Size? _resolution;

    public SpriteRenderMode ScalingMode;
    public TargetDrawMode DrawMode;

    public Size? Resolution
    {
        get => _resolution;
        set
        {
            if (value != null)
            {
                _resolution = value;
                _shouldAutomaticallyAdjust = false;
                ResizeTarget(value.Value);
            }
        }
    }
    
    public PostProcessor(ref PostProcessorSettings settings, EaselGraphics graphics, Size? resolution = null)
    {
        _resolution = resolution;
        _shouldAutomaticallyAdjust = resolution == null;
        MainTarget = new RenderTarget(resolution ?? graphics.Viewport.Size, false);
        graphics.SwapchainResized += GraphicsOnSwapchainResized;
        CreateResources(ref settings);

        ScalingMode = SpriteRenderMode.Linear;
        DrawMode = TargetDrawMode.ActualSize;
    }

    private void GraphicsOnSwapchainResized(Size size)
    {
        if (_shouldAutomaticallyAdjust)
            ResizeTarget(size);
    }

    private void ResizeTarget(Size size)
    {
        MainTarget.Dispose();
        MainTarget = new RenderTarget(size, false);
    }

    public void ApplyPostProcessorSettings(ref PostProcessorSettings settings)
    {
        Clear();
        CreateResources(ref settings);
    }

    private void Clear()
    {
        
    }

    private void CreateResources(ref PostProcessorSettings settings)
    {
        _effect = new Effect("Easel.Graphics.Shaders.SpriteRenderer.Sprite.vert",
            "Easel.Graphics.Shaders.PostProcessor.frag");
    }

    public void Process(EaselGraphics graphics)
    {
        graphics.Clear(Color.Black);
        graphics.SpriteRenderer.Begin(effect: _effect, mode: ScalingMode);
        Vector2 position, scale, origin;
        switch (DrawMode)
        {
            case TargetDrawMode.ActualSize:
                position = Vector2.Zero;
                scale = Vector2.One;
                origin = Vector2.Zero;
                break;
            case TargetDrawMode.Stretch:
                position = Vector2.Zero;
                scale = new Vector2(graphics.Viewport.Width / (float) MainTarget.Size.Width,
                    graphics.Viewport.Height / (float) MainTarget.Size.Height);
                origin = Vector2.Zero;
                break;
            case TargetDrawMode.Fill:
                scale = new Vector2(graphics.Viewport.Width > graphics.Viewport.Height
                    ? graphics.Viewport.Height / (float) MainTarget.Size.Height
                    : graphics.Viewport.Width / (float) MainTarget.Size.Width);
                position = (Vector2) graphics.Viewport.Size / 2;
                origin = (Vector2) MainTarget.Size / 2;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        graphics.SpriteRenderer.Draw(MainTarget, position, null, Color.White, 0, origin, scale);
        graphics.SpriteRenderer.End();
    }
    
    public struct PostProcessorSettings
    {
        public bool Fxaa;
    }

    public enum TargetDrawMode
    {
        /// <summary>
        /// Draw the target at its actual size, at the top left of the screen. Does not draw the target at any other
        /// size.
        /// </summary>
        ActualSize,
        
        /// <summary>
        /// Stretches the target to fill the entire screen.
        /// </summary>
        Stretch,
        
        /// <summary>
        /// Fills the screen, but does not stretch.
        /// </summary>
        Fill
    }
}