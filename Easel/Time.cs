using System.Diagnostics;
using Pie.Windowing;

namespace Easel;

public static class Time
{
    private static double _deltaTime;
    
    private static Stopwatch _stopwatch;

    public static float DeltaTime => (float) _deltaTime;

    internal static void Initialize()
    {
        _stopwatch = Stopwatch.StartNew();
    }

    internal static void Update()
    { 
        _deltaTime = (float) _stopwatch.Elapsed.TotalSeconds;
        _stopwatch.Restart();

        //double time = window.Time;
        //double time = _stopwatch.Elapsed.TotalSeconds;
        //_deltaTime = time - _lastTime;
        //_lastTime = time;
    }
}