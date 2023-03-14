struct VSInput
{
    float3 position: POSITION;
};

struct VSOutput
{
    float4 position: SV_Position;
};

cbuffer ProjViewModel : register(b0)
{
    float4x4 projection;
    float4x4 view;
    float4x4 model;
    float4x4 lightSpace;
}

VSOutput VertexShader(in VSInput input)
{
    VSOutput output;
    output.position = float4(input.position, 1.0) * model * view * projection;

    return output;
}

void PixelShader(in VSOutput output) {}