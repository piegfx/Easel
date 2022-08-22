using Easel.Entities;
using Easel.Scenes;
using Pie;
using Pie.Audio;

namespace Easel.Interfaces;

// Not actually an interface but the only way to have PROTECTED METHODS ARGH
public abstract class InheritableEntity
{
    protected abstract EaselGame Game { get; }
    
    protected abstract GraphicsDevice GraphicsDevice { get; }
    
    protected abstract Scene ActiveScene { get; }
    
    protected abstract AudioDevice AudioDevice { get; }
    
    protected abstract void AddEntity(string name, Entity entity);

    protected abstract void AddEntity(Entity entity);

    protected abstract void RemoveEntity(string name);

    protected abstract void RemoveEntity(Entity entity);

    protected abstract Entity GetEntity(string name);

    protected abstract T GetEntity<T>(string name) where T : Entity;

    protected abstract Entity[] GetEntitiesWithTag(string tag);
}