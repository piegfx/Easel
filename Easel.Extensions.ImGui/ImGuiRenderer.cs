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
        //EaselGame.Instance.Window.TextInput += PressChar;

        _pressedChars = new List<char>();
        _keysList = Enum.GetValues<Keys>();

        _context = ImGui.CreateContext();
        ImGui.SetCurrentContext(_context);
        ImGuiIOPtr io = ImGui.GetIO();
        io.Fonts.AddFontDefault();
        io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

        CreateDeviceResources();
        //SetKeyMappings();

        //SetPerFrameImGuiData(1f / 60f);
        
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
        _inputLayout = device.CreateInputLayout(_stride, new InputLayoutDescription("aPosition", AttributeType.Vec2),
            new InputLayoutDescription("aTexCoords", AttributeType.Vec2),
            new InputLayoutDescription("aColor", AttributeType.Vec4));
    }

    private void RecreateFontDeviceTexture()
    {
        ImGuiIOPtr io = ImGui.GetIO();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}