using System.Numerics;
using Easel.Math;

namespace Easel.Graphics.Renderers;

public interface I2DDrawMethods
{
    public void Draw(Texture texture, Rectangle<float> destination, Color tint, float z = 0);

    public void Draw(Texture texture, Rectangle<float> destination, Rectangle<int>? source, Color tint, float z = 0);

    public void Draw(Texture texture, Rectangle<float> destination, Rectangle<int>? source, Color tint, float rotation,
        Vector2T<float> origin, SpriteFlip flip = SpriteFlip.None, float z = 0);

    public void Draw(Texture texture, Vector3T<float> position);

    public void Draw(Texture texture, Vector3T<float> position, Color tint);

    public void Draw(Texture texture, Vector3T<float> position, Rectangle<int>? source, Color tint);

    public void Draw(Texture texture, Vector3T<float> position, Rectangle<float>? source, Color tint, float rotation,
        Vector2T<float> origin, float scale, SpriteFlip flip = SpriteFlip.None);

    public void Draw(Texture texture, Vector3T<float> position, Rectangle<float>? source, Color tint, float rotation,
        Vector2T<float> origin, Vector2T<float> scale, SpriteFlip flip = SpriteFlip.None);

    public void DrawRectangle(Vector3T<float> position, Size<float> size, int borderWidth, float radius, Color color,
        Color borderColor, float rotation, Vector2T<float> origin);
    
    public void DrawRectangle(Texture texture, Vector3T<float> position, Size<float> size, int borderWidth, float radius,
        Color color, Color borderColor, float rotation, Vector2T<float> origin);
}