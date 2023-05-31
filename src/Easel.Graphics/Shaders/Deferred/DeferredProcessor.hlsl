struct VSInput
{
    uint vertId: SV_VertexID;
};

struct VSOutput
{
    float4 position: SV_Position;
    float2 texCoord: TEXCOORD0;
};

struct PSOutput
{
    float4 color: SV_Target0;
};

Texture2D albedoTexture  : register(t0);
SamplerState albedoState : register(s0);
Texture2D posTexture  : register(t1);
SamplerState posState : register(s1);

VSOutput VertexShader(const in VSInput input)
{
    VSOutput output;

    const float4 vertices[] = {
        float4(-1.0, -1.0, 0.0, 0.0),
        float4( 1.0, -1.0, 1.0, 0.0),
        float4( 1.0,  1.0, 1.0, 1.0),
        float4(-1.0,  1.0, 0.0, 1.0),
    };

    const uint indices[] = {
        0, 1, 3,
        1, 2, 3
    };

    const float4 vertex = vertices[indices[input.vertId]];

    output.position = float4(vertex.xy, 0.0, 1.0);
    output.texCoord = vertex.zw;
    
    return output;
}

PSOutput PixelShader(const in VSOutput input)
{
    PSOutput output;

    const float4 albedo = albedoTexture.Sample(albedoState, input.texCoord);

    if (albedo.a < 0.9)
        discard;

    output.color = albedo;
    
    return output;
}