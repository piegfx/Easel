#include "../Common/Common.hlsli"

struct VSInput
{
    float3 position: POSITION;
    float2 texCoord: TEXCOORD0;
    float3 normal:   NORMAL0;
    float3 tangent:  TANGENT0;
};

struct VSOutput
{
    float4 position: SV_Position;
    float2 texCoord: TEXCOORD0;
    float3 fragPos:  TEXCOORD1;
    float3 normal:   NORMAL;
};

struct PSOutput
{
    float4 albedo: SV_Target0;
    float4 pos:    SV_Target1;
    float4 normal: SV_Target2;
};

cbuffer CameraMatrices : register(b0)
{
    float4x4 projection;
    float4x4 view;
}

cbuffer RenderInfo : register(b1)
{
    float4x4 world;
    SceneInfo scene;
    Material material;
}

Texture2D albedo   : register(t2);
SamplerState state : register(s2);

VSOutput VertexShader(const in VSInput input)
{
    VSOutput output;

    const float4 fragPos = mul(world, float4(input.position, 1.0));

    output.position = mul(projection, mul(view, fragPos));
    output.fragPos = fragPos.xyz;

    output.texCoord = input.texCoord;

    output.normal = input.normal;

    return output;
}

PSOutput PixelShader(const in VSOutput input)
{
    PSOutput output;

    output.albedo = float4(albedo.Sample(state, input.texCoord).rgb, 1.0);
    output.pos = float4(input.fragPos, 1.0);
    output.normal = float4(input.normal, 1.0);

    return output;
}