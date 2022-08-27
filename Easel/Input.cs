using System;
using System.Collections.Generic;
using System.Numerics;
using Pie.Windowing;

namespace Easel;

/// <summary>
/// Provides functions to query input states for the current frame.
/// </summary>
public static class Input
{
    private static HashSet<Keys> _keysPressed;
    private static HashSet<Keys> _newKeys;

    static Input()
    {
        _keysPressed = new HashSet<Keys>();
        _newKeys = new HashSet<Keys>();
    }

    /// <summary>
    /// Query if the given key is currently held down.
    /// </summary>
    /// <param name="key">The key to query.</param>
    /// <returns><see langword="true"/>, if the key is currently held.</returns>
    public static bool KeyDown(Keys key) => _keysPressed.Contains(key);

    /// <summary>
    /// Query if the given key was pressed this frame.
    /// </summary>
    /// <param name="key">The key to query.</param>
    /// <returns><see langword="true"/>, if the key was pressed this frame.</returns>
    public static bool KeyPressed(Keys key) => _newKeys.Contains(key);
    
    /// <summary>
    /// Get the current mouse position relative to the window (top left = 0, 0)
    /// </summary>
    public static Vector2 MousePosition { get; private set; }
    
    /// <summary>
    /// Returns the number of pixels the mouse has moved since the last frame.
    /// </summary>
    public static Vector2 DeltaMousePosition { get; private set; }

    private static MouseState _currentMouseState;
    private static bool _mouseStateChanged;

    /// <summary>
    /// Get or set the mouse state for the current game window.
    /// </summary>
    public static MouseState MouseState
    {
        get => _currentMouseState;
        set
        {
            _currentMouseState = value;
            _mouseStateChanged = true;
        }
    }

    internal static void Initialize(Window window)
    {
        window.KeyDown += WindowOnKeyDown;
        window.KeyUp += WindowOnKeyUp;
    }

    internal static void Update(Window window)
    {
        _newKeys.Clear();
        
        InputState state = window.ProcessEvents();
        DeltaMousePosition = state.MousePosition - MousePosition;
        MousePosition = state.MousePosition;

        if (_mouseStateChanged)
        {
            window.MouseState = _currentMouseState;
            _mouseStateChanged = false;
        }
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