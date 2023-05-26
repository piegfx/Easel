using System.Collections.Generic;

namespace Easel.Animations;

public interface IAnimationChannel
{
    public List<IAnimationKeyframe> Keyframes { get; set; }

    protected internal void Update(double dt);

    protected internal void Reset();
}