#ifndef LIGHTING_GLSL
#define LIGHTING_GLSL

struct DirectionalLight
{
    vec4 diffuseColor;
    vec4 specularColor;
    vec3 direction;
};

vec4 CalculateDirectional(DirectionalLight light, vec3 normal, vec3 viewDir, vec2 texCoords, float shininess, sampler2D diffuse, sampler2D specular)
{
    vec3 lightDir = normalize(-light.direction);
    
    float diff = max(dot(normal, lightDir), 0.0);
    
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), shininess);
    
    vec4 diffuseTex = texture(diffuse, texCoords);
    
    // TODO: Scene ambient color
    vec3 ambientCol = vec3(0.2, 0.2, 0.2) * diffuseTex.rgb;
    vec3 diffuseCol = light.diffuseColor.rgb * diff * diffuseTex.rgb;
    vec3 specularCol = light.specularColor.rgb * spec * texture(specular, texCoords).rgb;
    return vec4(ambientCol + diffuseCol + specularCol, diffuseTex.a);
}

#endif