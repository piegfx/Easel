using System;
using System.Collections.Concurrent;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Threading;
using Easel.Audio;
using Easel.Content;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.GUI;
using Easel.Math;
using Easel.Scenes;
using Easel.Utilities;
using Pie;
using Pie.Windowing;
using Window = Pie.Windowing.Window;

namespace Easel;

/// <summary>
/// The main game required for an Easel application to function. Initializes key objects such as the graphics device and
/// audio device, as well as providing a window, scene management, and all initialize, update and draw functions.
///
/// Currently only one instance of <see cref="EaselGame"/> is officially supported per application, however it may be
/// possible to run multiple instances, but this is not officially supported.
/// </summary>
public class EaselGame : IDisposable
{
    private GameSettings _settings;
    private double _targetFrameTime;

    public static readonly Version Version = Assembly.GetExecutingAssembly().GetName().Version;

    /// <summary>
    /// The underlying game window. Access this to change its size, title, and subscribe to various events.
    /// </summary>
    public Window Window { get; private set; }

    internal EaselGraphics GraphicsInternal;

    /// <summary>
    /// The graphics device for this EaselGame.
    /// </summary>
    public EaselGraphics Graphics => GraphicsInternal;

    internal AudioDevice AudioInternal;

    /// <summary>
    /// The audio device for this EaselGame.
    /// </summary>
    public AudioDevice Audio => AudioInternal;

    public ContentManager Content;

    /// <summary>
    /// If enabled, the game will synchronize with the monitor's vertical refresh rate.
    /// </summary>
    public bool VSync;

    public bool AllowMissing;
    
    public bool ShowMetrics;

    private ConcurrentBag<Action> _actions;

    /// <summary>
    /// The target frames per second of the application.
    /// </summary>
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

    /// <summary>
    /// Create a new EaselGame instance, with the initial settings and scene, if any.
    /// </summary>
    /// <param name="settings">The game settings that will be used on startup. Many of these can be changed at runtime.</param>
    /// <param name="scene">The initial scene that the game will load, if any. Set as <see langword="null" /> if you do
    /// not wish to start with a scene.</param>
    /// <remarks>This does <b>not</b> initialize any of Easel's core objects. You must call <see cref="Run"/> to initialize
    /// them.</remarks>
    public EaselGame(GameSettings settings, Scene scene)
    {
        Logger.Debug("New EaselGame created!");
        _settings = settings;
        VSync = settings.VSync;
        AllowMissing = settings.AllowMissing;
        if (AllowMissing)
            Logger.Info("Missing content support is enabled.");
        Instance = this;
        SceneManager.InitializeScene(scene);

        TargetFps = settings.TargetFps;
        _actions = new ConcurrentBag<Action>();
    }

