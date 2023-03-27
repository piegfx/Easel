#ifndef TYPES_HLSL
#define TYPES_HLSL

struct Material
{
    float4 tiling;

    float4 albedo;
    float1 metallic;
    float1 roughness;
    float1 ao;
};

struct DirectionalLight
{
    float4 direction;
    float4 color;
};

#endif