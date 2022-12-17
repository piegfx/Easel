layout (location = 0) in vec2 frag_texCoords;
layout (location = 1) in vec3 frag_normal;
layout (location = 2) in vec3 frag_position;
layout (location = 3) in vec3 frag_tangentLightPos;
layout (location = 4) in vec3 frag_tangentViewPos;
layout (location = 5) in vec3 frag_tangentFragPos;

layout (location = 0) out vec3 out_gPosition;
layout (location = 1) out vec3 out_gNormal;
layout (location = 2) out vec4 out_gAlbedoSpec;

layout (binding = 2) uniform sampler2D uDiffuse;
layout (binding = 3) uniform sampler2D uSpecular;

void main() 
{
    out_gPosition = frag_position;
    out_gNormal = normalize(frag_normal);
    out_gAlbedoSpec.rgb = texture(uDiffuse, frag_texCoords).rgb;
    out_gAlbedoSpec.a = texture(uSpecular, frag_texCoords).r;
}