using System;
using System.Drawing;
using Easel.Scenes;
using Pie;
using Pie.Audio;
using Pie.Windowing;

namespace Easel;

public class EaselGame : IDisposable
{
    private GameSettings _settings;
    
    public Window Window { get; private set; }

    internal GraphicsDevice Graphics;
    
    public GraphicsDevice GraphicsDevice => Graphics;

    internal AudioDevice Audio;

    public AudioDevice AudioDevice => Audio;

    public bool VSync;

    public EaselGame(GameSettings settings, Scene scene)
    {
        _settings = settings;
        VSync = settings.VSync;
        Instance = this;
        SceneManager.InitializeScene(scene);
    }

    public void Run()
    {
        WindowSettings settings = new WindowSettings()
        {
            Size = _settings.Size,
            Title = _settings.Title,
            Resizable = _settings.Resizable,
            EventDriven = false
        };

        GraphicsDeviceCreationFlags flags = GraphicsDeviceCreationFlags.None;
        
#if DEBUG
        flags |= GraphicsDeviceCreationFlags.Debug;
#endif
        
        Window = Window.CreateWithGraphicsDevice(settings, _settings.Api ?? GraphicsDevice.GetBestApiForPlatform(),
            out Graphics, flags);
        
        Window.Resize += PieWindowOnResize;

        Input.Initialize(Window);
        Time.Initialize();
        
        Initialize();

        while (!Window.ShouldClose)
        {
            Input.Update(Window);
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

    public static EaselGame Instance;
}