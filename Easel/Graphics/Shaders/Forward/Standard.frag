#include "Easel.Graphics.Shaders.Material.glsl"
#include "Easel.Graphics.Shaders.Lighting.glsl"

layout (location = 0) in VertexInfo
{
    vec2 texCoords;
    vec3 normal;
    vec3 fragPosition;
} in_data;

layout (location = 0) out vec4 out_color;

layout (binding = 1) uniform SceneInfo
{
    vec4 uCameraPos;
    Material uMaterial;
    DirectionalLight uSun;
};

layout (binding = 2) uniform sampler2D uDiffuse;
// These textures are only defined in the standard shader.
#ifdef LIGHTING
layout (binding = 3) uniform sampler2D uSpecular;
layout (binding = 4) uniform sampler2D uNormal;
#endif

void main()
{
    #ifdef LIGHTING
    vec3 normal = normalize(in_data.normal);
    vec3 lightDir = normalize(vec3(uCameraPos) - in_data.fragPosition);
    
    out_color = CalculateDirectional(uSun, normal, lightDir, in_data.texCoords, uMaterial.shininess, uDiffuse, uSpecular);
    #else
    out_color = texture(uDiffuse, in_data.texCoords) * uMaterial.color;
    #endif
    
    // Standard materials do not support transparency whatsoever. Therefore, we must remove it.
    #ifndef TRANSPARENCY
    out_color = vec4(out_color.xyz, 1.0);
    #endif
}