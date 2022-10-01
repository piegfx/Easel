layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoords;
layout (location = 2) in vec3 aNormals;

layout (location = 0) out vec2 frag_texCoords;
layout (location = 1) out vec3 frag_normal;
layout (location = 2) out vec3 frag_position;

layout (binding = 0) uniform ProjViewModel
{
    mat4 uProjView;
    mat4 uModel;
};

void main()
{
    frag_texCoords = aTexCoords;
    frag_position = vec3(uModel * vec4(aPosition, 1.0));
    gl_Position = uProjView * vec4(frag_position, 1.0);
    frag_normal = mat3(transpose(inverse(uModel))) * aNormals;
}