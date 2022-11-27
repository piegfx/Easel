using System;
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
    
    public bool IsClicked;

    public bool IsHovering;

    protected bool IsMouseButtonHeld;

    protected Point CalculatedScreenPos;

    public UITheme Theme;

    public Tooltip Tooltip;

    protected UIElement(Position position, Size size)
    {
        Position = position;
        Size = size;
        // UITheme is purposefully a struct, copy it for each element.
        Theme = UI.Theme;
    }

    protected internal virtual void Update(ref bool mouseTaken, Rectangle viewport)
    {
        Vector2 mousePos = Input.MousePosition;

        CalculatedScreenPos = Position.CalculatePosition(viewport, Size);

        IsClicked = false;
        IsHovering = false;
        
        if (!mouseTaken && mousePos.X >= CalculatedScreenPos.X && mousePos.X < CalculatedScreenPos.X + Size.Width &&
            mousePos.Y >= CalculatedScreenPos.Y && mousePos.Y < CalculatedScreenPos.Y + Size.Height)
        {
            mouseTaken = true;
            IsHovering = true;

            UI.CurrentTooltip = Tooltip;

            if (Input.MouseButtonDown(MouseButton.Left))
                IsMouseButtonHeld = true;
            else if (IsMouseButtonHeld)
            {
                Click?.Invoke(this);
                IsClicked = true;
                IsMouseButtonHeld = false;
            }
        }
    }

    protected internal abstract void Draw(SpriteRenderer renderer);

    public delegate void OnClick(UIElement element);
}