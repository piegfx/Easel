using System;
using Easel.Graphics;
using Easel.Interfaces;
using Easel.Scenes;
using Pie;
using Pie.Audio;

namespace Easel.Entities.Components;

/// <summary>
/// The base class for all entity components and scripts.
/// </summary>
public abstract class Component : InheritableEntity, IDisposable
{
    protected override EaselGame Game => EaselGame.Instance;
    
    protected override EaselGraphics Graphics => EaselGame.Instance.GraphicsInternal;
    
    protected override Scene ActiveScene => SceneManager.ActiveScene;
    
    protected override AudioDevice Audio => EaselGame.Instance.AudioInternal;
    
    protected internal Entity Entity { get; internal set; }

    /// <summary>
    /// The <see cref="Easel.Entities.Transform"/> of the current entity.
    /// </summary>
    protected Transform Transform => Entity.Transform;

    /// <summary>
    /// Called during entity initialization (usually when the entity is added to the scene), or, if the entity is already
    /// initialized, called when the component is added to the entity.
    /// </summary>
    protected internal virtual void Initialize() { }

    /// <summary>
    /// Called once per frame during update.
    /// </summary>
    protected internal virtual void Update() { }

    /// <summary>
    /// Called once per frame during draw.
    /// </summary>
    protected internal virtual void Draw() { }

    /// <summary>
    /// Called when an entity is removed from the scene, or when the scene changes.
    /// Use this to dispose of native resources.
    /// </summary>
    public virtual void Dispose() { }

    protected override void AddEntity(string name, Entity entity) => SceneManager.ActiveScene.AddEntity(name, entity);

    protected override void AddEntity(Entity entity) => SceneManager.ActiveScene.AddEntity(entity);

    protected override void RemoveEntity(string name) => SceneManager.ActiveScene.RemoveEntity(name);

    protected override void RemoveEntity(Entity entity) => SceneManager.ActiveScene.RemoveEntity(entity);

    protected override Entity GetEntity(string name) => SceneManager.ActiveScene.GetEntity(name);

    protected override T GetEntity<T>(string name) => SceneManager.ActiveScene.GetEntity<T>(name);

    protected override Entity[] GetEntitiesWithTag(string tag) => SceneManager.ActiveScene.GetEntitiesWithTag(tag);
}