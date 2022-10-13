#include "Easel.Graphics.Shaders.Forward.Lighting.glsl"
#include "Easel.Graphics.Shaders.Forward.Material.glsl"

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoords;
layout (location = 2) in vec3 aNormals;
layout (location = 3) in vec3 aTangent;

layout (location = 0) out vec2 frag_texCoords;
layout (location = 1) out vec3 frag_normal;
//layout (location = 1) out mat3 frag_tbn;
layout (location = 2) out vec3 frag_position;
layout (location = 3) out vec3 frag_tangentLightPos;
layout (location = 4) out vec3 frag_tangentViewPos;
layout (location = 5) out vec3 frag_tangentFragPos;

layout (binding = 0) uniform ProjViewModel
{
    mat4 uProjView;
    mat4 uModel;
};

layout (binding = 1) uniform CameraInfo
{
    Material uMaterial;
    DirectionalLight uSun;
    vec4 uCameraPos;
};

void main()
{
    frag_texCoords = (aTexCoords * uMaterial.tiling.xy) + uMaterial.tiling.zw;
    frag_position = vec3(uModel * vec4(aPosition, 1.0));
    gl_Position = uProjView * vec4(frag_position, 1.0);
    frag_normal = mat3(transpose(inverse(uModel))) * aNormals;
    mat3 normal = transpose(inverse(mat3(uModel)));
    vec3 T = normalize(normal * aTangent);
    vec3 N = normalize(normal * aNormals);
    T = normalize(T - dot(T, N) * N);
    vec3 B = cross(N, T);
    mat3 TBN = transpose(mat3(T, B, N));
    frag_tangentLightPos = TBN * normalize(-uSun.direction.xyz);
    frag_tangentViewPos = TBN * uCameraPos.xyz;
    frag_tangentFragPos = TBN * frag_position;
}