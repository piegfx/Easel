namespace Easel.Graphics.Renderers;

public struct RenderOptions
{
    /// <summary>
    /// If enabled, Pie's debug layer will be enabled. Useful if you are encountering unexpected graphical bugs either
    /// in the engine itself, or in your custom rendering code. !! WARNING !! This will <b>spam</b> your logs with debug
    /// messages. It is highly recommended that you do not enable this option when releasing.
    /// </summary>
    public bool GraphicsDebugging;
    
    /// <summary>
    /// If enabled, Easel will use a Deferred pipeline. Otherwise, a Forward+ pipeline will be used.
    /// </summary>
    public bool Deferred;

    public static RenderOptions Default => new RenderOptions()
    {
        Deferred = false,
        GraphicsDebugging = false
    };
}