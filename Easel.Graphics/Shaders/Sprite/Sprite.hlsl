/*
    (C) 2023 PIEGFX
    Performs sprite rendering.
*/

struct VSInput
{
    float2 position:  POSITION;
    float2 texCoords: TEXCOORD0;
    float4 tint:      COLOR0;
};

struct VSOutput
{
    float4 position:  SV_Position;
    float2 texCoords: TEXCOORD0;
    float4 tint:      COLOR0;
};

struct PSOutput
{
    float4 color: SV_Target0;
};

cbuffer Matrices : register(b0)
{
    float4x4 projection;
    float4x4 transform;
}

Texture2D sprite   : register(t1);
SamplerState state : register(s1);

VSOutput VertexShader(in VSInput input)
{
    VSOutput output;

    float2 position = input.position;
    
    output.position = mul(projection, mul(transform, float4(position, 0.0, 1.0)));
    output.texCoords = input.texCoords;
    output.tint = input.tint;

    return output;
}

PSOutput PixelShader(in VSOutput input)
{
    PSOutput output;

    output.color = sprite.Sample(state, input.texCoords) * input.tint;
    return output;
}