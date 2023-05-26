using System;
using Easel.Math;

namespace Easel.Animations;

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

            double time = CurrentTime / _endTime;

            if (PingPong && (_numRepeats & 1) == 1)
                time = 1.0 - time;

            return EaselMath.Clamp(time, 0.0, 1.0);
        }
    }
    
    private double _endTime;
    private int _numRepeats;
    
    public double CurrentTime;

    public Tween(double duration)
    {
        _endTime = duration;
        NumRepeats = 1;
    }

    public void Update()
    {
        if (!IsFinished && CurrentTime >= _endTime)
        {
            _numRepeats++;
            if (NumRepeats > 0 && _numRepeats >= NumRepeats)
            {
                IsFinished = true;
                return;
            }
            CurrentTime -= _endTime;
        }

        CurrentTime += Time.DeltaTimeD;
    }
}