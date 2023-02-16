using Easel.Imgui;
using Easel.Scenes;

namespace Easel.Tests;

public class TestGame : EaselGame
{
    public ImGuiRenderer ImGuiRenderer;
    
    public TestGame(GameSettings settings, Scene scene) : base(settings, scene) { }

    protected override void Initialize()
    {
        ImGuiRenderer = new ImGuiRenderer();
        
        base.Initialize();

        // TODO: Fix this bug
        Graphics.PieGraphics.ResizeSwapchain(new System.Drawing.Size(1280, 720));
    }

    protected override void Update()
    {
        ImGuiRenderer.Update();
        
        base.Update();
    }

    protected override void Draw()
    {
        base.Draw();
        
        ImGuiRenderer.Draw();
    }
}