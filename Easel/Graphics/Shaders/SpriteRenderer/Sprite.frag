#version 450

layout (location = 0) in vec2 frag_texCoords;

layout (location = 0) out vec4 out_color;

layout (binding = 1) uniform sampler2D uTexture;

void main()
{
    out_color = texture(uTexture, frag_texCoords);
}