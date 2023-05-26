struct VSInput
{
    float2 position: POSITION;
    float2 texCoord: TEXCOORD0;
    float4 tint:     COLOR0;
};

struct VSOutput
{
    float4 position: SV_Position;
    float2 texCoord: TEXCOORD0;
    float4 tint:     COLOR0;
};

struct PSOutput
{
    float4 color: SV_Target0;
};

cbuffer SpriteMatrices : register(b0)
{
    float4x4 projection;
    float4x4 transform;
}

Texture2D sprite   : register(t1);
SamplerState state : register(s1);

VSOutput VertexShader(const in VSInput input)
{
    VSOutput output;

    output.position = mul(projection, mul(transform, float4(input.position, 0.0, 1.0)));
    output.texCoord = input.texCoord;
    output.tint = input.tint;

    return output;
}

PSOutput PixelShader(const in VSOutput input)
{
    PSOutput output;

    output.color = sprite.Sample(state, input.texCoord) * input.tint;
    
    return output;
}