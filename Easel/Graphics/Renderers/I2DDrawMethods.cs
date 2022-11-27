using System.Numerics;
using Easel.Math;

namespace Easel.Graphics.Renderers;

public interface I2DDrawMethods
{
    public void Draw(Texture texture, Rectangle destination, Color tint, float z = 0);

    public void Draw(Texture texture, Rectangle destination, Rectangle? source, Color tint, float z = 0);

    public void Draw(Texture texture, Rectangle destination, Rectangle? source, Color tint, float rotation,
        Vector2 origin, SpriteFlip flip = SpriteFlip.None, float z = 0);

    public void Draw(Texture texture, Vector3 position);

    public void Draw(Texture texture, Vector3 position, Color tint);

    public void Draw(Texture texture, Vector3 position, Rectangle? source, Color tint);

    public void Draw(Texture texture, Vector3 position, Rectangle? source, Color tint, float rotation,
        Vector2 origin, float scale, SpriteFlip flip = SpriteFlip.None);

    public void Draw(Texture texture, Vector3 position, Rectangle? source, Color tint, float rotation, Vector2 origin,
        Vector2 scale, SpriteFlip flip = SpriteFlip.None);
}