using Pie;

namespace Easel.Scenes;

public abstract class Scene
{
    protected EaselGame Game = EaselGame.Instance;

    protected GraphicsDevice GraphicsDevice => EaselGame.Device;

    protected internal virtual void Initialize() { }

    protected internal virtual void Update() { }

    protected internal virtual void Draw() { }
}