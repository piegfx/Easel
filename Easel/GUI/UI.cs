using System.Collections.Generic;
using System.Numerics;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.Math;

namespace Easel.GUI;

public static class UI
{
    public static UITheme Theme;

    static UI()
    {
        _drawList = new List<IDrawListInstruction>();
        Theme = new UITheme();
    }

    internal static void BeforeUpdate()
    {
        _drawList.Clear();
    }

    internal static void Update(Rectangle viewport)
    {
        bool mouseCaptured = false;
        
        
    }

    internal static void Draw(EaselGraphics graphics)
    {
        graphics.SpriteRenderer.Begin();

        foreach (IDrawListInstruction instruction in _drawList)
            instruction.Draw(graphics.SpriteRenderer, graphics.Viewport);
        
        graphics.SpriteRenderer.End();
    }

    #region Immediate mode

    private static List<IDrawListInstruction> _drawList;

    public static void AddToDrawList(IDrawListInstruction instruction)
    {
        _drawList.Add(instruction);
    }

    public interface IDrawListInstruction
    {
        public Position Position { get; set; }
        
        public Size Size { get; set; }

        public void Draw(SpriteRenderer renderer, Rectangle viewport);
    }

    public class TextureDrawListInstruction : IDrawListInstruction
    {
        private Texture _texture;

        public TextureDrawListInstruction(Texture texture, Position position, Size size)
        {
            Position = position;
            _texture = texture;
            Size = size;
        }

        public Position Position { get; set; }
        public Size Size { get; set; }

        public void Draw(SpriteRenderer renderer, Rectangle viewport)
        {
            renderer.Draw(_texture, (Vector2) Position.CalculatePosition(viewport, Size), null, Color.White, 0,
                Vector2.Zero,
                new Vector2(Size.Width / (float) _texture.Size.Width, Size.Height / (float) _texture.Size.Height));
        }
    }
    
    public class TextDrawListInstruction : IDrawListInstruction
    {
        private string _text;
        private uint _size;
        
        public Position Position { get; set; }
        
        public Size Size { get; set; }

        public TextDrawListInstruction(Position position, string text, uint size)
        {
            Position = position;
            _text = text;
            _size = size;
        }
        
        public void Draw(SpriteRenderer renderer, Rectangle viewport)
        {
            Size size = Theme.Font.MeasureStringBBCode(_size, _text);
            Theme.Font.DrawBBCode(_size, _text, (Vector2) Position.CalculatePosition(viewport, size), Color.White);
        }
    }

    #endregion
}