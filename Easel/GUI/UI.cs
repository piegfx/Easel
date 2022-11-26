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
    private static Dictionary<string, UIElement> _elements;
    private static List<UIElement> _reversedElements;

    public static UITheme Theme;

    static UI()
    {
        _elements = new Dictionary<string, UIElement>();
        _reversedElements = new List<UIElement>();
        
        Theme = new UITheme();
    }

    public static void Add(string id, UIElement element)
    {
        _elements.Add(id, element);
        _reversedElements.Add(element);
    }

    public static void Remove(string id)
    {
        _elements.Remove(id, out UIElement element);
        _reversedElements.Remove(element);
    }

    internal static void Update(Rectangle viewport)
    {
        bool mouseCaptured = false;
        for (int i = _reversedElements.Count - 1; i >= 0; i--)
            _reversedElements[i].Update(ref mouseCaptured, viewport);
    }

    internal static void Draw(EaselGraphics graphics)
    {
        graphics.SpriteRenderer.Begin();
        
        foreach ((_, UIElement element) in _elements)
            element.Draw(graphics.SpriteRenderer);
        
        graphics.SpriteRenderer.End();
    }
}