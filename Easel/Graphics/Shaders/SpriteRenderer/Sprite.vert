#version 450

layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec2 aTexCoords;

layout (location = 0) out vec2 frag_texCoords;

layout (binding = 0) uniform ProjView
{
    mat4 uProjView;
};

void main()
{
    gl_Position = uProjView * vec4(aPosition, 0.0, 1.0);
    frag_texCoords = aTexCoords;
}