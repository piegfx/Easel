layout (location = 0) in vec2 frag_texCoords;
layout (location = 1) in vec4 frag_tint;
layout (location = 2) in vec4 frag_meta1;
layout (location = 3) in vec4 frag_meta2;

layout (location = 0) out vec4 out_color;

layout (binding = 1) uniform sampler2D uTexture;

float RectSDF(vec2 p, vec2 b, float r)
{
    vec2 d = abs(p) - b + vec2(r);
    return min(max(d.x, d.y), 0.0) + length(max(d, 0.0)) - r;
}

void main()
{
    vec2 pos = frag_meta1.zw * frag_texCoords;
    float fDist = RectSDF(pos - frag_meta1.zw / 2.0, frag_meta1.zw / 2.0 - frag_meta1.x / 2.0 - 1.0, frag_meta1.y);
    float fBlendAmount = smoothstep(-1.0, 1.0, abs(fDist) - frag_meta1.x / 2.0);

    vec4 v4ToColor = (fDist < 0.0) ? texture(uTexture, frag_texCoords) * frag_tint : vec4(0.0);
    out_color = mix(frag_meta2, v4ToColor, fBlendAmount);
}