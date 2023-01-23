#ifndef TYPES_GLSL
#define TYPES_GLSL

struct Material
{
    vec4 albedo;
    vec4 tiling;
    float metallic;
    float roughness;
    float ao;
    
    float _padding;
};

struct DirectionalLight
{
    vec4 direction;
    vec4 color;
};

#endif