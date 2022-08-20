using System;
using System.Drawing;
using Easel.Scenes;
using Pie;
using Pie.Audio;

namespace Easel;

public class EaselGame : IDisposable
{
    public readonly GameWindow Window;

    internal static GraphicsDevice Graphics;
    
    public GraphicsDevice GraphicsDevice => Graphics;

    internal static AudioDevice Audio;

    public AudioDevice AudioDevice => Audio;

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
        Graphics = Window.PieWindow.CreateGraphicsDevice();

        Input.Initialize(Window.PieWindow);
        Time.Initialize();
        
        Initialize();
        
        while (!Window.PieWindow.ShouldClose)
        {
            Input.Update(Window.PieWindow);
            Time.Update();
            Update();
            Draw();
            Graphics.Present(VSync ? 1 : 0);
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
        Graphics.Dispose();
        Window.Dispose();
    }
    
    private void PieWindowOnResize(Size size)
    {
        Graphics.ResizeSwapchain(size);
    }

    internal static EaselGame Instance;
}