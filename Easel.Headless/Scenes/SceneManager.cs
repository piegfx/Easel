using System;
using Easel.Core;

namespace Easel.Headless.Scenes;

public static class SceneManager
{
    private static Scene _activeScene;
    private static Scene _switchScene;

    public static Scene ActiveScene => _activeScene;

    public static void InitializeScene(Scene scene)
    {
        _activeScene = scene;
    }

    public static void Initialize()
    {
        Logger.Debug("Initializing SceneManager...");
        if (_activeScene == null)
            Logger.Info("Scene was null, will not use a scene by default.");
        else
            _activeScene.Initialize();
        _switchScene = null;
    }

    public static void Update()
    {
        if (_switchScene != null)
        {
            _activeScene?.Dispose();
            _activeScene = null;
            GC.Collect();
            _activeScene = _switchScene;
            _activeScene.Initialize();
            _switchScene = null;
        }
        
        _activeScene?.Update();
    }

    public static void AfterUpdate()
    {
        _activeScene?.AfterUpdate();
    }
    
    public static void FixedUpdate()
    {
        _activeScene?.FixedUpdate();
    }

    public static void Draw()
    {
        if (_activeScene != null)
        {
            _activeScene.Draw();
        }
    }

    /// <summary>
    /// Set the scene that will be transitioned to.
    /// </summary>
    /// <param name="scene">The scene to use.</param>
    /// <remarks>Transitioning occurs at the start of the update cycle.</remarks>
    public static void SetScene(Scene scene)
    {
        _switchScene = scene;
    }
}