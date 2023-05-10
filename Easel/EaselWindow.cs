using System.Drawing;
using Easel.Math;
using Pie.Windowing;

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

    public WindowBorder Border
    {
        get => Window.Border;
        set => Window.Border = value;
    }

    public bool Fullscreen
    {
        get => Window.Fullscreen;
        set => Window.Fullscreen = value;
    }

    public MouseState MouseState
    {
        get => Window.MouseState;
        set => Window.MouseState = value;
    }

    public bool Focused => Window.Focused;

    public void SetFullscreen(bool fullscreen, Size<int> resolution, int refreshRate = -1, int monitorIndex = 0)
    {
        Window.SetFullscreen(fullscreen, (System.Drawing.Size) resolution, refreshRate, monitorIndex);
    }

    public void Center() => Window.Center();

    public void Maximize() => Window.Maximize();

    public void Minimize() => Window.Minimize();

    public void Restore() => Window.Restore();

    internal EaselWindow(Window window)
    {
        Window = window;

        _title = window.Title;
        window.Resize += WindowOnResize;
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

    public delegate void OnResize(Size<int> newSize);
}