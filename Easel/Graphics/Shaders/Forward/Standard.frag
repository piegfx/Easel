#version 450

layout (location = 0) in vec2 frag_texCoords;
layout (location = 1) in vec3 frag_normal;
layout (location = 2) in vec3 frag_position;

layout (location = 0) out vec4 out_color;



layout (location = 1) 

layout (binding = 2) uniform sampler2D uTexture;

void main()
{
    out_color = texture(uTexture, frag_texCoords);
}