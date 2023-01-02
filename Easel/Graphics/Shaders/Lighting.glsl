#ifndef LIGHTING_GLSL
#define LIGHTING_GLSL

struct DirectionalLight
{
    vec4 diffuseColor;
    vec4 specularColor;
    vec3 direction;
};

#endif