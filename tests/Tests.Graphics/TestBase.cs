using System;
using System.Diagnostics;
using Easel.Core;
using Easel.Graphics;
using Easel.Math;
using Pie;
using Pie.Windowing;
using Pie.Windowing.Events;

namespace Tests.Graphics;

public abstract class TestBase : IDisposable
{
    public Window Window;
    public Renderer Renderer;

    private string _title;

    protected TestBase(string title)
    {
        _title = title;
    }

    protected virtual void Initialize() { }

    protected virtual void Update(double dt) { }

    protected virtual void Draw(double dt) { }

    public void Run()
    {
        Logger.UseConsoleLogs();
        PieLog.DebugLog += (type, message) =>
        {
            // Ignore verbose errors cause they are exactly what they say they are, and clog up stdout.
            if (type == LogType.Verbose)
                return;
            
            Logger.LogType eType = type switch
            {
                LogType.Debug => Logger.LogType.Debug,
                LogType.Info => Logger.LogType.Info,
                LogType.Warning => Logger.LogType.Warn,
                LogType.Error => Logger.LogType.Error,
                LogType.Critical => Logger.LogType.Fatal,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            Logger.Log(eType, message);
        };
        
        Window = new WindowBuilder()
            .Size(1280, 720)
            .Title(_title)
            .Resizable()
            .GraphicsDeviceOptions(new GraphicsDeviceOptions() { Debug = true })
            .Build(out GraphicsDevice device);

        Renderer = new Renderer(device, new RendererOptions());
        
        Initialize();
        
        Stopwatch sw = Stopwatch.StartNew();

        bool wantsClose = false;
        while (!wantsClose)
        {
            while (Window.PollEvent(out IWindowEvent winEvent))
            {
                switch (winEvent)
                {
                    case QuitEvent:
                        wantsClose = true;
                        break;
                    case ResizeEvent resize:
                        Renderer.Resize(new Size<int>(resize.Width, resize.Height));
                        break;
                }
            }

            double dt = sw.Elapsed.TotalSeconds;
            sw.Restart();
            
            Update(dt);
            Draw(dt);
            
            Renderer.Present();
        }
    }

    public void Dispose()
    {
        Renderer.Dispose();
        Window.Dispose();
    }
}