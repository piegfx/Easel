#version 450

#include "../Types.glsl"
#include "../Lighting.glsl"

layout (location = 0) in VertexInfo
{
    vec2 texCoords;
    vec3 normal;
    vec3 fragPosition;
    vec4 lightSpace;
} in_data;

layout (location = 0) out vec4 out_color;

layout (binding = 1) uniform SceneInfo
{
    vec4 uCameraPos;
    Material uMaterial;
    DirectionalLight uSun;
};

layout (binding = 2) uniform sampler2D uShadow;
layout (binding = 3) uniform sampler2D uAlbedo;
layout (binding = 4) uniform sampler2D uNormal;
#ifdef COMBINE_TEXTURES
// Combined metallic, roughness, and ambient occlusion textures.
layout (binding = 5) uniform sampler2D uMraTex;
#else
layout (binding = 5) uniform sampler2D uMetallic;
layout (binding = 6) uniform sampler2D uRoughness;
layout (binding = 7) uniform sampler2D uAo;
#endif

void main()
{
    // convert albedo texture to linear space
    vec4 albedo = pow(texture(uAlbedo, in_data.texCoords), vec4(2.2)) * uMaterial.albedo;
    vec3 normal = TempNormal(uNormal, in_data.fragPosition, in_data.texCoords, in_data.normal);
    #ifdef COMBINE_TEXTURES
    float metallic = texture(uMraTex, in_data.texCoords).r * uMaterial.metallic;
    float roughness = texture(uMraTex, in_data.texCoords).g * uMaterial.roughness;
    float ao = texture(uMraTex, in_data.texCoords).b * uMaterial.ao;
    #else
    float metallic = texture(uMetallic, in_data.texCoords).r * uMaterial.metallic;
    float roughness = texture(uRoughness, in_data.texCoords).r * uMaterial.roughness;
    //float ao = texture(uAo, in_data.texCoords).r * uMaterial.ao;
    float ao = 1.0;
    
    #endif
    
    vec3 viewDir = normalize(vec3(uCameraPos) - in_data.fragPosition);
    vec3 result = ProcessDirLight(uSun, viewDir, albedo.rgb, normal, metallic, roughness);
    
    //float shadow = ProcessShadow(in_data.lightSpace, uShadow);
    float shadow = 0.0;
    
    vec3 ambient = vec3(0.03) * albedo.rgb * ao;
    vec3 color = ambient + (1.0 - shadow) * result;
    
    // HDR and gamma correction
    color = color / (color + vec3(1.0));
    color = pow(color, vec3(1.0 / 2.2));
    
    #ifdef ALPHA
    out_color = vec4(color, albedo.a);
    #else
    out_color = vec4(color, 1.0);
    #endif
}