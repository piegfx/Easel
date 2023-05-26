using System.Numerics;
using Easel.Graphics.Renderers;
using Easel.Math;
using Pie.Windowing;

namespace Easel.GUI;

public abstract class UIElement
{
    public event OnClick Click;
    
    public string Name;
    
    public Position Position;

    public Size<int> Size;

    public Style Style;

    public bool MouseTransparent;

    protected Vector2T<int> CalculatedScreenPos;

    protected bool IsHovered;
    
    protected bool IsClicked;
    
    public UIElement(string name, Position position, Size<int> size)
    {
        Name = name;
        Position = position;
        Size = size;
        Style = UI.DefaultStyle;
    }

    protected internal virtual void Update(Rectangle<int> viewport, ref bool mouseCaptured)
    {
        CalculatedScreenPos = Position.CalculatePosition(viewport, Size);

        Vector2 mousePosition = Input.MousePosition;

        if (!mouseCaptured &&
            mousePosition.X >= CalculatedScreenPos.X && mousePosition.X < CalculatedScreenPos.X + Size.Width &&
            mousePosition.Y >= CalculatedScreenPos.Y && mousePosition.Y < CalculatedScreenPos.Y + Size.Height)
        {
            mouseCaptured = true;

            IsHovered = true;

            if (Input.MouseButtonDown(MouseButton.Left))
                IsClicked = true;
            else if (IsClicked)
            {
                IsClicked = false;
                Click?.Invoke(this);
            }
        }
        else
        {
            IsClicked = false;
            IsHovered = false;
        }
    }
    
    protected internal abstract void Draw(SpriteRenderer renderer);

    public delegate void OnClick(UIElement element);
}