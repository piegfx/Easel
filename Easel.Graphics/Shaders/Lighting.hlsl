#ifndef LIGHTING_HLSL
#define LIGHTING_HLSL

#include "Types.hlsl"

const float PI = 3.141592653589793;

// TODO: Replace this with proper normal mapping (still)
float3 TempNormal(Texture2D normalTex, SamplerState state, float3 fragPos, float2 texCoords, float3 normal)
{
    float3 tangentNormal = normalTex.Sample(state, texCoords).rgb * 2.0 - 1.0;

    float3 Q1 = ddx(fragPos);
    float3 Q2 = ddy(fragPos);
    float2 st1 = ddx(texCoords);
    float2 st2 = ddy(texCoords);

    float3 N = normalize(normal);
    float3 T = normalize(Q1 * st2.y - Q2 * st1.y);
    float3 B = -normalize(cross(N, T));
    float3x3 tbn = float3x3(T, B, N);

    return normalize(mul(tangentNormal, tbn));
}

float3 FresnelSchlick(float cosTheta, float3 F0)
{
    return F0 + (1.0 - F0) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
}

float DistributionGGX(float3 N, float3 H, float roughness)
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
    float r = roughness + 1.0;
    float k = (r * r) / 8.0;

    float num = NdotV;
    float denom = NdotV * (1.0 - k) + k;

    return num / denom;
}

float GeometrySmith(float3 N, float3 V, float3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NDotL = max(dot(N, L), 0.0);
    float ggx2 = GeometrySchlickGGX(NdotV, roughness);
    float ggx1 = GeometrySchlickGGX(NDotL, roughness);

    return ggx1 * ggx2;
}

float3 ProcessLight(float3 albedo, float3 normal, float metallic, float roughness, float3 L, float3 N, float3 V, float3 radiance)
{
    float3 H = normalize(V + L);

    // 0.04 looks correct for dialectic surfaces.
    float3 F0 = (float4) 0.04;
    F0 = lerp(F0, albedo, metallic);
    float3 F = FresnelSchlick(max(dot(H, V), 0.0), F0);

    float NDF = DistributionGGX(N, H, roughness);
    float G = GeometrySmith(N, V, L, roughness);

    float3 numerator = NDF * G * F;
    float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0) + 0.0001;
    float3 specular = numerator / denominator;

    float3 kS = F;
    float3 kD = (float3) 1.0 - kS;
    kD *= 1.0 - metallic;

    float NdotL = max(dot(N, L), 0.0);
    return (kD * albedo.rgb / PI + specular) * radiance * NdotL;
}

float3 ProcessDirLight(DirectionalLight light, float3 viewDir, float3 albedo, float3 normal, float metallic, float roughness)
{
    float3 N = normalize(normal);

    float3 L = normalize(-light.direction.xyz);
    float3 radiance = light.color.rgb;

    return ProcessLight(albedo, normal, metallic, roughness, L, N, viewDir, radiance);
}

#endif