    /// <summary>
    /// Run the game! This will initialize scenes, windows, graphics devices, etc.
    /// </summary>
    public void Run()
    {
        Logger.Debug("Hello World! Easel is setting up...");
        
        Logger.Info("System information:");
        Logger.Info("\tCPU: " + SystemInfo.CpuInfo);
        Logger.Info("\tMemory: " + SystemInfo.MemoryInfo);
        Logger.Info("\tLogical threads: " + SystemInfo.LogicalThreads);
        Logger.Info("\tOS: " + Environment.OSVersion.VersionString);
        

        _settings.Icon ??= new Bitmap(Utils.LoadEmbeddedResource("Easel.EaselLogo.png"));
        
        Icon icon = new Icon((uint) _settings.Icon.Size.Width, (uint) _settings.Icon.Size.Height, _settings.Icon.Data);

        WindowSettings settings = new WindowSettings()
        {
            Size = (System.Drawing.Size) _settings.Size,
            Title = _settings.Title,
            Border = _settings.Border,
            EventDriven = false,
            Icons = new []{ icon },
            StartVisible = _settings.StartVisible
        };

        GraphicsDeviceOptions options = new GraphicsDeviceOptions();
        
#if DEBUG
        Logger.Info("Graphics debugging enabled.");
        options.Debug = true;
#endif
        
        Logger.Debug($"Checking for {EnvVars.ForceApi}...");
        string? apistr = Environment.GetEnvironmentVariable(EnvVars.ForceApi);
        GraphicsApi api = _settings.Api ?? GraphicsDevice.GetBestApiForPlatform();
        if (apistr != null)
        {
            Logger.Debug($"{EnvVars.ForceApi} environment variable set. Attempting to use \"{apistr}\".");
            if (!Enum.TryParse(apistr, true, out GraphicsApi potApi))
                Logger.Warn($"Could not parse API \"{apistr}\", reverting back to default API.");
            else
                api = potApi;
        }
        
        if ((_settings.TitleBarFlags & TitleBarFlags.ShowEasel) == TitleBarFlags.ShowEasel)
            settings.Title += " - Easel";
        if ((_settings.TitleBarFlags & TitleBarFlags.ShowGraphicsApi) == TitleBarFlags.ShowGraphicsApi)
            settings.Title += " - " + api.ToFriendlyString();
        if ((_settings.TitleBarFlags & TitleBarFlags.ShowFps) == TitleBarFlags.ShowFps)
            settings.Title += " - 0 FPS";
        
        Logger.Info($"Using {api.ToFriendlyString()} graphics API.");

        Logger.Debug("Creating window...");
        Window = Window.CreateWindow(settings, api);
        Logger.Debug("Creating graphics device...");
        GraphicsInternal = new EaselGraphics(Window, options);
        GraphicsInternal.Initialize(new ForwardRenderer(GraphicsInternal), new Default2DRenderer(GraphicsInternal));

        Logger.Debug("Creating audio device...");
        AudioInternal = new AudioDevice(48000, 256);

        Logger.Debug("Initializing input...");
        Input.Initialize(Window);
        Logger.Debug("Initializing time...");
        Time.Initialize();
        
        Logger.Debug("Creating content manager...");
        Content = new ContentManager();
        
        Logger.Debug("Initializing your application...");
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
            Metrics.Update();
            Update();
            // TODO: Fix pie
            GraphicsInternal.SetRenderTarget(null);
            Draw();
            if (ShowMetrics)
                DrawMetrics();
            Graphics.PieGraphics.Present(VSync ? 1 : 0);
        }
        
        Logger.Debug("Close requested, shutting down...");
    }

    /// <summary>
    /// Gets called on game initialization. Where you call the base function will determine when the rest of Easel
    /// (except for core objects) get initialized.
    /// </summary>
    protected virtual void Initialize()
    {
        SceneManager.Initialize();
    }

    /// <summary>
    /// Gets called on game update. Where you call the base function will determine when Easel updates the current scene.
    /// </summary>
    protected virtual void Update()
    {
        SceneManager.Update();
        AudioEffect.Update();
        UI.Update(GraphicsInternal.Viewport);
    }

    /// <summary>
    /// Gets called on game draw. Where you call the base function will determine when Easel draws the current scene,
    /// as well as when it executes any <see cref="RunOnMainThread"/> calls.
    /// </summary>
    protected virtual void Draw()
    {
        SceneManager.Draw();
        foreach (Action action in _actions)
            action();
        _actions.Clear();
        UI.Draw(GraphicsInternal);
    }

    /// <summary>
    /// Dispose this game. It's recommended you use a using statement instead of manually calling this function if
    /// possible.
    /// </summary>
    public void Dispose()
    {
        SceneManager.ActiveScene?.Dispose();
        Graphics.Dispose();
        Window.Dispose();
        Logger.Debug("EaselGame disposed.");
    }

    /// <summary>
    /// Run the given code on the main thread - useful for graphics calls which <b>cannot</b> run on any other thread.
    /// These actions are processed at the end of <see cref="Draw"/>.
    /// </summary>
    /// <param name="code"></param>
    public void RunOnMainThread(Action code)
    {
        _actions.Add(code);
    }

    /// <summary>
    /// The game will close & stop running when it is safe to do so.
    /// </summary>
    public void Close()
    {
        Window.ShouldClose = true;
    }

    /// <summary>
    /// Access the current easel game instance - this is usually not needed however, since scenes and entities will
    /// include a property which references this.
    /// </summary>
    /// <remarks>Currently you can set this value. <b>Do not do this</b> unless you have a reason to, as it will screw
    /// up many other parts of the engine and it will likely stop working.</remarks>
    public static EaselGame Instance;

    private void DrawMetrics()
    {
        string metrics = Metrics.GetString();
        Graphics.SpriteRenderer.Begin();
        Font font = UI.Theme.Font;
        Size size = font.MeasureString(12, metrics);
        //Graphics.SpriteRenderer.DrawRectangle(Vector2.Zero, size + new Size(10), new Color(Color.Black, 0.5f), 0, Vector2.Zero);
        font.Draw(12, metrics, new Vector2(5), Color.White);
        Graphics.SpriteRenderer.End();
    }
}