using System;
using System.Numerics;
using Easel.Math;
using Pie.Windowing;

namespace Easel.GUI;

public abstract class UIElement
{
    public readonly Position Position;

    public readonly Size Size;
    
    public bool IsClicked;

    public bool IsHovering;

    protected bool IsMouseButtonHeld;

    protected UIElement(Position position, Size size)
    {
        Position = position;
        Size = size;
    }

    protected internal virtual void Update(ref bool mouseTaken, Rectangle viewport)
    {
        Vector2 mousePos = Input.MousePosition;

        Point pos = Position.CalculatePosition(viewport, Size);

        IsClicked = false;
        IsHovering = false;
        
        if (!mouseTaken && mousePos.X >= pos.X && mousePos.X < pos.X + Size.Width &&
            mousePos.Y >= pos.Y && mousePos.Y < pos.Y + Size.Height)
        {
            mouseTaken = true;
            IsHovering = true;

            if (Input.MouseButtonDown(MouseButton.Left))
                IsMouseButtonHeld = true;
            else if (IsMouseButtonHeld)
            {
                IsClicked = true;
                IsMouseButtonHeld = false;
            }
        }
    }
}