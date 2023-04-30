using Easel.Headless.Scenes;
using Easel.Imgui;

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

        base.Update();
    }

    protected override void Draw()
    {
        base.Draw();
        
        ImGuiRenderer?.Draw();
    }
}