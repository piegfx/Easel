#include "Easel.Graphics.Shaders.Forward.Lighting.glsl"
#include "Easel.Graphics.Shaders.Forward.Material.glsl"

layout (location = 0) in vec2 frag_texCoords;
layout (location = 1) in vec3 frag_normal;
layout (location = 2) in vec3 frag_position;
layout (location = 3) in vec3 frag_tangentLightPos;
layout (location = 4) in vec3 frag_tangentViewPos;
layout (location = 5) in vec3 frag_tangentFragPos;

layout (location = 0) out vec4 out_color;

layout (binding = 1) uniform CameraInfo
{
    Material uMaterial;
    DirectionalLight uSun;
    vec4 uCameraPos;
};

layout (binding = 2) uniform sampler2D uAlbedo;
layout (binding = 3) uniform sampler2D uSpecular;
layout (binding = 4) uniform sampler2D uNormal;

void main()
{
#ifdef LIGHTING
    //
    #ifdef NORMAL_MAPS
        vec3 norm = texture(uNormal, frag_texCoords).rgb;
        norm = normalize(norm * 2.0 - 1.0);
        
        vec3 viewDir = normalize(frag_tangentViewPos - frag_tangentFragPos);
        vec3 lightDir = normalize(frag_tangentLightPos);
    #else
        vec3 norm = normalize(frag_normal);
        vec3 viewDir = normalize(uCameraPos.xyz - frag_position);
        vec3 lightDir = normalize(-uSun.direction.xyz);
    #endif
    
    vec4 result = CalculateDirectional(uSun, uMaterial, uAlbedo, uSpecular, frag_texCoords, norm, viewDir, lightDir);
    
#else
    vec4 result = texture(uAlbedo, frag_texCoords);
#endif
    
#ifdef ALPHA
    if (result.a <= uMaterial.alphaCutoff.x)
        discard;
#endif
    
    out_color = result * uMaterial.color;
}