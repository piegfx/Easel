using System.Collections.Generic;
using Pie;
using Pie.ShaderCompiler;

namespace Easel.Renderers;

public static class ForwardRenderer
{
    private const string TempVertex = @"
#version 450

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoords;
layout (location = 2) in vec3 aNormals;

layout (location = 0) out vec2 frag_texCoords;
layout (location = 1) out vec3 frag_normals;

layout (binding = 0) uniform ProjViewModel
{
    mat4 uProjView;
    mat4 uModel;
};

void main()
{
    gl_Position = uProjView * uModel * vec4(aPosition, 1.0);
    frag_texCoords = aTexCoords;
    frag_normals = aNormals;
}";

    private const string TempFragment = @"
#version 450

layout (location = 0) in vec2 frag_texCoords;
layout (location = 1) in vec3 frag_normals;

layout (location = 0) out out_color;

layout (binding = 1) uniform sampler2D uTexture;

void main()
{
    out_color = texture(uTexture, frag_texCoords);
}";

    private static Shader _tempShader;
    private static InputLayout _tempInputLayout;

    private static List<Renderable> _translucents;

    private static List<Renderable> _opaques;

    static ForwardRenderer()
    {
        _translucents = new List<Renderable>();
        _opaques = new List<Renderable>();
        
        GraphicsDevice device = EaselGame.Instance.Graphics;
        _tempShader = device.CreateCrossPlatformShader(new ShaderAttachment(ShaderStage.Vertex, TempVertex),
            new ShaderAttachment(ShaderStage.Fragment, TempFragment));
        _tempInputLayout = device.CreateInputLayout(new InputLayoutDescription("aPosition", AttributeType.Vec3),
            new InputLayoutDescription("aTexCoords", AttributeType.Vec2),
            new InputLayoutDescription("aNormals", AttributeType.Vec3));
    }

    public static void DrawTranslucent(Renderable renderable)
    {
        _translucents.Add(renderable);
    }

    public static void DrawOpaque(Renderable renderable)
    {
        _opaques.Add(renderable);
    }

    public static void ClearAll()
    {
        _translucents.Clear();
        _opaques.Clear();
    }

    public static void Render()
    {
        GraphicsDevice device = EaselGame.Instance.Graphics;
        
        
    }
}