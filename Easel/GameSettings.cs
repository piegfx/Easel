using System.Reflection;
using Easel.Math;
using Pie;

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
    /// Whether or not the window will be resizable. (Default: <see langword="false" />)
    /// </summary>
    public bool Resizable;

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
    /// Create the default game settings.
    /// </summary>
    public GameSettings()
    {
        Size = new Size(1280, 720);

        string? name = Assembly.GetEntryAssembly()?.GetName().Name;
        Title = name == null ? "Easel Window" : name + " - Easel";
        Resizable = false;
        VSync = true;
        TargetFps = 0;
        Api = null;
    }
}