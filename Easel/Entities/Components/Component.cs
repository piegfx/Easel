using System;
using Easel.Interfaces;
using Easel.Scenes;
using Pie;
using Pie.Audio;

namespace Easel.Entities.Components;

public abstract class Component : InheritableEntity, IDisposable
{
    protected override EaselGame Game => EaselGame.Instance;

    protected override GraphicsDevice GraphicsDevice => EaselGame.Instance.Graphics;
    
    protected override Scene ActiveScene => SceneManager.ActiveScene;
    
    protected override AudioDevice AudioDevice => EaselGame.Instance.Audio;
    
    protected internal Entity Entity { get; internal set; }

    protected Transform Transform => Entity.Transform;

    protected internal virtual void Initialize() { }

    protected internal virtual void Update() { }

    protected internal virtual void Draw() { }

    public virtual void Dispose() { }

    protected override void AddEntity(string name, Entity entity) => SceneManager.ActiveScene.AddEntity(name, entity);

    protected override void AddEntity(Entity entity) => SceneManager.ActiveScene.AddEntity(entity);

    protected override void RemoveEntity(string name) => SceneManager.ActiveScene.RemoveEntity(name);

    protected override void RemoveEntity(Entity entity) => SceneManager.ActiveScene.RemoveEntity(entity);

    protected override Entity GetEntity(string name) => SceneManager.ActiveScene.GetEntity(name);

    protected override T GetEntity<T>(string name) => SceneManager.ActiveScene.GetEntity<T>(name);

    protected override Entity[] GetEntitiesWithTag(string tag) => SceneManager.ActiveScene.GetEntitiesWithTag(tag);
}