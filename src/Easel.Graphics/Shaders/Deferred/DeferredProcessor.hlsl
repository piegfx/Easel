#include "../Common/Common.hlsli"

[[vk::constant_id(0)]] const uint IS_OPENGL = 0;

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

cbuffer RenderInfo : register(b0)
{
    float4x4 world;
    SceneInfo scene;
    Material material;
}

Texture2D albedoTexture  : register(t1);
SamplerState albedoState : register(s1);
Texture2D posTexture  : register(t2);
SamplerState posState : register(s2);

VSOutput VertexShader(const in VSInput input)
{
    VSOutput output;

    // Stores position and texture coordinates in one.
    // XY = Position
    // ZW = Tex coords
    const float4 vertices[] = {
        float4(-1.0, -1.0, 0.0, 1.0),
        float4( 1.0, -1.0, 1.0, 1.0),
        float4( 1.0,  1.0, 1.0, 0.0),
        float4(-1.0,  1.0, 0.0, 0.0),
    };

    const uint indices[] = {
        0, 1, 3,
        1, 2, 3
    };

    const float4 vertex = vertices[indices[input.vertId]];

    output.position = float4(vertex.xy, 0.0, 1.0);

    // We must flip the texture coordinates if the graphics API is OpenGL.
    // Thanks OpenGL!!!!!!111111!11
    if (IS_OPENGL == 1)
    {
        output.texCoord = float2(vertex.z, 1.0 - vertex.w);
    }
    else
    {
        output.texCoord = vertex.zw;
    }
    
    return output;
}

PSOutput PixelShader(const in VSOutput input)
{
    PSOutput output;

    const float4 albedo = albedoTexture.Sample(albedoState, input.texCoord);
    const float4 pos = posTexture.Sample(posState, input.texCoord);

    if (albedo.a < 0.9)
        discard;

    output.color = float4(albedo.xyz * scene.ambientMultiplier, 1.0);
    
    return output;
}