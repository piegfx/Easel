using System.Collections.Generic;
using Easel.Graphics.Renderers;
using Easel.Math;

namespace Easel.GUI;

public static class UI
{
    private static Dictionary<string, UIElement> _elements;

    private static List<UIElement> _elementsList;

    public static Style DefaultStyle;

    public static Size<int>? TargetSize;

    static UI()
    {
        DefaultStyle = new Style();
        TargetSize = null;

        _elements = new Dictionary<string, UIElement>();
        _elementsList = new List<UIElement>();
    }
    
    public static void Clear()
    {
        _elements.Clear();
        _elementsList.Clear();
    }

    public static void Add(UIElement element)
    {
        _elements.Add(element.Name, element);
        _elementsList.Add(element);
    }

    internal static void Update()
    {
        Rectangle<int> viewport =
            new Rectangle<int>(Vector2T<int>.Zero, EaselGame.Instance.GraphicsInternal.MainTarget.Size);

        bool mouseCaptured = false;
        for (int i = _elementsList.Count - 1; i >= 0; i--)
            _elementsList[i].Update(viewport, ref mouseCaptured);
    }

    internal static void Draw(SpriteRenderer renderer)
    {
        renderer.Begin();
        
        for (int i = 0; i < _elements.Count; i++)
            _elementsList[i].Draw(renderer, 1.0);
        
        renderer.End();
    }
}