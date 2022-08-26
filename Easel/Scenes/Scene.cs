using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Easel.Entities;
using Easel.Graphics;
using Easel.Interfaces;
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

    /// <summary>
    /// Contains a list of disposable objects that will be automatically disposed when the scene is disposed. Engine
    /// objects, such as textures, will be automatically added to this list on creation (unless you tell them not to).
    /// Pie objects, such as graphics buffers, however, will not, so it is recommended that you add them to this list
    /// so you don't need to remember to delete them later.
    /// </summary>
    /// <remarks>It's recommended you don't clear this list unless you remember to manually clean objects later. You
    /// also don't need to add entities to this list, they are automatically disposed separately.</remarks>
    public List<IDisposable> GarbageCollections;

    private int _entityCount;

    private int _unnamedEntityId;
    
    protected EaselGame Game => EaselGame.Instance;

    protected EaselGraphics Graphics => EaselGame.Instance.GraphicsInternal;

    protected AudioDevice AudioDevice => EaselGame.Instance.Audio;

    public readonly World World;

    protected Scene(int initialCapacity = 128)
    {
        _entities = new Entity[initialCapacity];
        _entityPointers = new Dictionary<string, int>(initialCapacity);
        GarbageCollections = new List<IDisposable>();
        World = new World();
    }

    protected internal virtual void Initialize()
    {
        Size size = EaselGame.Instance.Window.Size;
        Camera camera = new Camera(EaselMath.ToRadians(70), size.Width / (float) size.Height);
        camera.Tag = Tags.MainCamera;
        AddEntity("Main Camera", camera);
    }

    protected internal virtual void Update()
    {
        for (int i = 0; i < _entityCount; i++)
        {
            ref Entity entity = ref _entities[i];
            if (entity == null || !entity.Enabled)
                return;
            entity.Update();
        }
    }

    protected internal virtual void Draw()
    {
        ForwardRenderer.ClearAll();

        Graphics.Clear(World.ClearColor);
        
        for (int i = 0; i < _entityCount; i++)
        {
            ref Entity entity = ref _entities[i];
            if (!entity.Enabled)
                break;
            entity.Draw();
        }

        ForwardRenderer.Render();
    }

    public virtual void Dispose()
    {
        for (int i = 0; i < _entityCount; i++)
            _entities[i].Dispose();
        
        foreach (IDisposable disposable in GarbageCollections)
            disposable.Dispose();
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

    public void AddEntity(Entity entity)
    {
        entity.Name = _unnamedEntityId++.ToString();
        AddEntity(entity.Name, entity);
    }

    
    public void RemoveEntity(string name)
    {
        int location = _entityPointers[name];
        _entities[location].Dispose();
        _entities[location] = null;
        //GC.Collect();
        _entityCount--;
        _entityPointers.Remove(name);
        if (location == _entityCount)
            return;

        _entityPointers.Remove(_entities[_entityCount].Name);
        _entities[location] = _entities[_entityCount];
        _entities[_entityCount] = null;
        _entityPointers.Add(_entities[location].Name, location);
    }

    public void RemoveEntity(Entity entity) => RemoveEntity(entity.Name);

    public Entity GetEntity(string name)
    {
        if (!_entityPointers.TryGetValue(name, out int ptr))
            return null;

        return _entities[ptr];
    }

    public T GetEntity<T>(string name) where T : Entity => (T) GetEntity(name);

    public Entity[] GetEntitiesWithTag(string tag)
    {
        List<Entity> entities = new List<Entity>();
        for (int i = 0; i < _entityCount; i++)
        {
            ref Entity entity = ref _entities[i];
            if (entity.Tag == tag)
                entities.Add(entity);
        }

        return entities.ToArray();
    }
}