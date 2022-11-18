using System;
using Easel.Graphics.Renderers;

namespace Easel.Graphics;

public struct GCRenderable : IDisposable
{
    public TimeSpan DisposeTime;
    
    private Renderable _renderable;
    private DateTime _lastUse;
    
    public GCRenderable(in Renderable renderable)
    {
        _renderable = renderable;
        _lastUse = DateTime.Now;

        DisposeTime = TimeSpan.FromSeconds(60);
    }

    public Renderable Get()
    {
        _lastUse = DateTime.Now;
        return _renderable;
    }

    public bool TryDispose()
    {
        bool needsDispose = DateTime.Now - _lastUse >= DisposeTime;
        if (needsDispose)
            Dispose();
        return needsDispose;
    }

    public void Dispose()
    {
        _renderable.Dispose();
    }
}