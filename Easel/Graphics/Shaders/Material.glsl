#ifndef MATERIAL_GLSL
#define MATERIAL_GLSL

struct Material
{
    vec4 albedo;
    vec4 tiling;
    float metallic;
    float roughness;
    float ao;
    
    float _padding;
};

#endif