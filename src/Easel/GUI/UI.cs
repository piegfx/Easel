using System.Collections.Generic;
using Easel.Graphics.Renderers;
using Easel.Math;

namespace Easel.GUI;

public static class UI
{
    private static Dictionary<string, UIElement> _elements;

    private static List<UIElement> _elementsList;

    private static Size<int>? _targetSize;
    private static float _scaleMultiplier;
    private static float _calculatedScale;

    public static Style DefaultStyle;

    public static Size<int>? TargetSize
    {
        get => _targetSize;
        set
        {
            _targetSize = value;
            CalculateScale();
        }
    }

    public static float Scale
    {
        get
        {
            CalculateScale();
            return _calculatedScale * _scaleMultiplier;
        }
        set => _scaleMultiplier = value;
    }

    static UI()
    {
        DefaultStyle = new Style();
        TargetSize = null;

        _elements = new Dictionary<string, UIElement>();
        _elementsList = new List<UIElement>();

        _scaleMultiplier = 1.0f;
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
            _elementsList[i].Draw(renderer);
        
        renderer.End();
    }

    private static void CalculateScale()
    {
        if (_targetSize == null)
        {
            _calculatedScale = 1f;
            return;
        }

        Size<int> size = EaselGame.Instance.GraphicsInternal.MainTarget.Size;
        _calculatedScale = size.Height / (float) _targetSize.Value.Height;
    }
}