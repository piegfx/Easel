#include "FXAA.hlsl"

struct VSInput
{
    float2 position:  POSITION;
    float2 texCoords: TEXCOORD0;
    float4 tint:      COLOR0;
    float1 rotation:  TEXCOORD1;
    float2 origin:    TEXCOORD2;
    float2 scale:     TEXCOORD3;
    float4 meta1:     TEXCOORD4;
    float4 meta2:     TEXCOORD5;
};

struct VSOutput
{
    float4 position:  SV_Position;
    float2 texCoords: TEXCOORD0;
    float4 tint:      COLOR0;
    float4 meta1:     TEXCOORD1;
    float4 meta2:     TEXCOORD2;
};

struct PSOutput
{
    float4 color: SV_Target0;
};

cbuffer ProjView : register(b0)
{
    float4x4 projView;
};

Texture2D sprite   : register(t1);
SamplerState state : register(s1);

VSOutput VertexShader(in VSInput input)
{
    VSOutput output;

    float cosRot = cos(input.rotation);
    float sinRot = sin(input.rotation);

    float2 vertexPos = input.position - (input.origin * input.scale);
    float2x2 rot = float2x2(
        cosRot, sinRot,
        -sinRot, cosRot
    );

    vertexPos = rot * vertexPos;
    vertexPos += input.origin * input.scale;

    output.position = float4(vertexPos, 0.0, 1.0) * projView;
    output.texCoords = input.texCoords;
    output.tint = input.tint;
    output.meta1 = input.meta1;
    output.meta2 = input.meta2;

    return output;
}

PSOutput PixelShader(in VSOutput input)
{
    PSOutput output;

    uint width, height;
    sprite.GetDimensions(width, height);

    float2 frame = float2(1.0 / width, 1.0 / height);

    float3 color = FxaaPS(float4(input.texCoords, input.texCoords - (frame * (0.5 + (1.0 / 4.0)))), sprite, state, frame);

    output.color = float4(color, 1.0);
    
    return output;
}