#include "Easel.Graphics.Shaders.Forward.Lighting.glsl"
#include "Easel.Graphics.Shaders.Forward.Material.glsl"

layout (location = 0) in vec2 frag_texCoords;
layout (location = 1) in vec3 frag_normal;
layout (location = 2) in vec3 frag_position;

layout (location = 0) out vec4 out_color;

layout (binding = 1) uniform CameraInfo
{
    Material uMaterial;
    DirectionalLight uSun;
    vec4 uCameraPos;
};

layout (binding = 2) uniform sampler2D uAlbedo;
layout (binding = 3) uniform sampler2D uSpecular;

void main()
{
    #ifdef LIGHTING
    vec3 norm = normalize(frag_normal);
    vec3 viewDir = normalize(uCameraPos.xyz - frag_position);
    
    vec4 result = CalculateDirectional(uSun, uMaterial, uAlbedo, uSpecular, frag_texCoords * uMaterial.tiling.xy, norm, viewDir);
    
    #else
    vec4 result = texture(uAlbedo, frag_texCoords);
    #endif
    
    #ifdef ALPHA
    if (result.a <= uMaterial.alphaCutoff.x)
        discard;
    #endif
    
    out_color = result * uMaterial.color;
}