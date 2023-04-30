using System.Diagnostics;

namespace Easel.Headless;

/// <summary>
/// Provides engine time functions.
/// </summary>
public static class Time
{
    private static double _deltaTime;
    
    public static Stopwatch InternalStopwatch;
    private static Stopwatch _timerWatch;

    /// <summary>
    /// Get the amount of time passed since the previous frame. Use to create framerate independent actions.
    /// </summary>
    public static float DeltaTime => (float) _deltaTime;

    public static double DeltaTimeD => _deltaTime;

    public static float TotalSeconds => (float) _timerWatch.Elapsed.TotalSeconds;

    public static double TotalSecondsD => _timerWatch.Elapsed.TotalSeconds;

    public static void Initialize()
    {
        InternalStopwatch = Stopwatch.StartNew();
        _timerWatch = Stopwatch.StartNew();
    }

    public static void Update()
    { 
        _deltaTime = (float) InternalStopwatch.Elapsed.TotalSeconds;
        InternalStopwatch.Restart();

        //double time = view.Time;
        //double time = _stopwatch.Elapsed.TotalSeconds;
        //_deltaTime = time - _lastTime;
        //_lastTime = time;
    }
}