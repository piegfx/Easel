namespace Easel.Headless;

/// <summary>
/// Initial game settings used on game startup.
/// </summary>
public struct GameSettings
{
    /// <summary>
    /// The target frames per second of the game. Set to 0 to have unlimited FPS. (Default: 0)
    /// </summary>
    public int TargetFps;

    public GameSettings(int targetFps)
    {
        TargetFps = targetFps;
    }
    
    /// <summary>
    /// Create the default game settings.
    /// </summary>
    public GameSettings()
    {
        TargetFps = 0;
    }
}