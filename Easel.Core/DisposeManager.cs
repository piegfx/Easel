using System;
using System.Collections.Generic;

namespace Easel.Core;

public static class DisposeManager
{
    private static List<IDisposable> _disposables;

    static DisposeManager()
    {
        _disposables = new List<IDisposable>();
    }
    
    public static void AddItem(IDisposable disposable)
    {
        _disposables.Add(disposable);
    }

    public static void ClearAll()
    {
        _disposables.Clear();
    }

    public static void DisposeAll()
    {
        for (int i = 0; i < _disposables.Count; i++)
            _disposables[i].Dispose();
        
        _disposables.Clear();
    }
}