using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.Math;

namespace Easel.GUI;

public static class UI
{
    public static UITheme Theme;

    public static int CurrentId;

    static UI()
    {
        _drawList = new List<IDrawListInstruction>();
        _elements = new List<UIElement>();
        Theme = new UITheme();
    }

    internal static void BeforeUpdate()
    {
        // I hate this but it should work??
        _lastElements = _elements.ToList();
        CurrentId = 0;
        _elements.Clear();
        _drawList.Clear();
    }

    internal static void Update(Rectangle viewport)
    {
        bool mouseCaptured = false;

        for (int i = _elements.Count - 1; i >= 0; i--)
            _elements[i].Update(ref mouseCaptured, viewport);
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
    private static List<UIElement> _lastElements;
    private static List<UIElement> _elements;

    public static void AddElement(UIElement element, IDrawListInstruction instruction)
    {
        _elements.Add(element);
        _drawList.Add(instruction);
        CurrentId++;
    }

    public static T GetElement<T>(int id) where T : UIElement
    {
        if (id >= _lastElements.Count || id < 0 || _lastElements[id].GetType() != typeof(T))
            return null;
        return (T) _lastElements[id];
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
        private Color _color;

        public TextureDrawListInstruction(Texture texture, Position position, Size size, Color color)
        {
            Position = position;
            _texture = texture;
            _color = color;
            Size = size;
        }

        public Position Position { get; set; }
        public Size Size { get; set; }

        public void Draw(SpriteRenderer renderer, Rectangle viewport)
        {
            renderer.Draw(_texture, (Vector2) Position.CalculatePosition(viewport, Size), null, _color, 0,
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