using System.Collections.Generic;
using System.Numerics;
using Easel.Entities;
using Easel.Math;
using Easel.Scenes;
using Easel.Utilities;

namespace Easel.Graphics.Renderers;

public class Default2DRenderer : I2DRenderer
{
    private EaselGraphics _graphics;

    private List<ISprite> _drawList;

    public Default2DRenderer(EaselGraphics graphics)
    {
        _graphics = graphics;
        _drawList = new List<ISprite>();
    }
    
    public void Draw(Texture texture, Rectangle destination, Color tint, float z = 0)
    {
        _drawList.Add(new Sprite(texture, new Vector3((Vector2) destination.Location, z), null, tint, 0, Vector2.Zero,
            (Vector2) destination.Size / (Vector2) texture.Size, SpriteFlip.None));
    }

    public void Draw(Texture texture, Rectangle destination, Rectangle? source, Color tint, float z = 0)
    {
        _drawList.Add(new Sprite(texture, new Vector3((Vector2) destination.Location, 0), source, tint, 0, Vector2.Zero,
            (Vector2) destination.Size / (Vector2) texture.Size, SpriteFlip.None));
    }

    public void Draw(Texture texture, Rectangle destination, Rectangle? source, Color tint, float rotation, Vector2 origin,
        SpriteFlip flip = SpriteFlip.None, float z = 0)
    {
        _drawList.Add(new Sprite(texture, new Vector3((Vector2) destination.Location, z), source, tint, rotation, origin,
            (Vector2) destination.Size / (Vector2) texture.Size, flip));
    }

    public void Draw(Texture texture, Vector3 position)
    {
        _drawList.Add(new Sprite(texture, position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteFlip.None));
    }

    public void Draw(Texture texture, Vector3 position, Color tint)
    {
        _drawList.Add(new Sprite(texture, position, null, tint, 0, Vector2.Zero, Vector2.One, SpriteFlip.None));
    }

    public void Draw(Texture texture, Vector3 position, Rectangle? source, Color tint)
    {
        _drawList.Add(new Sprite(texture, position, source, tint, 0, Vector2.Zero, Vector2.One, SpriteFlip.None));
    }

    public void Draw(Texture texture, Vector3 position, Rectangle? source, Color tint, float rotation, Vector2 origin, float scale,
        SpriteFlip flip = SpriteFlip.None)
    {
        _drawList.Add(new Sprite(texture, position, source, tint, rotation, origin, new Vector2(scale), flip));
    }

    public void Draw(Texture texture, Vector3 position, Rectangle? source, Color tint, float rotation, Vector2 origin,
        Vector2 scale, SpriteFlip flip = SpriteFlip.None)
    {
        _drawList.Add(new Sprite(texture, position, source, tint, rotation, origin, scale, flip));
    }

    public void DrawRectangle(Vector3 position, Size size, int borderWidth, float radius, Color color, Color borderColor,
        float rotation, Vector2 origin)
    {
        DrawRectangle(Texture2D.Blank, position, size, borderWidth, radius, color, borderColor, rotation, origin);
    }

    public void DrawRectangle(Texture texture, Vector3 position, Size size, int borderWidth, float radius, Color color,
        Color borderColor, float rotation, Vector2 origin)
    {
        _drawList.Add(new RectangleSprite(texture, position, size, borderWidth, radius, color, borderColor, rotation, origin));
    }

    public void ClearAll()
    {
        _drawList.Clear();
    }

    public void Render(Camera camera, World world)
    {
        _graphics.SpriteRenderer.Begin(camera.ViewMatrix, camera.ProjectionMatrix, mode: world.SpriteRenderMode);
        _drawList.Sort((sprite, sprite1) => sprite1.Depth.CompareTo(sprite.Depth));
        for (int i = 0; i < _drawList.Count; i++)
            _drawList[i].Draw(_graphics.SpriteRenderer);
        _graphics.SpriteRenderer.End();
    }

    private interface ISprite
    {
        public float Depth { get; }
        
        public void Draw(SpriteRenderer renderer);
    }

    private struct Sprite : ISprite
    {
        private Texture _texture;
        private Vector2 _position;
        private Rectangle? _source;
        private Color _tint;
        private float _rotation;
        private Vector2 _origin;
        private Vector2 _scale;
        private SpriteFlip _flip;

        public Sprite(Texture texture, Vector3 position, Rectangle? source, Color tint, float rotation, Vector2 origin, Vector2 scale, SpriteFlip flip)
        {
            _texture = texture;
            _position = position.ToVector2();
            _source = source;
            _tint = tint;
            _rotation = rotation;
            _origin = origin;
            _scale = scale;
            _flip = flip;
            Depth = position.Z;
        }

        public float Depth { get; }

        public void Draw(SpriteRenderer renderer)
        {
            renderer.Draw(_texture, _position, _source, _tint, _rotation, _origin, _scale, _flip);
        }
    }

    private struct RectangleSprite : ISprite
    {
        private Texture _texture;
        private Vector2 _position;
        private Size _size;
        private int _borderWidth;
        private float _radius;
        private Color _color;
        private Color _borderColor;
        private float _rotation;
        private Vector2 _origin;

        public RectangleSprite(Texture texture, Vector3 position, Size size, int borderWidth, float radius, Color color, Color borderColor, float rotation, Vector2 origin)
        {
            _texture = texture;
            _position = position.ToVector2();
            _size = size;
            _borderWidth = borderWidth;
            _radius = radius;
            _color = color;
            _borderColor = borderColor;
            _rotation = rotation;
            _origin = origin;
            Depth = position.Z;
        }

        public float Depth { get; }
        
        public void Draw(SpriteRenderer renderer)
        {
            renderer.DrawRectangle(_texture, _position, _size, _borderWidth, _radius, _color, _borderColor, _rotation,
                _origin);
        }
    }
}