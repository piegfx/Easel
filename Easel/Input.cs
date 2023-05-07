using System;
using System.Collections.Generic;
using System.Numerics;
using Easel.Math;
using Pie.Windowing;
using MouseButton = Pie.Windowing.MouseButton;

namespace Easel;

/// <summary>
/// Provides functions to query input states for the current frame.
/// </summary>
public static class Input
{
    public static event OnKeyDown NewKeyDown;
    
    public static event OnKeyUp KeyUp;
    
    public static event OnMouseButtonDown MouseDown;
    
    public static event OnMouseButtonUp MouseUp;
    
    public static event OnScroll Scroll;

    public static event OnMouseMove MouseMove;

    public static event OnTextInput TextInput;
    
    private static HashSet<Key> _keysPressed;
    private static HashSet<Key> _newKeys;

    private static HashSet<MouseButton> _mouseButtonsPressed;
    private static HashSet<MouseButton> _newMouseButtons;

    private static string _currentInputScene;
    private static Dictionary<string, InputScene> _scenes;

    public static string CurrentScene
    {
        get => _currentInputScene;
        set
        {
            _currentInputScene = value;
            InputScene scene = _scenes[value];
            MouseState = scene.Options.MouseState;
        }
    }

    static Input()
    {
        _keysPressed = new HashSet<Key>();
        _newKeys = new HashSet<Key>();

        _mouseButtonsPressed = new HashSet<MouseButton>();
        _newMouseButtons = new HashSet<MouseButton>();

        _scenes = new Dictionary<string, InputScene>();
    }

    /// <summary>
    /// Query if the given key is currently held down.
    /// </summary>
    /// <param name="key">The key to query.</param>
    /// <returns><see langword="true"/>, if the key is currently held.</returns>
    public static bool KeyDown(Key key) => _keysPressed.Contains(key);

    public static bool AnyKeyDown(out Key pressedKey, params Key[] keys)
    {
        foreach (Key key in keys)
        {
            if (_keysPressed.Contains(key))
            {
                pressedKey = key;
                return true;
            }
        }

        pressedKey = Key.Unknown;
        return false;
    }

    public static bool AnyKeyDown(params Key[] keys) => AnyKeyDown(out _, keys);
    
    public static bool AnyKeyPressed(out Key pressedKey, params Key[] keys)
    {
        foreach (Key key in keys)
        {
            if (_newKeys.Contains(key))
            {
                pressedKey = key;
                return true;
            }
        }

        pressedKey = Key.Unknown;
        return false;
    }

    public static bool AnyKeyPressed(params Key[] keys) => AnyKeyPressed(out _, keys);

    /// <summary>
    /// Query if the given key was pressed this frame.
    /// </summary>
    /// <param name="key">The key to query.</param>
    /// <returns><see langword="true"/>, if the key was pressed this frame.</returns>
    public static bool KeyPressed(Key key) => _newKeys.Contains(key);

    /// <summary>
    /// Query if the given mouse button is currently held down.
    /// </summary>
    /// <param name="button">The mouse button to query.</param>
    /// <returns><see langword="true"/>, if the mouse button is currently held.</returns>
    public static bool MouseButtonDown(MouseButton button) => _mouseButtonsPressed.Contains(button);

    /// <summary>
    /// Query if the given mouse button was pressed this frame.
    /// </summary>
    /// <param name="button">The mouse button to query.</param>
    /// <returns><see langword="true"/>, if the mouse button was pressed this frame.</returns>
    public static bool MouseButtonPressed(MouseButton button) => _newMouseButtons.Contains(button);
    
    /// <summary>
    /// Get the current mouse position relative to the view (top left = 0, 0)
    /// </summary>
    public static Vector2 MousePosition { get; private set; }
    
    /// <summary>
    /// Returns the number of pixels the mouse has moved since the last frame.
    /// </summary>
    public static Vector2 DeltaMousePosition { get; private set; }

    private static MouseState _currentMouseState;
    private static bool _mouseStateChanged;

    /// <summary>
    /// Get or set the mouse state for the current game view.
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
    
    public static Vector2 ScrollWheelDelta { get; private set; }

    public static void CreateScene(string name, InputSceneOptions options)
    {
        _scenes.Add(name, new InputScene(name, options));
    }

    public static InputScene GetScene(string name) => _scenes[name];

    internal static void Initialize(Window window)
    {
        window.KeyDown += WindowOnKeyDown;
        window.KeyUp += WindowOnKeyUp;
        window.MouseButtonDown += WindowOnMouseButtonDown;
        window.MouseButtonUp += WindowOnMouseButtonUp;
        window.Scroll += WindowOnScroll;
        window.MouseMove += WindowOnMouseMove;
        window.TextInput += WindowOnTextInput;

        InputState state = window.ProcessEvents();
        MousePosition = (Vector2) state.MousePosition;
    }

