using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Easel.Graphics;
using Easel.Math;
using ImGuiNET;
using Pie;
using Pie.ShaderCompiler;
using Pie.Windowing;
using Texture = Pie.Texture;

namespace Easel.Extensions.Imgui;

public class ImGuiRenderer : IDisposable
{
    private int _windowWidth;
    private int _windowHeight;

    private bool _frameBegun;

    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _indexBuffer;

    private uint _vboSize;
    private uint _eboSize;

    private Shader _shader;

    private Texture _fontTexture;

    public Vector2 Scale;

    private readonly List<char> _pressedChars;

    private Keys[] _keysList;

    private uint _stride;
    private InputLayout _inputLayout;

    private IntPtr _context;

    public ImGuiRenderer()
    {
        Scale = Vector2.One;

        EaselGraphics graphics = EaselGame.Instance.Graphics;
        _windowWidth = graphics.Viewport.Width;
        _windowHeight = graphics.Viewport.Height;

        graphics.ViewportResized += ViewportOnResize;
        // TODO: Input.TextInput
        EaselGame.Instance.Window.TextInput += PressChar;

        _pressedChars = new List<char>();
        _keysList = Enum.GetValues<Keys>();

        _context = ImGui.CreateContext();
        ImGui.SetCurrentContext(_context);
        ImGuiIOPtr io = ImGui.GetIO();
        io.Fonts.AddFontDefault();
        io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

        CreateDeviceResources();
        SetKeyMappings();

        SetPerFrameImGuiData(1f / 60f);
        
        ImGui.NewFrame();
        _frameBegun = true;
    }

    private void ViewportOnResize(Rectangle viewport)
    {
        _windowWidth = viewport.Width;
        _windowHeight = viewport.Height;
    }

    private void CreateDeviceResources()
    {
        _vboSize = 10000;
        _eboSize = 2000;

        GraphicsDevice device = EaselGame.Instance.Graphics.PieGraphics;
        _vertexBuffer = device.CreateBuffer<ImDrawVert>(BufferType.VertexBuffer, _vboSize, null, true);
        _indexBuffer = device.CreateBuffer<uint>(BufferType.IndexBuffer, _eboSize, null, true);

        RecreateFontDeviceTexture();
        
        const string vertexSource = @"
#version 450
layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec2 aTexCoords;
layout (location = 2) in vec4 aColor;

layout (location = 0) out vec4 frag_color;
layout (location = 1) out vec2 frag_texCoords;

layout (binding = 0) uniform ProjMatrix
{
    mat4 uProjection;
}

void main()
{
    gl_Position = uProjection * vec4(aPosition, 0, 1);
    frag_color = aColor;
    frag_texCoords = aTexCoords;
}";

        const string fragmentSource = @"
#version 450
layout (location = 0) in vec4 frag_color;
layout (location = 1) in vec2 frag_texCoords;

layout (location = 0) out vec4 out_color;

layout (binding = 1) uniform sampler2D uTexture;

void main()
{
    out_color = frag_color * texture(uTexture, frag_texCoords);
}";

        _shader = device.CreateCrossPlatformShader(new ShaderAttachment(ShaderStage.Vertex, vertexSource),
            new ShaderAttachment(ShaderStage.Fragment, fragmentSource));

        _stride = (uint) Unsafe.SizeOf<ImDrawVert>();
        // TODO: Byte attribute types.
        _inputLayout = device.CreateInputLayout(_stride, new InputLayoutDescription("aPosition", AttributeType.Float2),
            new InputLayoutDescription("aTexCoords", AttributeType.Float2),
            new InputLayoutDescription("aColor", AttributeType.Byte4));
    }

