#define FXAA_PC 1
#define FXAA_HLSL_5 1
#define FXAA_GREEN_AS_LUMA 1
#define FXAA_QUALITY__PRESET 26

#include "FXAA3_11.h"

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
    noperspective float2 texCoords: TEXCOORD0;
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

    vertexPos = mul(rot, vertexPos);
    vertexPos += input.origin * input.scale;

    output.position = mul(projView, float4(vertexPos, 0.0, 1.0));
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

    FxaaTex tex;
    tex.tex = sprite;
    tex.smpl = state;

    float4 color = FxaaPixelShader(
        input.texCoords,
        0.0,
        tex,
        tex,
        tex,
        frame,
        0.0,
        0.0,
        0.0,
        0.75,
        0.166,
        0.0833,
        0.0,
        0.0,
        0.0,
        0.0
    );

    output.color = color;
    
    return output;
}