    internal static void Update(Window window)
    {
        _newKeys.Clear();
        _newMouseButtons.Clear();
        
        ScrollWheelDelta = Vector2.Zero;

        InputState state = window.ProcessEvents();
        DeltaMousePosition = (Vector2) state.MousePosition - MousePosition;
        MousePosition = (Vector2) state.MousePosition;

        if (_mouseStateChanged)
        {
            window.MouseState = _currentMouseState;
            _mouseStateChanged = false;
        }
    }

    private static void WindowOnKeyDown(Key key)
    {
        _keysPressed.Add(key);
        _newKeys.Add(key);
        NewKeyDown?.Invoke(key);
    }
    
    private static void WindowOnKeyUp(Key key)
    {
        _keysPressed.Remove(key);
        _newKeys.Remove(key);
        KeyUp?.Invoke(key);
    }
    
    private static void WindowOnMouseButtonDown(MouseButton button)
    {
        _mouseButtonsPressed.Add(button);
        _newMouseButtons.Add(button);
        MouseDown?.Invoke(button);
    }
    
    private static void WindowOnMouseButtonUp(MouseButton button)
    {
        _mouseButtonsPressed.Remove(button);
        _newMouseButtons.Remove(button);
        MouseUp?.Invoke(button);
    }
    
    private static void WindowOnScroll(System.Numerics.Vector2 scroll)
    {
        ScrollWheelDelta += (Vector2) scroll;
        Scroll?.Invoke(scroll);
    }
    
    private static void WindowOnMouseMove(Vector2 position)
    {
        MouseMove?.Invoke(position);
    }
    
    private static void WindowOnTextInput(char c)
    {
        TextInput?.Invoke(c);
    }
    
    public struct InputSceneOptions
    {
        public MouseState MouseState;
    }
    
    public class InputScene
    {
        internal readonly string Name;

        public InputSceneOptions Options;

        public bool KeyDown(Key key)
        {
            if (Name != Input.CurrentScene)
                return false;

            return Input.KeyDown(key);
        }

        public bool AnyKeyDown(out Key pressedKey, params Key[] keys)
        {
            pressedKey = Key.Unknown;

            if (Name != Input.CurrentScene)
                return false;

            return Input.AnyKeyDown(out pressedKey, keys);
        }

        public bool AnyKeyDown(params Key[] keys)
        {
            if (Name != Input.CurrentScene)
                return false;

            return Input.AnyKeyDown(keys);
        }
        
        public bool AnyKeyPressed(out Key pressedKey, params Key[] keys)
        {
            pressedKey = Key.Unknown;

            if (Name != Input.CurrentScene)
                return false;

            return Input.AnyKeyPressed(out pressedKey, keys);
        }

        public bool AnyKeyPressed(params Key[] keys)
        {
            if (Name != Input.CurrentScene)
                return false;

            return Input.AnyKeyPressed(keys);
        }

        public bool KeyPressed(Key key)
        {
            if (Name != Input.CurrentScene)
                return false;

            return Input.KeyPressed(key);
        }
        
        public bool MouseButtonDown(MouseButton button)
        {
            if (Name != Input.CurrentScene)
                return false;

            return Input.MouseButtonDown(button);
        }
        
        public bool MouseButtonPressed(MouseButton button)
        {
            if (Name != Input.CurrentScene)
                return false;

            return Input.MouseButtonPressed(button);
        }

        public Vector2 MousePosition
        {
            get
            {
                if (Name != Input.CurrentScene)
                    return Vector2.Zero;

                return Input.MousePosition;
            }
        }
        
        public Vector2 DeltaMousePosition
        {
            get
            {
                if (Name != Input.CurrentScene)
                    return Vector2.Zero;

                return Input.DeltaMousePosition;
            }
        }
        
        public Vector2 ScrollWheelDelta
        {
            get
            {
                if (Name != Input.CurrentScene)
                    return Vector2.Zero;

                return Input.ScrollWheelDelta;
            }
        }

        internal InputScene(string name, InputSceneOptions options)
        {
            Name = name;
            Options = options;
        }
    }

    public delegate void OnKeyDown(Key key);

    public delegate void OnKeyUp(Key key);

    public delegate void OnMouseButtonDown(MouseButton button);

    public delegate void OnMouseButtonUp(MouseButton button);

    public delegate void OnScroll(Vector2 scroll);

    public delegate void OnMouseMove(Vector2 moveAmount);

    public delegate void OnTextInput(char c);
}