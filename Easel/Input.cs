using System;
using System.Collections.Generic;
using System.Numerics;
using Pie.Windowing;

namespace Easel;

public static class Input
{
    private static HashSet<Keys> _keysPressed;
    private static HashSet<Keys> _newKeys;

    static Input()
    {
        _keysPressed = new HashSet<Keys>();
        _newKeys = new HashSet<Keys>();
    }

    public static bool KeyDown(Keys key) => _keysPressed.Contains(key);

    public static bool KeyPressed(Keys key) => _newKeys.Contains(key);
    
    public static Vector2 MousePosition { get; private set; }

    internal static void Initialize(Window window)
    {
        window.KeyDown += WindowOnKeyDown;
        window.KeyUp += WindowOnKeyUp;
    }

    internal static void Update(Window window)
    {
        _newKeys.Clear();
        
        InputState state = window.ProcessEvents();
        MousePosition = state.MousePosition;
    }

    private static void WindowOnKeyDown(Keys key)
    {
        _keysPressed.Add(key);
        _newKeys.Add(key);
    }
    
    private static void WindowOnKeyUp(Keys key)
    {
        _keysPressed.Remove(key);
        _newKeys.Remove(key);
    }
}