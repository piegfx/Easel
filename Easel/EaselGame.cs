using System;
using System.Collections.Concurrent;
using System.Threading;
using Easel.Graphics;
using Easel.Scenes;
using Easel.Utilities;
using Pie;
using Pie.Audio;
using Pie.Windowing;

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

    /// <summary>
    /// If enabled, the game will synchronize with the monitor's vertical refresh rate.
    /// </summary>
    public bool VSync;

    public bool AllowMissing;

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
        Logging.Log("New EaselGame created!");
        _settings = settings;
        VSync = settings.VSync;
        AllowMissing = settings.AllowMissing;
        if (AllowMissing)
            Logging.Info("Missing content support is enabled.");
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
        Logging.Log("Hello World! Welcome to Easel. Setting up...");

        _settings.Icon ??= new Bitmap(Utils.LoadEmbeddedResource("Easel.EaselLogo.png"));
        
        Icon icon = new Icon((uint) _settings.Icon.Size.Width, (uint) _settings.Icon.Size.Height, _settings.Icon.Data);

        WindowSettings settings = new WindowSettings()
        {
            Size = (System.Drawing.Size) _settings.Size,
            Title = _settings.Title,
            Border = _settings.Border,
            EventDriven = false,
            Icons = new []{ icon }
        };

        GraphicsDeviceOptions options = new GraphicsDeviceOptions();
        
#if DEBUG
        Logging.Info("Graphics debugging enabled.");
        options.Debug = true;
#endif
        
        Logging.Log($"Checking for {EnvVars.ForceApi}...");
        string? apistr = Environment.GetEnvironmentVariable(EnvVars.ForceApi);
        GraphicsApi api = _settings.Api ?? GraphicsDevice.GetBestApiForPlatform();
        if (apistr != null)
        {
            Logging.Log($"{EnvVars.ForceApi} environment variable set. Attempting to use \"{apistr}\".");
            if (!Enum.TryParse(apistr, true, out GraphicsApi potApi))
                Logging.Warn($"Could not parse API \"{apistr}\", reverting back to default API.");
            else
                api = potApi;
        }
        
        if ((_settings.TitleBarFlags & TitleBarFlags.ShowEasel) == TitleBarFlags.ShowEasel)
            settings.Title += " - Easel";
        if ((_settings.TitleBarFlags & TitleBarFlags.ShowGraphicsApi) == TitleBarFlags.ShowGraphicsApi)
            settings.Title += " - " + api.ToFriendlyString();
        if ((_settings.TitleBarFlags & TitleBarFlags.ShowFps) == TitleBarFlags.ShowFps)
            settings.Title += " - 0 FPS";
        
        Logging.Info($"Using {api.ToFriendlyString()} graphics API.");

        Logging.Log("Creating window...");
        Window = Window.CreateWindow(settings, api);
        Logging.Log("Creating graphics device...");
        GraphicsInternal = new EaselGraphics(Window, options);

        Logging.Log("Creating audio device...");
        AudioInternal = new AudioDevice(256);

        Logging.Log("Initializing input...");
        Input.Initialize(Window);
        Logging.Log("Initializing time...");
        Time.Initialize();
        
        Logging.Log("Initializing your application...");
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
            Graphics.PieGraphics.Present(VSync ? 1 : 0);
        }
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
        Physics.Update();
        SceneManager.PhysicsUpdate();
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
    }

    /// <summary>
    /// Dispose this game. It's recommended you use a using statement instead of manually calling this function if
    /// possible.
    /// </summary>
    public void Dispose()
    {
        Graphics.Dispose();
        Window.Dispose();
        Logging.Log("EaselGame disposed.");
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
    /// Access the current easel game instance - this is usually not needed however, since scenes and entities will
    /// include a property which references this.
    /// </summary>
    /// <remarks>Currently you can set this value. <b>Do not do this</b> unless you have a reason to, as it will screw
    /// up many other parts of the engine and it will likely stop working.</remarks>
    public static EaselGame Instance;
}