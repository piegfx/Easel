using System;

namespace Easel.Entities.Components;

public abstract class Component : IDisposable
{
    public Entity Entity { get; internal set; }

    public Transform Transform => Entity.Transform;

    protected internal virtual void Initialize() { }

    protected internal virtual void Update() { }

    protected internal virtual void Draw() { }

    public virtual void Dispose() { }
}