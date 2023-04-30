using System;
using System.Reflection;
using System.Threading;
using Easel.Core;
using Easel.Headless.Physics;
using Easel.Headless.Scenes;

namespace Easel.Headless;

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
    
    private bool _shouldClose;

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

        Logger.Debug("Initializing physics...");
        // TODO: PhysicsInitSettings in the GameSettings.
        Simulation = new Simulation(new PhysicsInitSettings());
        
        Logger.Debug("Initializing time...");
        Time.Initialize();
        
        Logger.Debug("Initializing your application...");
        Initialize();

        SpinWait sw = new SpinWait();

        while (!_shouldClose)
        {
            if (_targetFrameTime != 0 && TargetFps < 60 && Time.InternalStopwatch.Elapsed.TotalSeconds <= _targetFrameTime)
            {
                sw.SpinOnce();
                continue;
            }

            sw.Reset();
            Time.Update();
            FixedUpdate();
            Update();
            AfterUpdate();
            Draw();
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
        SceneManager.Draw();
    }

    /// <summary>
    /// Dispose this game. It's recommended you use a using statement instead of manually calling this function if
    /// possible.
    /// </summary>
    public void Dispose()
    {
        SceneManager.ActiveScene?.Dispose();
        Logger.Debug("EaselGame disposed.");
    }

    /// <summary>
    /// The game will close & stop running when it is safe to do so.
    /// </summary>
    public void Close()
    {
        _shouldClose = true;
    }

    /// <summary>
    /// Access the current easel game instance - this is usually not needed however, since scenes and entities will
    /// include a property which references this.
    /// </summary>
    /// <remarks>Currently you can set this value. <b>Do not do this</b> unless you have a reason to, as it will screw
    /// up many other parts of the engine and it will likely stop working.</remarks>
    public static EaselGame Instance;
}