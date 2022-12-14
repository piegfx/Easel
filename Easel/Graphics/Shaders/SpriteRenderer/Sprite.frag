layout (location = 0) in vec2 frag_texCoords;
layout (location = 1) in vec4 frag_tint;
layout (location = 2) in vec4 frag_meta1;
layout (location = 3) in vec4 frag_meta2;

layout (location = 0) out vec4 out_color;

layout (binding = 1) uniform sampler2D uTexture;

void main()
{
    out_color = texture(uTexture, frag_texCoords) * frag_tint;
}