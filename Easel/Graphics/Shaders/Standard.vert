// The bog standard vertex shader all render pipelines use.

#include "Easel.Graphics.Shaders.Material.glsl"
        
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoords;
layout (location = 2) in vec3 aNormals;

layout (location = 0) out VertexInfo
{
    vec2 texCoords;
} out_data;

layout (binding = 0) uniform ProjViewModel
{
    mat4 uProjection;
    mat4 uView;
    mat4 uModel;
};

layout (binding = 1) uniform MatInfo
{
    Material uMaterial;
};

void main()
{
    gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
    out_data.texCoords = aTexCoords * uMaterial.tiling;
}