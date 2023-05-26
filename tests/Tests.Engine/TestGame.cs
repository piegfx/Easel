namespace Tests.Engine;

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