#include "Easel.Graphics.Shaders.Material.glsl"

layout (location = 0) in VertexInfo
{
    vec2 texCoords;
} in_data;

layout (location = 0) out vec4 out_color;

layout (binding = 1) uniform MatInfo
{
    Material uMaterial;
};

layout (binding = 2) uniform sampler2D uDiffuse;
// These textures are only defined in the standard shader.
#ifdef STANDARD
layout (binding = 3) uniform sampler2D uSpecular;
layout (binding = 4) uniform sampler2D uNormal;
#endif

void main()
{
    out_color = texture(uDiffuse, in_data.texCoords) * uMaterial.color;
    
    // Standard materials do not support transparency whatsoever. Therefore, we must remove it.
    #ifndef TRANSPARENCY
    out_color = vec4(out_color.xyz, 1.0);
    #endif
}