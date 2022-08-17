using System;
using System.Drawing;
using Easel.Scenes;
using Pie;

namespace Easel;

public class EaselGame : IDisposable
{
    public readonly GameWindow Window;

    internal static GraphicsDevice Device;
    public GraphicsDevice GraphicsDevice => Device;

    public bool VSync;

    public EaselGame(GameSettings settings, Scene scene)
    {
        Window = new GameWindow(settings);
        VSync = settings.VSync;
        Instance = this;
        SceneManager.InitializeScene(scene);
    }

    public void Run()
    {
        Window.Run();
        Window.PieWindow.Resize += PieWindowOnResize;
        Device = Window.PieWindow.CreateGraphicsDevice();

        Input.Initialize(Window.PieWindow);
        Time.Initialize();
        
        Initialize();
        
        while (!Window.PieWindow.ShouldClose)
        {
            Input.Update(Window.PieWindow);
            Time.Update();
            Update();
            Draw();
            Device.Present(VSync ? 1 : 0);
        }
    }

    protected virtual void Initialize()
    {
        SceneManager.Initialize();
    }

    protected virtual void Update()
    {
        SceneManager.Update();
    }

    protected virtual void Draw()
    {
        SceneManager.Draw();
    }

    public void Dispose()
    {
        Device.Dispose();
        Window.Dispose();
    }
    
    private void PieWindowOnResize(Size size)
    {
        Device.ResizeSwapchain(size);
    }

    internal static EaselGame Instance;
}