    private void RecreateFontDeviceTexture()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height);
        GraphicsDevice device = EaselGame.Instance.Graphics.PieGraphics;
        _fontTexture =
            device.CreateTexture(
                new TextureDescription(TextureType.Texture2D, width, height, PixelFormat.R8G8B8A8_UNorm, true, 1,
                    TextureUsage.ShaderResource), pixels);
        
        // TODO: Temp fix for fonts
        io.Fonts.SetTexID((IntPtr) 1);
    }

    public void Draw()
    {
        if (_frameBegun)
        {
            _frameBegun = false;
            ImGui.Render();
            RenderImDrawData(ImGui.GetDrawData());
        }
    }

    private void Update()
    {
        if (_frameBegun)
            ImGui.Render();

        SetPerFrameImGuiData(Time.DeltaTime);
        UpdateImGuiInput();

        _frameBegun = true;
        ImGui.NewFrame();
    }

    private void SetPerFrameImGuiData(float deltaTime)
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.DisplaySize = new Vector2(_windowWidth / Scale.X, _windowHeight / Scale.Y);
        io.DisplayFramebufferScale = Scale;
        io.DeltaTime = deltaTime;
    }

    private void UpdateImGuiInput()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        
        //io.MouseDown[0] = Input

        io.MousePos = Input.MousePosition / Scale;

        foreach (Keys key in _keysList)
        {
            // This code is terrible, needs to be changed to Input.KeysDown list instead so we're not iterating through
            // *every* key.
            if ((int) key > 0)
                io.KeysDown[(int) key] = Input.KeyDown(key);
        }
        
        // Why not just have this in PressChar()???
        foreach (char c in _pressedChars)
            io.AddInputCharacter(c);
        _pressedChars.Clear();

        io.KeyCtrl = Input.KeyDown(Keys.LeftControl) || Input.KeyDown(Keys.RightControl);
        io.KeyAlt = Input.KeyDown(Keys.LeftAlt) || Input.KeyDown(Keys.RightAlt);
        io.KeyShift = Input.KeyDown(Keys.LeftShift) || Input.KeyDown(Keys.RightShift);
        io.KeySuper = Input.KeyDown(Keys.LeftSuper) || Input.KeyDown(Keys.RightSuper);
    }

    private void PressChar(char chr)
    {
        _pressedChars.Add(chr);
    }

    private static void SetKeyMappings()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.KeyMap[(int)ImGuiKey.Tab] = (int)Keys.Tab;
        io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Keys.Left;
        io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Keys.Right;
        io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Keys.Up;
        io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Keys.Down;
        io.KeyMap[(int)ImGuiKey.PageUp] = (int)Keys.PageUp;
        io.KeyMap[(int)ImGuiKey.PageDown] = (int)Keys.PageDown;
        io.KeyMap[(int)ImGuiKey.Home] = (int)Keys.Home;
        io.KeyMap[(int)ImGuiKey.End] = (int)Keys.End;
        io.KeyMap[(int)ImGuiKey.Delete] = (int)Keys.Delete;
        io.KeyMap[(int)ImGuiKey.Backspace] = (int)Keys.Backspace;
        io.KeyMap[(int)ImGuiKey.Enter] = (int)Keys.Enter;
        io.KeyMap[(int)ImGuiKey.Escape] = (int)Keys.Escape;
        io.KeyMap[(int)ImGuiKey.A] = (int)Keys.A;
        io.KeyMap[(int)ImGuiKey.C] = (int)Keys.C;
        io.KeyMap[(int)ImGuiKey.V] = (int)Keys.V;
        io.KeyMap[(int)ImGuiKey.X] = (int)Keys.X;
        io.KeyMap[(int)ImGuiKey.Y] = (int)Keys.Y;
        io.KeyMap[(int)ImGuiKey.Z] = (int)Keys.Z;
    }

    private unsafe void RenderImDrawData(ImDrawDataPtr drawData)
    {
        GraphicsDevice device = EaselGame.Instance.Graphics.PieGraphics;
        
        uint vertexOffsetInVertices = 0;
        uint indexOffsetInElements = 0;

        if (drawData.CmdListsCount == 0)
            return;

        uint totalVbSize = (uint) (drawData.TotalVtxCount * Unsafe.SizeOf<ImDrawVert>());
        if (totalVbSize > _vboSize)
        {
            _vboSize = (uint) MathF.Max(_vboSize * 1.5f, totalVbSize);
            _vertexBuffer.Dispose();
            _vertexBuffer = device.CreateBuffer<ImDrawVert>(BufferType.VertexBuffer, _vboSize, null, true);
        }
        
        uint totalIbSize = (uint) (drawData.TotalIdxCount * sizeof(ushort));
        if (totalIbSize > _eboSize)
        {
            _eboSize = (uint) MathF.Max(_eboSize * 1.5f, totalIbSize);
            _indexBuffer.Dispose();
            _indexBuffer = device.CreateBuffer<ushort>(BufferType.IndexBuffer, _eboSize, null, true);
        }

        for (int i = 0; i < drawData.CmdListsCount; i++)
        {
            ImDrawListPtr cmdList = drawData.CmdListsRange[i];
            
            // TODO: Device.UpdateBuffer() with intptr
            ReadOnlySpan<ImDrawVert> vData =
                new ReadOnlySpan<ImDrawVert>(cmdList.VtxBuffer.Data.ToPointer(), cmdList.VtxBuffer.Size);
            device.UpdateBuffer(_vertexBuffer, vertexOffsetInVertices, vData.ToArray());

            ReadOnlySpan<ushort> iData =
                new ReadOnlySpan<ushort>(cmdList.IdxBuffer.Data.ToPointer(), cmdList.IdxBuffer.Size);
            device.UpdateBuffer(_indexBuffer, indexOffsetInElements, iData.ToArray());

            vertexOffsetInVertices += (uint) cmdList.VtxBuffer.Size;
            indexOffsetInElements += (uint) cmdList.IdxBuffer.Size;
        }
        
        
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}