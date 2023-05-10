using System.Collections.Generic;
using System.Linq;
using Easel.Entities;

namespace Easel.Animations;

public class TransformChannel : IAnimationChannel
{
    private int _currentKeyframe;
    private double _currentTime;
    
    public Transform Transform;
    private IAnimationChannel _animationChannelImplementation;

    public List<IAnimationKeyframe> Keyframes { get; set; }

    public TransformChannel(params TransformKeyframe[] keyframes)
    {
        Keyframes = new List<IAnimationKeyframe>(keyframes.Cast<IAnimationKeyframe>());
        _currentKeyframe = 0;
    }
    
    public void Update(double dt)
    {
        if (_currentKeyframe + 1 >= Keyframes.Count)
            return;
        
        TransformKeyframe currentFrame = (TransformKeyframe) Keyframes[_currentKeyframe];
        TransformKeyframe nextFrame = (TransformKeyframe) Keyframes[_currentKeyframe + 1];

        double lerpAmount = (_currentTime - currentFrame.Time) / (nextFrame.Time - currentFrame.Time);

        Transform = Transform.Lerp(currentFrame.Transform, nextFrame.Transform,
            (float) double.Clamp(lerpAmount, 0.0, 1.0));

        _currentTime += dt;

        if (_currentTime >= nextFrame.Time)
        {
            _currentKeyframe++;

            if (_currentKeyframe >= Keyframes.Count)
                _currentKeyframe = 0;
        }
    }

    public void Reset()
    {
        _currentKeyframe = 0;
        _currentTime = 0;
    }
}