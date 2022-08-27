using System;
using System.Drawing;
using System.Numerics;
using Easel.Graphics;
using Pie;
using Pie.ShaderCompiler;
using Pie.Utils;

namespace Easel.Renderers;

/// <summary>
/// Efficiently batches and renders 2D sprites.
/// </summary>
public static class SpriteRenderer
{
    // Basic sprite renderer
    // TODO: Make a proper sprite batcher once pie is ready
    
    private static VertexPositionTexture[] _vertices = new[]
    {
        new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 1)),
        new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(1, 0)),
        new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 0)),
        new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(0, 1))
    };

    private static uint[] _indices = new[]
    {
        0u, 1u, 3u,
        1u, 2u, 3u
    };

    private static GraphicsBuffer _vertexBuffer;
    private static GraphicsBuffer _indexBuffer;

    private static GraphicsBuffer _projViewModelBuffer;
    private static ProjViewModel _projViewModel;
    private static Matrix4x4 _projection;

    private static Shader _shader;
    private static InputLayout _inputLayout;
    private static RasterizerState _rasterizerState;

    private static bool _begin;

    /// <summary>
    /// TEMPORARY: The vertex shader of the sprite renderer. This will be moved to an embedded resource.
    /// </summary>
    public const string SpriteVertex = @"
#version 450

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoords;

layout (location = 0) out vec2 frag_texCoords;

layout (binding = 0) uniform ProjViewModel
{
    mat4 uProjView;
    mat4 uModel;
};

void main()
{
    gl_Position = uProjView * uModel * vec4(aPosition, 1.0);
    frag_texCoords = aTexCoords;
}";

    /// <summary>
    /// TEMPORARY: The fragment shader of the sprite renderer. This will be moved to an embedded resource.
    /// </summary>
    public const string SpriteFragment = @"
#version 450

layout (location = 0) in vec2 frag_texCoords;

layout (location = 0) out vec4 out_color;

layout (binding = 1) uniform sampler2D uTexture;

void main()
{
    out_color = texture(uTexture, frag_texCoords);
}";

    static SpriteRenderer()
    {
        GraphicsDevice device = EaselGame.Instance.Graphics.PieGraphics;

        _vertexBuffer = device.CreateBuffer(BufferType.VertexBuffer, _vertices);
        _indexBuffer = device.CreateBuffer(BufferType.IndexBuffer, _indices);
        
        CreateOrthoMatrix(EaselGame.Instance.Window.Size);
        _projViewModel = new ProjViewModel()
        {
            ProjView = _projection,
            Model = Matrix4x4.Identity
        };
        _projViewModelBuffer = device.CreateBuffer(BufferType.UniformBuffer, _projViewModel, true);

        _shader = device.CreateCrossPlatformShader(new ShaderAttachment(ShaderStage.Vertex, SpriteVertex),
            new ShaderAttachment(ShaderStage.Fragment, SpriteFragment));

        _inputLayout = device.CreateInputLayout(new InputLayoutDescription("aPosition", AttributeType.Vec3),
            new InputLayoutDescription("aTexCoords", AttributeType.Vec2));

        _rasterizerState = device.CreateRasterizerState(RasterizerStateDescription.CullCounterClockwise);

        EaselGame.Instance.Window.Resize += CreateOrthoMatrix;
    }

    private static void CreateOrthoMatrix(Size size)
    {
        _projection = Matrix4x4.CreateOrthographicOffCenter(0, size.Width, size.Height, 0, -1, 1);
    }

    /// <summary>
    /// Begin a new SpriteRenderer session for drawing.
    /// </summary>
    /// <param name="transform">The transform matrix to apply, if any.</param>
    /// <exception cref="EaselException">Thrown if a session is already active.</exception>
    public static void Begin(Matrix4x4? transform = null)
    {
        if (_begin)
        {
            throw new EaselException(
                "SpriteRenderer session has already begun. You must call End() before you can call Begin() again.");
        }

        _begin = true;

        _projViewModel.ProjView = _projection * (transform ?? Matrix4x4.Identity);
    }

    /// <summary>
    /// Draw a texture at the given position.
    /// </summary>
    /// <param name="texture">The <see cref="TextureObject"/> to draw.</param>
    /// <param name="position">The position, in pixels, to draw at. (0, 0) = top left</param>
    /// <exception cref="EaselException">Thrown if no session is active.</exception>
    public static void Draw(TextureObject texture, Vector2 position)
    {
        if (!_begin)
        {
            throw new EaselException(
                "No SpriteRenderer session is active. You must call Begin() before you can call Draw().");
        }

        GraphicsDevice device = EaselGame.Instance.Graphics.PieGraphics;
        
        _projViewModel.Model = Matrix4x4.CreateScale(texture.Size.Width, texture.Size.Height, 1) *
                               Matrix4x4.CreateTranslation(new Vector3(position, 0));
        _projViewModelBuffer.Update(0, _projViewModel);
        
        device.SetShader(_shader);
        device.SetUniformBuffer(0, _projViewModelBuffer);
        device.SetTexture(1, texture.PieTexture);
        device.SetRasterizerState(_rasterizerState);
        device.SetVertexBuffer(_vertexBuffer, _inputLayout);
        device.SetIndexBuffer(_indexBuffer);
        device.Draw((uint) _indices.Length);
    }

    /// <summary>
    /// End a session and flush remaining draw lists to the GPU.
    /// </summary>
    /// <exception cref="EaselException">Thrown if there is no active session.</exception>
    public static void End()
    {
        if (!_begin)
        {
            throw new EaselException(
                "There is no current SpriteRenderer session to end. You must call Begin() before you can call End() again.");
        }

        _begin = false;
    }

    private struct ProjViewModel
    {
        public Matrix4x4 ProjView;
        public Matrix4x4 Model;
    }
}