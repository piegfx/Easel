using System.Numerics;

namespace Easel.Graphics.Renderers;

public class TransformedRenderable
{
    public Renderable Renderable;
    public Matrix4x4 World;

    public TransformedRenderable(Renderable renderable, Matrix4x4 world)
    {
        Renderable = renderable;
        World = world;
    }
}