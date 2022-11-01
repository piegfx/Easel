using System.Numerics;
using Easel.Math;

namespace Easel.Graphics.Renderers;

public interface I2DDrawMethods
{
    public void Draw(Texture texture, Rectangle destination, Color tint);

    public void Draw(Texture texture, Rectangle destination, Rectangle? source, Color tint);

    public void Draw(Texture texture, Rectangle destination, Rectangle? source, Color tint, float rotation,
        Vector2 origin, SpriteFlip flip = SpriteFlip.None);

    public void Draw(Texture texture, Vector2 position);

    public void Draw(Texture texture, Vector2 position, Color tint);

    public void Draw(Texture texture, Vector2 position, Rectangle? source, Color tint);

    public void Draw(Texture texture, Vector2 position, Rectangle? source, Color tint, float rotation,
        Vector2 origin, float scale, SpriteFlip flip = SpriteFlip.None);

    public void Draw(Texture texture, Vector2 position, Rectangle? source, Color tint, float rotation, Vector2 origin,
        Vector2 scale, SpriteFlip flip = SpriteFlip.None);

    public void DrawRectangle(Vector2 position, Size size, Color color, float rotation, Vector2 origin);

    public void DrawRoundedRect(Vector2 position, Size size, int borderWidth, float radius, Color color,
        Color borderColor, float rotation, Vector2 origin);

    public void DrawRoundedRect(Texture2D texture, Vector2 position, Size size, int borderWidth, float radius,
        Color color, Color borderColor, float rotation, Vector2 origin);
}