using System;
using Easel.Math;

namespace Easel.Headless.Animations;

public struct Tween
{
    public bool IsFinished;

    public int NumRepeats;

    public bool PingPong;

    public double Value
    {
        get
        {
            if (IsFinished)
                return 1.0;

            double time = _currentTime / _endTime;

            if (PingPong && (_numRepeats & 1) == 1)
                time = 1.0 - time;

            return time;
        }
    }
    
    private double _endTime;
    private double _currentTime;
    private int _numRepeats;

    public Tween(double duration)
    {
        _endTime = duration;
        NumRepeats = 1;
    }

    public void Update()
    {
        if (!IsFinished && _currentTime >= _endTime)
        {
            _numRepeats++;
            if (NumRepeats > 0 && _numRepeats >= NumRepeats)
            {
                IsFinished = true;
                return;
            }
            _currentTime -= _endTime;
        }

        _currentTime += Time.DeltaTimeD;
    }
}