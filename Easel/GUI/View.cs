using System.Collections.Generic;
using System.Numerics;
using Easel.Graphics.Renderers;
using Easel.Math;

namespace Easel.GUI;

public class View : UIElement
{
    private Dictionary<string, UIElement> _elements;
    private List<UIElement> _reversedElements;

    public bool Transparent;

    public View(Position position, Size size) : base(position, size)
    {
        _elements = new Dictionary<string, UIElement>();
        _reversedElements = new List<UIElement>();
    }

    public void AddElement(string name, UIElement element)
    {
        _elements.Add(name, element);
        _reversedElements.Add(element);
    }

    protected internal override void Update(ref bool mouseCaptured, Rectangle viewport)
    {
        base.Update(ref mouseCaptured, viewport);
        
        for (int i = _reversedElements.Count - 1; i >= 0; i--)
            _reversedElements[i].Update(ref mouseCaptured, viewport);
    }

    protected internal override void Draw(SpriteRenderer renderer)
    {
        base.Draw(renderer);

        if (!Transparent)
        {
            Color color = Color.White;

            if (IsHovered)
                color = Color.Gray;

            renderer.DrawRectangle((Vector2) ResolvedPosition, Size, color, 0, Vector2.Zero);
        }

        foreach (UIElement element in _reversedElements)
            element.Draw(renderer);
    }
}