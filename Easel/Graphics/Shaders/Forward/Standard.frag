#include "Easel.Graphics.Shaders.Material.glsl"
#include "Easel.Graphics.Shaders.Lighting.glsl"

const float PI = 3.141592653589793;

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
layout (binding = 3) uniform sampler2D uSpecular;
layout (binding = 4) uniform sampler2D uNormal;

vec3 FresnelSchlick(float cosTheta, vec3 F0)
{
    return F0 + (1.0 - F0) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
}

float DistributionGGX(vec3 N, vec3 H, float roughness)
{
    float a = roughness * roughness;
    float a2 = a * a;
    float NdotH = max(dot(N, H), 0.0);
    float NdotH2 = NdotH * NdotH;
    
    float num = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;
    
    return num / denom;
}

float GeometrySchlickGGX(float NdotV, float roughness)
{
    float r = (roughness + 1.0);
    float k = (r * r) / 8.0;
    
    float num = NdotV;
    float denom = NdotV * (1.0 - k) + k;
    
    return num / denom;
}

float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx2 = GeometrySchlickGGX(NdotV, roughness);
    float ggx1 = GeometrySchlickGGX(NdotL, roughness);
    
    return ggx1 * ggx2;
}

void main()
{
    vec3 N = normalize(in_data.normal);
    vec3 V = normalize(vec3(uCameraPos) - in_data.fragPosition);
    
    vec3 lightPos = -uSun.direction.xyz;
    
    vec3 Lo = vec3(0.0);
    
    //vec3 L = normalize(lightPos - in_data.fragPosition);
    vec3 L = normalize(lightPos);
    vec3 H = normalize(V + L);
    
    //float distance = length(lightPos - in_data.fragPosition);
    //float attenuation = 1.0 / (distance * distance);
    //vec3 radiance = uSun.color.rgb * attenuation;
    vec3 radiance = uSun.color.rgb;
    
    // 0.04 looks correct for dialetric surfaces
    vec3 F0 = vec3(0.04);
    F0 = mix(F0, uMaterial.albedo.rgb, uMaterial.metallic);
    vec3 F = FresnelSchlick(max(dot(H, V), 0.0), F0);
    
    float NDF = DistributionGGX(N, H, uMaterial.roughness);
    float G = GeometrySmith(N, V, L, uMaterial.roughness);
    
    vec3 numerator = NDF * G * F;
    float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0) + 0.0001;
    vec3 specular = numerator / denominator;
    
    vec3 kS = F;
    vec3 kD = vec3(1.0) - kS;
    kD *= 1.0 - uMaterial.metallic;
    
    float NdotL = max(dot(N, L), 0.0);
    Lo += (kD * uMaterial.albedo.rgb / PI + specular) * radiance * NdotL;
    
    vec3 ambient = vec3(0.03) * uMaterial.albedo.rgb * uMaterial.ao;
    vec3 color = ambient + Lo;
    
    color = color / (color + vec3(1.0));
    color = pow(color, vec3(1.0 / 2.2));
    
    out_color = vec4(color, 1.0);
}