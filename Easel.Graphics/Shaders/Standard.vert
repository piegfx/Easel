// The bog standard vertex shader all render pipelines use.
#version 450

#include "Types.glsl"
        
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoords;
layout (location = 2) in vec3 aNormals;
layout (location = 3) in vec3 aTangents;

layout (location = 0) out VertexInfo
{
    vec2 texCoords;
    vec3 normal;
    vec3 fragPosition;
    vec4 lightSpace;
} out_data;

layout (binding = 0) uniform ProjViewModel
{
    mat4 uProjection;
    mat4 uView;
    mat4 uModel;
    mat4 uLightSpace;
};

layout (binding = 1) uniform SceneInfo
{
    vec4 uCameraPos;
    Material uMaterial;
    DirectionalLight uSun;
};

void main()
{
    gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
    out_data.texCoords = aTexCoords * uMaterial.tiling.xy;
    out_data.normal = mat3(uModel) * aNormals;
    vec3 fragPosition = vec3(uModel * vec4(aPosition, 1.0));
    out_data.fragPosition = fragPosition;
    out_data.lightSpace = uLightSpace * vec4(fragPosition, 1.0);
}