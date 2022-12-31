layout (location = 0) in VertexInfo
{
    vec2 texCoords;
} in_data;

layout (location = 0) out vec4 out_color;

layout (binding = 1) uniform sampler2D uTexture;

void main()
{
    out_color = texture(uTexture, in_data.texCoords);
}