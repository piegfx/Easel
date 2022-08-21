using System;
using System.Collections.Generic;
using System.Drawing;
using Easel.Entities;
using Easel.Renderers;
using Easel.Utilities;
using Pie;
using Pie.Audio;

namespace Easel.Scenes;

public abstract class Scene : IDisposable
{
    // This is the array which gets looped through on update and draw - arrays are FAR faster to loop through compared
    // to dictionaries, so we want to use the array for speed reasons.
    private Entity[] _entities;

    // This dictionary doesn't get looped through - it's purpose is to provide fast lookup to an entity in the array.
    // Instead of searching through the entire array when entity stuff is called, the dictionary just contains an array
    // index to the entity which gets returned instead.
    public Dictionary<string, int> _entityPointers;

    private int _entityCount;
    
    protected EaselGame Game => EaselGame.Instance;

    protected GraphicsDevice GraphicsDevice => EaselGame.Instance.Graphics;

    protected AudioDevice AudioDevice => EaselGame.Instance.Audio;

    protected Scene(int initialCapacity = 128)
    {
        _entities = new Entity[initialCapacity];
        _entityPointers = new Dictionary<string, int>(initialCapacity);

        Size size = EaselGame.Instance.Window.Size;
        Camera camera = new Camera(EaselMath.ToRadians(70), size.Width / (float) size.Height);
        AddEntity("Main Camera", camera);
    }

    protected internal virtual void Initialize() { }

    protected internal virtual void Update()
    {
        for (int i = 0; i < _entityCount; i++)
            _entities[i].Update();
    }

    protected internal virtual void Draw()
    {
        ForwardRenderer.ClearAll();
        
        for (int i = 0; i < _entityCount; i++)
            _entities[i].Draw();
        
        ForwardRenderer.Render();
    }

    public virtual void Dispose()
    {
        for (int i = 0; i < _entityCount; i++)
            _entities[i].Dispose();
    }

    public void AddEntity(string name, Entity entity)
    {
        entity.Name = name;
        entity.Initialize();
        
        int count = _entityCount++;
        if (count >= _entities.Length)
            Array.Resize(ref _entities, _entities.Length << 1);
        _entities[count] = entity;
        _entityPointers.Add(name, count);
    }

    public void RemoveEntity(string name)
    {
        int location = _entityPointers[name];
        _entities[location].Dispose();
        _entities[location] = null;
        GC.Collect();
        _entities[location] = _entities[_entityCount];
        _entities[_entityCount] = null;
        _entityPointers.Remove(name);
        _entityPointers.Add(name, location);
        _entityCount--;
    }

    public Entity GetEntity(string name)
    {
        if (!_entityPointers.TryGetValue(name, out int ptr))
            return null;

        return _entities[ptr];
    }

    public T GetEntity<T>(string name) where T : Entity => (T) GetEntity(name);
}