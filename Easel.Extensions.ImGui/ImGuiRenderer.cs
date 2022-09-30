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
using SRect = System.Drawing.Rectangle;

namespace Easel.Extensions.Imgui;

public class ImGuiRenderer : IDisposable
{
    private int _windowWidth;
    private int _windowHeight;

    private bool _frameBegun;

    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _indexBuffer;
    private GraphicsBuffer _uniformBuffer;

    private uint _vboSize;
    private uint _eboSize;

    private Shader _shader;
    private DepthState _depthState;
    private RasterizerState _rasterizerState;
    private SamplerState _samplerState;
    private BlendState _blendState;

    private Texture _fontTexture;

    public Vector2 Scale;

    private readonly List<char> _pressedChars;

    private Key[] _keysList;

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
        _keysList = Enum.GetValues<Key>();

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
        _indexBuffer = device.CreateBuffer<ushort>(BufferType.IndexBuffer, _eboSize, null, true);
        _uniformBuffer = device.CreateBuffer(BufferType.UniformBuffer, Matrix4x4.Identity, true);

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
};

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

        _depthState = device.CreateDepthState(DepthStateDescription.Disabled);
        RasterizerStateDescription stateDesc = RasterizerStateDescription.CullNone;
        stateDesc.ScissorTest = true;
        _rasterizerState = device.CreateRasterizerState(stateDesc);
        _samplerState = device.CreateSamplerState(SamplerStateDescription.LinearClamp);
        _blendState = device.CreateBlendState(BlendStateDescription.NonPremultiplied);

        _stride = (uint) Unsafe.SizeOf<ImDrawVert>();
        // TODO: Byte attribute types.
        _inputLayout = device.CreateInputLayout(_stride, 
            new InputLayoutDescription("aPosition", AttributeType.Float2),
            new InputLayoutDescription("aTexCoords", AttributeType.Float2),
            new InputLayoutDescription("aColor", AttributeType.NByte4));
    }

    public void RecreateFontDeviceTexture()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height);
        GraphicsDevice device = EaselGame.Instance.Graphics.PieGraphics;
        _fontTexture?.Dispose();
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

    public void Update()
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

        io.MouseDown[0] = Input.MouseButtonDown(MouseButton.Left);
        io.MouseDown[1] = Input.MouseButtonDown(MouseButton.Right);
        io.MouseDown[2] = Input.MouseButtonDown(MouseButton.Middle);

        io.MousePos = Input.MousePosition / Scale;

        foreach (Key key in _keysList)
        {
            // This code is terrible, needs to be changed to Input.KeyDown list instead so we're not iterating through
            // *every* key.
            if ((int) key > 0)
                io.KeysDown[(int) key] = Input.KeyDown(key);
        }
        
        // Why not just have this in PressChar()???
        foreach (char c in _pressedChars)
            io.AddInputCharacter(c);
        _pressedChars.Clear();

        io.KeyCtrl = Input.KeyDown(Key.LeftControl) || Input.KeyDown(Key.RightControl);
        io.KeyAlt = Input.KeyDown(Key.LeftAlt) || Input.KeyDown(Key.RightAlt);
        io.KeyShift = Input.KeyDown(Key.LeftShift) || Input.KeyDown(Key.RightShift);
        io.KeySuper = Input.KeyDown(Key.LeftSuper) || Input.KeyDown(Key.RightSuper);
    }

    private void PressChar(char chr)
    {
        _pressedChars.Add(chr);
    }

    private static void SetKeyMappings()
    {
        ImGuiIOPtr io = ImGui.GetIO();
        io.KeyMap[(int)ImGuiKey.Tab] = (int)Key.Tab;
        io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Key.Left;
        io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Key.Right;
        io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Key.Up;
        io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Key.Down;
        io.KeyMap[(int)ImGuiKey.PageUp] = (int)Key.PageUp;
        io.KeyMap[(int)ImGuiKey.PageDown] = (int)Key.PageDown;
        io.KeyMap[(int)ImGuiKey.Home] = (int)Key.Home;
        io.KeyMap[(int)ImGuiKey.End] = (int)Key.End;
        io.KeyMap[(int)ImGuiKey.Delete] = (int)Key.Delete;
        io.KeyMap[(int)ImGuiKey.Backspace] = (int)Key.Backspace;
        io.KeyMap[(int)ImGuiKey.Enter] = (int)Key.Enter;
        io.KeyMap[(int)ImGuiKey.Escape] = (int)Key.Escape;
        io.KeyMap[(int)ImGuiKey.A] = (int)Key.A;
        io.KeyMap[(int)ImGuiKey.C] = (int)Key.C;
        io.KeyMap[(int)ImGuiKey.V] = (int)Key.V;
        io.KeyMap[(int)ImGuiKey.X] = (int)Key.X;
        io.KeyMap[(int)ImGuiKey.Y] = (int)Key.Y;
        io.KeyMap[(int)ImGuiKey.Z] = (int)Key.Z;
    }

    private unsafe void RenderImDrawData(ImDrawDataPtr drawData)
    {
        GraphicsDevice device = EaselGame.Instance.Graphics.PieGraphics;

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

        ImGuiIOPtr io = ImGui.GetIO();

        SRect scissor = device.Scissor;

        Matrix4x4 mvp = Matrix4x4.CreateOrthographicOffCenter(0.0f, io.DisplaySize.X, io.DisplaySize.Y, 0.0f, -1, 1);
        device.UpdateBuffer(_uniformBuffer, 0, mvp);
        
        drawData.ScaleClipRects(io.DisplayFramebufferScale);
        
        device.SetShader(_shader);
        device.SetRasterizerState(_rasterizerState);
        device.SetDepthState(_depthState);
        device.SetBlendState(_blendState);
        device.SetUniformBuffer(0, _uniformBuffer);

        for (int n = 0; n < drawData.CmdListsCount; n++)
        {
            ImDrawListPtr cmdList = drawData.CmdListsRange[n];
            
            ReadOnlySpan<ImDrawVert> vData =
                new ReadOnlySpan<ImDrawVert>(cmdList.VtxBuffer.Data.ToPointer(), cmdList.VtxBuffer.Size);
            device.UpdateBuffer(_vertexBuffer, 0, vData.ToArray());
            
            ReadOnlySpan<ushort> iData =
                new ReadOnlySpan<ushort>(cmdList.IdxBuffer.Data.ToPointer(), cmdList.IdxBuffer.Size);
            device.UpdateBuffer(_indexBuffer, 0, iData.ToArray());
            
            for (int cmdI = 0; cmdI < cmdList.CmdBuffer.Size; cmdI++)
            {
                ImDrawCmdPtr pcmd = cmdList.CmdBuffer[cmdI];
                if (pcmd.UserCallback != IntPtr.Zero)
                    throw new NotImplementedException();
                
                device.SetTexture(1, _fontTexture, _samplerState);
                
                Vector4 clipRect = pcmd.ClipRect;
                device.Scissor = new SRect((int) clipRect.X, _windowHeight - (int) clipRect.W, 
                    (int) (clipRect.Z - clipRect.X), (int) (clipRect.W - clipRect.Y));
                
                device.SetVertexBuffer(_vertexBuffer, _inputLayout);
                device.SetIndexBuffer(_indexBuffer, IndexType.UShort);
                device.SetPrimitiveType(PrimitiveType.TriangleList);
                device.DrawIndexed(pcmd.ElemCount, (int) pcmd.IdxOffset * sizeof(ushort), (int) pcmd.VtxOffset);
            }
        }

        device.Scissor = device.Viewport;
    }

    public void Dispose()
    {
        _vertexBuffer.Dispose();
        _indexBuffer.Dispose();
        _uniformBuffer.Dispose();
        _shader.Dispose();
        _depthState.Dispose();
        _rasterizerState.Dispose();
        _samplerState.Dispose();
        _fontTexture.Dispose();
        _inputLayout.Dispose();
    }
}