using System;
using System.Collections.Generic;

namespace Easel.Animations;

public class Animation
{
    public List<IAnimationChannel> Channels;

    private double _currentTime;

    public double TotalRunningTime { get; private set; }

    public double Time
    {
        get => _currentTime;
        // set
    }

    public Animation(int numChannels = 0)
    {
        Channels = new List<IAnimationChannel>(numChannels);
    }

    public void AddChannel(IAnimationChannel channel)
    {
        Channels.Add(channel);
        CalculateRunningTime();
    }

    public void Update()
    {
        double dt = Easel.Time.DeltaTimeD;
        
        foreach (IAnimationChannel channel in Channels)
            channel.Update(dt);

        _currentTime += dt;
        
        if (_currentTime >= TotalRunningTime)
        {
            _currentTime = 0;
            foreach (IAnimationChannel channel in Channels)
                channel.Reset();
        }
    }

    private void CalculateRunningTime()
    {
        double largestTime = 0;
        
        foreach (IAnimationChannel channel in Channels)
        {
            double time = channel.Keyframes[^1].Time;
            if (time > largestTime)
                largestTime = time;
        }

        TotalRunningTime = largestTime;
    }
}