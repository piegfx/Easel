using System.Collections.Generic;
using Easel.Graphics.Renderers;
using Easel.Math;

namespace Easel.GUI;

public static class UI
{
    private static Dictionary<string, UIElement> _elements;
    private static List<UIElement> _reversedElements;

    public static UITheme DefaultTheme;

    static UI()
    {
        _elements = new Dictionary<string, UIElement>();
        _reversedElements = new List<UIElement>();
        DefaultTheme = new UITheme();
    }

    public static void AddElement(string name, UIElement element)
    {
        _elements.Add(name, element);
        _reversedElements.Add(element);
    }

    internal static void Update(Rectangle viewport)
    {
        bool mouseCaptured = false;
        
        for (int i = _reversedElements.Count - 1; i >= 0; i--)
            _reversedElements[i].Update(ref mouseCaptured, viewport);
    }

    internal static void Draw(SpriteRenderer renderer)
    {
        renderer.Begin();
        foreach ((_, UIElement element) in _elements)
            element.Draw(renderer);
        renderer.End();
    }
}