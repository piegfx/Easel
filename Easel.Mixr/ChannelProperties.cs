namespace Easel.Mixr;

public struct ChannelProperties
{
    public double Volume;
    public double Speed;
    public double Panning;

    public bool Loop;

    public ChannelProperties()
    {
        Volume = 1;
        Speed = 1;
        Panning = 0.5;
        Loop = false;
    }
}