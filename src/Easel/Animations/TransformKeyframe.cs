using Easel.Entities;

namespace Easel.Animations;

public struct TransformKeyframe : IAnimationKeyframe
{
    public Transform Transform;

    public double Time { get; set; }

    public TransformKeyframe(Transform transform, double time)
    {
        Transform = transform;
        Time = time;
    }
}