using System;
using Easel.Graphics;
using Easel.Imgui;
using Easel.Scenes;
using Pie.ShaderCompiler;
using Pie.Windowing;

namespace Easel.Tests;

public class TestGame : EaselGame
{
    public ImGuiRenderer ImGuiRenderer;
    
    public TestGame(GameSettings settings, Scene scene) : base(settings, scene) { }

    protected override void Initialize()
    {
        ImGuiRenderer = new ImGuiRenderer();

        base.Initialize();
    }

    protected override void Update()
    {
        ImGuiRenderer?.Update();

        if (Input.KeyPressed(Key.F11))
        {
            if (Window.FullscreenMode != FullscreenMode.Windowed)
                Window.FullscreenMode = FullscreenMode.Windowed;
            else
                Window.FullscreenMode = FullscreenMode.BorderlessFullscreen;
        }

        base.Update();
    }

    protected override void Draw()
    {
        base.Draw();
        
        ImGuiRenderer?.Draw();
    }
}