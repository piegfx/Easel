using System;
using Pie;

namespace Easel;

public class EaselGame : IDisposable
{
    public readonly GameWindow Window;

    internal static GraphicsDevice Device;
    public GraphicsDevice GraphicsDevice => Device;

    public bool VSync;

    public EaselGame(GameSettings settings)
    {
        Window = new GameWindow(settings);
        VSync = settings.VSync;
    }

    public void Run()
    {
        Window.Run();
        Device = Window.PieWindow.CreateGraphicsDevice();

        Input.Initialize(Window.PieWindow);
        
        Initialize();
        
        while (!Window.PieWindow.ShouldClose)
        {
            Input.Update(Window.PieWindow);
            Update();
            Draw();
            Device.Present(VSync ? 1 : 0);
        }
    }

    protected virtual void Initialize() { }

    protected virtual void Update() { }

    protected virtual void Draw() { }

    public void Dispose()
    {
        Device.Dispose();
        Window.Dispose();
    }
}