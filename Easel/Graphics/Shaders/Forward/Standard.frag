#include "Easel.Graphics.Shaders.Types.glsl"
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

layout (binding = 2) uniform sampler2D uAlbedo;
layout (binding = 3) uniform sampler2D uNormal;
#ifdef COMBINE_TEXTURES
// Combined metallic, roughness, and ambient occlusion textures.
layout (binding = 4) uniform sampler2D uMraTex;
#else
layout (binding = 4) uniform sampler2D uMetallic;
layout (binding = 5) uniform sampler2D uRoughness;
layout (binding = 6) uniform sampler2D uAo;
#endif

void main()
{
    // convert albedo texture to linear space
    vec3 albedo = pow(texture(uAlbedo, in_data.texCoords).rgb, vec3(2.2));
    vec3 normal = TempNormal(uNormal, in_data.fragPosition, in_data.texCoords, in_data.normal);
    #if COMBINE_TEXTURES
    float metallic = texture(uMraTex, in_data.texCoords).r;
    float roughness = texture(uMraTex, in_data.texCoords).g;
    float ao = texture(uMraTex, in_data.texCoords).b;
    #else
    float metallic = texture(uMetallic, in_data.texCoords).r;
    float roughness = texture(uRoughness, in_data.texCoords).r;
    float ao = texture(uAo, in_data.texCoords).r;
    #endif
    
    vec3 viewDir = normalize(vec3(uCameraPos) - in_data.fragPosition);
    vec3 result = ProcessDirLight(uSun, viewDir, albedo, normal, metallic, roughness);
    
    vec3 ambient = vec3(0.03) * albedo.rgb * ao;
    vec3 color = ambient + result;
    
    // HDR and gamma correction
    color = color / (color + vec3(1.0));
    color = pow(color, vec3(1.0 / 2.2));
    
    out_color = vec4(color, 1.0);
}