using System;
using System.Collections.Generic;
using Easel.Entities.Components;
using Easel.Scenes;
using Pie;

namespace Easel.Entities;

public class Entity : IDisposable
{
    protected EaselGame Game => EaselGame.Instance;

    protected GraphicsDevice GraphicsDevice => EaselGame.Instance.Graphics;

    protected Scene ActiveScene => SceneManager.ActiveScene;

    public string Name { get; internal set; }

    public bool Enabled;

    public Transform Transform;

    private Component[] _components;

    private Dictionary<Type, int> _componentPointers;

    private int _componentCount;

    private bool _hasInitialized;

    public Entity(int initialCapacity = 16) : this(new Transform(), initialCapacity) { }

    public Entity(Transform transform, int initialCapacity = 16)
    {
        Transform = transform;
        Enabled = true;
        _components = new Component[initialCapacity];
        _componentPointers = new Dictionary<Type, int>(initialCapacity);
    }

    protected internal virtual void Initialize()
    {
        for (int i = 0; i < _componentCount; i++)
            _components[i].Initialize();

        _hasInitialized = true;
    }

    protected internal virtual void Update()
    {
        for (int i = 0; i < _componentCount; i++)
            _components[i].Update();
    }

    protected internal virtual void Draw()
    {
        for (int i = 0; i < _componentCount; i++)
            _components[i].Draw();
    }

    public virtual void Dispose()
    {
        for (int i = 0; i < _componentCount; i++)
            _components[i].Dispose();
    }

    public void AddComponent(Component component)
    {
        Type type = component.GetType();
        if (_componentPointers.ContainsKey(type))
            throw new EaselException("Entities cannot have more than one type of each component.");

        component.Entity = this;
        if (_hasInitialized)
            component.Initialize();
        
        int count = _componentCount++;
        if (count >= _components.Length)
            Array.Resize(ref _components, _components.Length << 1);
        _components[count] = component;
        _componentPointers.Add(type, count);
    }
}