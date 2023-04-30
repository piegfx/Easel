﻿using System;
using System.Numerics;
using System.Reflection;
using System.Threading;
using Easel.Core;
using Easel.Math;
using Easel.Headless.Physics;
using Easel.Headless.Scenes;
using Easel.Audio;
using Easel.Content;
using Easel.Content.Builder;
using Easel.Graphics;
using Easel.GUI;
using Pie;
using Pie.Windowing;
using System.Collections.Concurrent;
using System.IO;
using Easel.Headless;
using Monitor = Pie.Windowing.Monitor;
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

    internal EaselAudio AudioInternal;

    /// <summary>
    /// The audio device for this EaselGame.
    /// </summary>
    public EaselAudio Audio => AudioInternal;

    public ContentManager Content;
    
    public bool AllowMissing;
    
    public bool ShowMetrics;

    private ConcurrentBag<Action> _actions;
    
    public Simulation Simulation;

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
        AllowMissing = settings.AllowMissing;
        if (AllowMissing)
            Logger.Info("Missing content support is enabled.");
        _actions = new ConcurrentBag<Action>();
        Instance = this;
        SceneManager.InitializeScene(scene);

        TargetFps = settings.TargetFps;
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

        _settings.Icon ??=
            new Bitmap(Utils.LoadEmbeddedResource(Assembly.GetExecutingAssembly(), "Easel.EaselLogo.png"));

        Icon icon = new Icon((uint) _settings.Icon.Size.Width, (uint) _settings.Icon.Size.Height,
            _settings.Icon.Data);

        System.Drawing.Size size = (System.Drawing.Size) _settings.Size;
        if (size.Width == -1 && size.Height == -1)
            size = Monitor.PrimaryMonitor.VideoMode.Size;

        WindowSettings settings = new WindowSettings()
        {
            Size = size,
            Title = _settings.Title,
            Border = _settings.Border,
            EventDriven = false,
            Icons = new[] { icon },
            StartVisible = false
        };

        GraphicsDeviceOptions options = new GraphicsDeviceOptions();

        if (_settings.RenderOptions.GraphicsDebugging)
        {
            Logger.Info("Graphics debugging enabled.");
            options.Debug = true;
        }

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
        PieLog.DebugLog += PieDebug;
        Window = Window.CreateWithGraphicsDevice(settings, api, out GraphicsDevice device, options);
        Window.SetFullscreen(_settings.Fullscreen, size, Monitor.PrimaryMonitor.VideoMode.RefreshRate);
        Window.Visible = _settings.StartVisible;

        Logger.Debug("Creating graphics device...");
        GraphicsInternal = new EaselGraphics(device, _settings.RenderOptions);
        GraphicsInternal.VSync = _settings.VSync;
        Window.Resize += WindowOnResize;

        Logger.Debug("Creating audio device...");
        AudioInternal = new EaselAudio();

        Logger.Debug("Initializing input...");
        Input.Initialize(Window);

        Logger.Debug("Creating content manager...");
        Content = new ContentManager();

        if (_settings.AutoGenerateContentDirectory != null)
        {
            Logger.Info("Auto-generate content is enabled. Generating...");
            try
            {
                ContentDefinition definition = ContentBuilder.FromDirectory(_settings.AutoGenerateContentDirectory)
                    .Build(DuplicateHandling.Ignore);
                Content.AddContent(definition);
            }
            catch (DirectoryNotFoundException)
            {
                Logger.Warn(
                    $"A directory called \"{_settings.AutoGenerateContentDirectory}\" was not found. Either create it, or change \"GameSettings.AutoGenerateContentDirectory\" to a valid content directory, or to \"null\" to disable auto content generation.");
            }
        }

        Logger.Debug("Initializing physics...");
        // TODO: PhysicsInitSettings in the GameSettings.
        Simulation = new Simulation(new PhysicsInitSettings());
        
        Logger.Debug("Initializing time...");
        Time.Initialize();
        
        Logger.Debug("Initializing your application...");
        Initialize();

        SpinWait sw = new SpinWait();
        
        while (!Window.ShouldClose)
        {
            if ((!Graphics.VSync || (_targetFrameTime != 0 && TargetFps < 60)) && Time.InternalStopwatch.Elapsed.TotalSeconds <= _targetFrameTime)
            {
                sw.SpinOnce();
                continue;
            }

            sw.Reset();
            Input.Update(Window);
            Time.Update();
            Metrics.Update();
            UI.Update();
            // TODO: Fixed update on separate thread/improved render loop?
            // For now fixed update just runs before every update cycle.
            FixedUpdate();
            Update();
            AfterUpdate();
            Draw();
            UI.Draw(GraphicsInternal.SpriteRenderer);
            if (ShowMetrics)
                DrawMetrics();
            GraphicsInternal.Present();
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
    }

    protected virtual void AfterUpdate()
    {
        SceneManager.AfterUpdate();
    }

    protected virtual void FixedUpdate()
    {
        Simulation.Timestep(1 / 60f);
        SceneManager.FixedUpdate();
    }

    /// <summary>
    /// Gets called on game draw. Where you call the base function will determine when Easel draws the current scene,
    /// as well as when it executes any <see cref="RunOnMainThread"/> calls.
    /// </summary>
    protected virtual void Draw()
    {
        foreach (Action action in _actions)
            action();
        _actions.Clear();

        SceneManager.Draw();
    }

    /// <summary>
    /// Dispose this game. It's recommended you use a using statement instead of manually calling this function if
    /// possible.
    /// </summary>
    public void Dispose()
    {
        SceneManager.ActiveScene?.Dispose();
        GraphicsInternal.Dispose();
        Window.Dispose();
        
        Logger.Debug("EaselGame disposed.");
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
    
    /// <summary>
    /// Run the given code on the main thread - useful for graphics calls which <b>cannot</b> run on any other thread.
    /// These actions are processed at the end of <see cref="Draw"/>.
    /// </summary>
    /// <param name="code"></param>
    public void RunOnMainThread(Action code)
    {
        _actions.Add(code);
    }

    private void DrawMetrics()
    {
        string metrics = Metrics.GetString();
        GraphicsInternal.SpriteRenderer.Begin();
        Font font = UI.DefaultStyle.Font;
        Size<int> size = font.MeasureString(12, metrics);
        //Graphics.SpriteRenderer.DrawRectangle(Vector2T.Zero, size + new Size(10), new Color(Color.Black, 0.5f), 0, Vector2T.Zero);
        font.Draw(GraphicsInternal.SpriteRenderer, 12, metrics, new Vector2T<int>(5), Color.White, 0,
            Vector2.Zero, Vector2.One);
        Graphics.SpriteRenderer.End();
    }
    
    private void WindowOnResize(System.Drawing.Size size)
    {
        GraphicsInternal.ResizeGraphics((Size<int>) size);
    }
    
    private void PieDebug(LogType logtype, string message)
    {
        //if (logtype == LogType.Debug)
        //    return;
        Logger.Log((Logger.LogType) logtype, message);
    }
}