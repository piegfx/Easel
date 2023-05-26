using System;
using System.Drawing;
using System.Numerics;
using Easel.Math;
using Pie.Windowing;
using Pie.Windowing.Events;

namespace Easel;

public class EaselWindow
{
    public event OnResize Resize;
    
    internal Window Window;

    private string _internalTitle;
    private string _title;

    public Size<int> Size
    {
        get => (Size<int>) Window.Size;
        set => Window.Size = (System.Drawing.Size) value;
    }

    public string Title
    {
        get => Window.Title;
        set
        {
            _title = value;
            Window.Title = _title + _internalTitle;
        }
    }

    public bool Visible
    {
        get => Window.Visible;
        set => Window.Visible = value;
    }

    public bool Resizable
    {
        get => Window.Resizable;
        set => Window.Resizable = value;
    }
    
    public bool Borderless
    {
        get => Window.Borderless;
        set => Window.Borderless = value;
    }

    public FullscreenMode FullscreenMode
    {
        get => Window.FullscreenMode;
        set => Window.FullscreenMode = value;
    }

    public bool Focused => Window.Focused;

    public void Focus() => Window.Focus();

    public void Center() => Window.Center();

    public void Maximize() => Window.Maximize();

    public void Minimize() => Window.Minimize();

    public void Restore() => Window.Restore();

    internal EaselWindow(Window window)
    {
        Window = window;

        _title = window.Title;
    }

    private void WindowOnResize(Size size)
    {
        Resize?.Invoke((Size<int>) size);
    }

    internal string InternalTitle
    {
        get => _internalTitle;
        set
        {
            _internalTitle = value;
            Window.Title = _title + _internalTitle;
        }
    }

    internal bool ProcessEvents()
    {
        bool wantsClose = false;
        
        while (Window.PollEvent(out IWindowEvent windowEvent))
        {
            switch (windowEvent.EventType)
            {
                case WindowEventType.Quit:
                    wantsClose = true;
                    break;
                case WindowEventType.Resize:
                    ResizeEvent resizeEvent = (ResizeEvent) windowEvent;
                    
                    Resize?.Invoke(new Size<int>(resizeEvent.Width, resizeEvent.Height));
                    break;
                case WindowEventType.KeyDown:
                    KeyEvent kdEvent = (KeyEvent) windowEvent;
                    
                    Input.AddKeyDown(kdEvent.Key);
                    break;
                case WindowEventType.KeyUp:
                    KeyEvent kuEvent = (KeyEvent) windowEvent;
                    
                    Input.AddKeyUp(kuEvent.Key);
                    break;
                case WindowEventType.KeyRepeat:
                    // TODO: Repeating keys.
                    break;
                case WindowEventType.TextInput:
                    TextInputEvent textEvent = (TextInputEvent) windowEvent;
                    
                    Input.AddTextInput(textEvent.Text);
                    break;
                case WindowEventType.MouseMove:
                    MouseMoveEvent moveEvent = (MouseMoveEvent) windowEvent;
                    
                    Input.AddMouseMove(moveEvent.MouseX, moveEvent.MouseY, moveEvent.DeltaX, moveEvent.DeltaY);
                    break;
                case WindowEventType.MouseButtonDown:
                    MouseButtonEvent mdEvent = (MouseButtonEvent) windowEvent;
                    
                    Input.AddnMouseButtonDown(mdEvent.Button);
                    break;
                case WindowEventType.MouseButtonUp:
                    MouseButtonEvent muEvent = (MouseButtonEvent) windowEvent;
                    
                    Input.AddMouseButtonUp(muEvent.Button);
                    break;
                case WindowEventType.MouseScroll:
                    MouseScrollEvent scrollEvent = (MouseScrollEvent) windowEvent;
                    
                    Input.AddScroll(new Vector2(scrollEvent.X, scrollEvent.Y));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return wantsClose;
    }

    public delegate void OnResize(Size<int> newSize);
}