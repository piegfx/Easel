using System;
using System.Drawing;
using System.Threading;
using Easel.Graphics;
using Easel.Scenes;
using Pie;
using Pie.Audio;
using Pie.Windowing;

namespace Easel;

public class EaselGame : IDisposable
{
    private GameSettings _settings;
    private double _targetFrameTime;
    
    public Window Window { get; private set; }

    internal EaselGraphics GraphicsInternal;

    public EaselGraphics Graphics => GraphicsInternal;

    internal AudioDevice Audio;

    public AudioDevice AudioDevice => Audio;

    public bool VSync;

    public int TargetFps
    {
        get => _targetFrameTime == 0 ? 0 : (int) (1d / _targetFrameTime);
        set
        {
            if (value == 0)
                _targetFrameTime = 0;
            else
                _targetFrameTime = 1d / value;
        }
    }

    public EaselGame(GameSettings settings, Scene scene)
    {
        _settings = settings;
        VSync = settings.VSync;
        Instance = this;
        SceneManager.InitializeScene(scene);

        TargetFps = settings.TargetFps;
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
        
        Window = Window.CreateWindow(settings, _settings.Api ?? GraphicsDevice.GetBestApiForPlatform());
        GraphicsInternal = new EaselGraphics(Window);

        Input.Initialize(Window);
        Time.Initialize();
        
        Initialize();

        SpinWait sw = new SpinWait();

        while (!Window.ShouldClose)
        {
            if ((!VSync || (_targetFrameTime != 0 && TargetFps < 60)) && Time.InternalStopwatch.Elapsed.TotalSeconds <= _targetFrameTime)
            {
                sw.SpinOnce();
                continue;
            }
            sw.Reset();
            Input.Update(Window);
            Time.Update();
            Update();
            Draw();
            Graphics.PieGraphics.Present(VSync ? 1 : 0);
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

    public static EaselGame Instance;
}