using System.Collections.Generic;
using System.Numerics;
using Easel.Entities;
using Easel.Math;

namespace Easel.Graphics.Renderers;

public class Default2DRenderer : I2DRenderer
{
    private EaselGraphics _graphics;

    private List<I2DDrawInstruction> _drawList;

    public Default2DRenderer(EaselGraphics graphics)
    {
        _graphics = graphics;
        _drawList = new List<I2DDrawInstruction>();
    }
    
    public void Draw(Texture texture, Rectangle destination, Color tint)
    {
        _drawList.Add(new Sprite(texture, (Vector2) destination.Location, null, tint, 0, Vector2.Zero,
            (Vector2) destination.Size / (Vector2) texture.Size, SpriteFlip.None));
    }

    public void Draw(Texture texture, Rectangle destination, Rectangle? source, Color tint)
    {
        _drawList.Add(new Sprite(texture, (Vector2) destination.Location, source, tint, 0, Vector2.Zero,
            (Vector2) destination.Size / (Vector2) texture.Size, SpriteFlip.None));
    }

    public void Draw(Texture texture, Rectangle destination, Rectangle? source, Color tint, float rotation, Vector2 origin,
        SpriteFlip flip = SpriteFlip.None)
    {
        _drawList.Add(new Sprite(texture, (Vector2) destination.Location, source, tint, rotation, origin,
            (Vector2) destination.Size / (Vector2) texture.Size, flip));
    }

    public void Draw(Texture texture, Vector2 position)
    {
        _drawList.Add(new Sprite(texture, position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteFlip.None));
    }

    public void Draw(Texture texture, Vector2 position, Color tint)
    {
        _drawList.Add(new Sprite(texture, position, null, tint, 0, Vector2.Zero, Vector2.One, SpriteFlip.None));
    }

    public void Draw(Texture texture, Vector2 position, Rectangle? source, Color tint)
    {
        _drawList.Add(new Sprite(texture, position, source, tint, 0, Vector2.Zero, Vector2.One, SpriteFlip.None));
    }

    public void Draw(Texture texture, Vector2 position, Rectangle? source, Color tint, float rotation, Vector2 origin, float scale,
        SpriteFlip flip = SpriteFlip.None)
    {
        _drawList.Add(new Sprite(texture, position, source, tint, rotation, origin, new Vector2(scale), flip));
    }

    public void Draw(Texture texture, Vector2 position, Rectangle? source, Color tint, float rotation, Vector2 origin,
        Vector2 scale, SpriteFlip flip = SpriteFlip.None)
    {
        _drawList.Add(new Sprite(texture, position, source, tint, rotation, origin, scale, flip));
    }

    public void ClearAll()
    {
        _drawList.Clear();
    }

    public void Render(Camera camera)
    {
        _graphics.SpriteRenderer.Begin(camera.ViewMatrix, camera.ProjectionMatrix);
        for (int i = 0; i < _drawList.Count; i++)
            _drawList[i].Draw(_graphics.SpriteRenderer);
        _graphics.SpriteRenderer.End();
    }
    
    private interface I2DDrawInstruction
    {
        public void Draw(SpriteRenderer renderer);
    }

    private struct Sprite : I2DDrawInstruction
    {
        private Texture _texture;
        private Vector2 _position;
        private Rectangle? _source;
        private Color _tint;
        private float _rotation;
        private Vector2 _origin;
        private Vector2 _scale;
        private SpriteFlip _flip;

        public Sprite(Texture texture, Vector2 position, Rectangle? source, Color tint, float rotation, Vector2 origin, Vector2 scale, SpriteFlip flip)
        {
            _texture = texture;
            _position = position;
            _source = source;
            _tint = tint;
            _rotation = rotation;
            _origin = origin;
            _scale = scale;
            _flip = flip;
        }

        public void Draw(SpriteRenderer renderer)
        {
            renderer.Draw(_texture, _position, _source, _tint, _rotation, _origin, _scale, _flip);
        }
    }
}