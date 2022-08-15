using System;
using System.Drawing;
using Pie.Windowing;

namespace Easel;

public class GameWindow : IDisposable
{
    internal Window PieWindow;
    private GameSettings _settings;

    public Size Size
    {
        get => PieWindow.Size;
        set => PieWindow.Size = value;
    }

    public string Title
    {
        get => PieWindow.Title;
        set => PieWindow.Title = value;
    }

    internal GameWindow(GameSettings settings)
    {
        _settings = settings;
    }

    public void Run()
    {
        WindowSettings settings = new WindowSettings()
        {
            Size = _settings.Size,
            Title = _settings.Title,
            Resizable = _settings.Resizable
        };

        PieWindow = _settings.Api != null
            ? Window.CreateWindow(settings, _settings.Api.Value)
            : Window.CreateWindow(settings);
        
    }

    public void Dispose()
    {
        PieWindow.Dispose();
    }
}