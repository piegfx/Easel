using System;
using System.Diagnostics;
using Easel.Core;
using Easel.Graphics;
using Pie;
using Pie.Windowing;

namespace Easel.Tests.Graphics;

public abstract class TestBase : IDisposable
{
    public Window Window;
    public Renderer Renderer;

    protected virtual void Initialize() { }

    protected virtual void Update(double dt) { }

    protected virtual void Draw(double dt) { }

    public void Run(in WindowSettings settings)
    {
        Logger.UseConsoleLogs();
        
        Logger.Debug("Creating window & graphics device.");
        Window = Window.CreateWithGraphicsDevice(settings, GraphicsDevice.GetBestApiForPlatform(),
            out GraphicsDevice device, new GraphicsDeviceOptions(true));

        Logger.Debug("Creating renderer.");
        Renderer = new Renderer(device, new RendererSettings());
        
        Logger.Debug("Initializing application.");
        Initialize();
        
        Logger.Debug("Entering render loop.");

        Stopwatch sw = Stopwatch.StartNew();
        
        while (!Window.ShouldClose)
        {
            Window.ProcessEvents();

            double dt = sw.Elapsed.TotalSeconds;
            Update(dt);
            Draw(dt);
            
            sw.Restart();
            
            Renderer.Present();
        }
    }

    public virtual void Dispose()
    {
        Renderer.Dispose();
        Window.Dispose();
    }
}