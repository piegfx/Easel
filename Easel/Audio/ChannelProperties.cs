namespace Easel.Audio;

public struct ChannelProperties
{
    public double Volume;
    public double Speed;
    public double Panning;
    public bool Loop;
    public InterpolationType InterpolationType;

    public ChannelProperties()
    {
        Volume = 1;
        Speed = 1;
        Panning = 0.5;
        Loop = false;
        InterpolationType = InterpolationType.Linear;
    }
}

public enum InterpolationType
{
    None,
    Linear
}