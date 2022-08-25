using System.Diagnostics;
using Pie.Windowing;

namespace Easel;

public static class Time
{
    private static double _deltaTime;
    
    internal static Stopwatch InternalStopwatch;

    public static float DeltaTime => (float) _deltaTime;

    internal static void Initialize()
    {
        InternalStopwatch = Stopwatch.StartNew();
    }

    internal static void Update()
    { 
        _deltaTime = (float) InternalStopwatch.Elapsed.TotalSeconds;
        InternalStopwatch.Restart();

        //double time = window.Time;
        //double time = _stopwatch.Elapsed.TotalSeconds;
        //_deltaTime = time - _lastTime;
        //_lastTime = time;
    }
}