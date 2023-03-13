#version 450

layout (location = 0) in vec3 aPosition;

layout (binding = 0) uniform ProjViewModel
{
    mat4 uProjection;
    mat4 uView;
    mat4 uModel;
    mat4 uLightSpace;
};

void main()
{
    gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
}