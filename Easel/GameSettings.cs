using System.Reflection;
using Easel.Math;
using Easel.Utilities;
using Pie;
using Pie.Windowing;

namespace Easel;

/// <summary>
/// Initial game settings used on game startup.
/// </summary>
public struct GameSettings
{
    /// <summary>
    /// The starting size (resolution) of the game window, in pixels. (Default: 1280x720)
    /// </summary>
    public Size Size;

    /// <summary>
    /// The starting title of the game window. (Default: The starting assembly name)
    /// </summary>
    public string Title;

    /// <summary>
    /// The initial border of the window. (Default: <see cref="WindowBorder.Fixed"/>)
    /// </summary>
    public WindowBorder Border;

    /// <summary>
    /// Whether or not the game will synchronize to the vertical refresh rate. (Default: <see langword="true" />)
    /// </summary>
    public bool VSync;

    /// <summary>
    /// The target frames per second of the game. Set to 0 to have unlimited FPS. (Default: 0, if <see cref="VSync"/> is
    /// enabled the game will run at the monitor's native refresh rate (typically 60, 144, etc.)
    /// </summary>
    public int TargetFps;

    /// <summary>
    /// The graphics API the game will use - leave null to let Easel decide which API to use. (Default: <see langword="null"/>)
    ///
    /// Default API:
    /// <list type="bullet">
    ///     <item><b>Windows -</b> <see cref="GraphicsApi.D3D11"/></item>
    ///     <item><b>Linux -</b> <see cref="GraphicsApi.OpenGl33"/></item>
    ///     <item><b>macOS -</b> <see cref="GraphicsApi.OpenGl33"/> - Warning: macOS is not officially supported</item>
    /// </list>
    /// </summary>
    public GraphicsApi? Api;

    /// <summary>
    /// If enabled, Easel will not error if it tries to load items that do not exist, such as textures, instead
    /// displaying a default "missing" object. (Default: <see langword="false" />
    /// </summary>
    public bool AllowMissing;

    /// <summary>
    /// The window icon, if any. (Default: <see langword="null" />)
    /// </summary>
    public Bitmap Icon;

    /// <summary>
    /// The title bar flags, if any (Default: <see cref="Easel.TitleBarFlags.ShowEasel"/>)
    /// </summary>
    public TitleBarFlags TitleBarFlags;
    
    /// <summary>
    /// Create the default game settings.
    /// </summary>
    public GameSettings()
    {
        Size = new Size(1280, 720);
        
        Title = Assembly.GetEntryAssembly()?.GetName().Name ?? "Easel Window";
        Border = WindowBorder.Fixed;
        VSync = true;
        TargetFps = 0;
        Api = null;
        AllowMissing = false;
        Icon = null;
        TitleBarFlags = TitleBarFlags.ShowEasel;
    }
}