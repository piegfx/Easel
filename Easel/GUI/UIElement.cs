using System.Numerics;
using Easel.Graphics.Renderers;
using Easel.Math;
using Pie.Windowing;

namespace Easel.GUI;

public abstract class UIElement
{
    public event OnClick Click;
    
    public Position Position;

    public Size Size;
    
    public UITheme Theme;

    protected bool IsHovered;
    
    protected bool IsClicked;

    protected Point ResolvedPosition;

    protected UIElement(Position position, Size size)
    {
        Position = position;
        Size = size;
        Theme = UI.DefaultTheme;
    }

    protected internal virtual void Update(ref bool mouseCaptured, Rectangle viewport)
    {
        ResolvedPosition = Position.CalculatePosition(viewport, Size);
        Vector2 mousePos = Input.MousePosition;

        if (!mouseCaptured && mousePos.X >= ResolvedPosition.X && mousePos.X < ResolvedPosition.X + Size.Width &&
            mousePos.Y >= ResolvedPosition.Y && mousePos.Y < ResolvedPosition.Y + Size.Height)
        {
            mouseCaptured = true;
            IsHovered = true;
            if (Input.MouseButtonDown(MouseButton.Left))
                IsClicked = true;
            else if (IsClicked)
            {
                Click?.Invoke();
                IsClicked = false;
            }
        }
        else
        {
            IsHovered = false;
            IsClicked = false;
        }
    }

    protected internal virtual void Draw(SpriteRenderer renderer) { }

    public delegate void OnClick();
}