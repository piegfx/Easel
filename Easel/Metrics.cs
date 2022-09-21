namespace Easel;

public static class Metrics
{
    private static ulong _totalFrames;
    private static float _secondsCounter;
    private static int _framesInSecond;
    private static int _fps;

    public static ulong TotalFrames => _totalFrames;

    public static int FPS => _fps;

    internal static void Update()
    {
        _totalFrames++;
        _secondsCounter += Time.DeltaTime;
        _framesInSecond++;
        if (_secondsCounter >= 1)
        {
            _secondsCounter = 0;
            _fps = _framesInSecond;
            _framesInSecond = 0;
        }
    }
}