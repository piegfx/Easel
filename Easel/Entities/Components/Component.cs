using System;
using Easel.Scenes;
using Pie;

namespace Easel.Entities.Components;

public abstract class Component : IDisposable
{
    protected EaselGame Game => EaselGame.Instance;

    protected GraphicsDevice GraphicsDevice => EaselGame.Instance.Graphics;
    
    protected Scene ActiveScene => SceneManager.ActiveScene;
    
    protected internal Entity Entity { get; internal set; }

    protected Transform Transform => Entity.Transform;

    protected internal virtual void Initialize() { }

    protected internal virtual void Update() { }

    protected internal virtual void Draw() { }

    public virtual void Dispose() { }
}