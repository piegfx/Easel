using Easel.Graphics.Renderers;
using Easel.Math;

namespace Easel.GUI;

public class Panel : UIElement
{
    public Panel(Position position, Size<int> size) : base(position, size) { }
    
    protected internal override void Draw(SpriteRenderer renderer)
    {
        base.Draw(renderer);

        // TODO: what
        //renderer.DrawRectangle(BlurTexture, (Vector2T<float>) CalculatedScreenPos, Size, 0, Theme.BorderRadius,
        //    Color.White, Color.White, 0, Vector2T<float>.Zero);

        renderer.Draw(BlurTexture, (Vector2T<float>) CalculatedScreenPos, null, Color.White, 0, Vector2T<float>.Zero,
            (Vector2T<float>) (Vector2T<int>) Size);
    }
}