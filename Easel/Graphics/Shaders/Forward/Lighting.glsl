#ifndef LIGHTING
#define LIGHTING

#include "Easel.Graphics.Shaders.Forward.Lighting.glsl"
#include "Easel.Graphics.Shaders.Forward.Material.glsl"

struct DirectionalLight
{
    vec4 direction;
    vec4 ambient;
    vec4 diffuse;
    vec4 specular;
};

vec4 CalculateDirectional(DirectionalLight light, Material material, sampler2D albedo, sampler2D specTex, vec2 texCoord, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction.xyz);
    
    float diff = max(dot(normal, lightDir), 0.0);
    
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    
    vec4 tex = texture(albedo, texCoord);
    
    vec3 ambient = vec3(light.ambient * tex);
    vec3 diffuse = vec3(light.diffuse * diff * tex);
    vec3 specular = vec3(light.specular * spec * texture(specTex, texCoord));
    return vec4(ambient + diffuse + specular, tex.a);
}
#endif