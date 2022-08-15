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

        while (!Window.PieWindow.ShouldClose)
        {
            Window.PieWindow.ProcessEvents();
            
            Device.Present(VSync ? 1 : 0);
        }
    }

    public void Dispose()
    {
        Device.Dispose();
        Window.Dispose();
    }
}