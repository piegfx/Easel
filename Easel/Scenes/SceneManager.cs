using System;

namespace Easel.Scenes;

public static class SceneManager
{
    private static Scene _activeScene;
    private static Scene _switchScene;

    internal static void InitializeScene(Scene scene)
    {
        _activeScene = scene;
    }

    internal static void Initialize()
    {
        _activeScene?.Initialize();
        _switchScene = null;
    }

    internal static void Update()
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

    internal static void Draw()
    {
        _activeScene?.Draw();
    }

    public static void SetScene(Scene scene)
    {
        _switchScene = scene;
    }
}