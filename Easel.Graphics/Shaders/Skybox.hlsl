struct VSInput
{
    float3 position : POSITION;
};

struct VSOutput
{
    float4 position: SV_Position;
    float3 texCoords: TEXCOORD0;
};

struct PSOutput
{
    float4 color: SV_Target0;
};

cbuffer CameraInfo : register(b0)
{
    float4x4 projection;
    float4x4 view;
};

TextureCube skybox : register(t1);
SamplerState state : register(s1);

VSOutput VertexShader(in VSInput input)
{
    VSOutput output;

    output.position = (float4(input.position, 1.0) * view * projection).xyww;
    output.texCoords = input.position;

    return output;
}

PSOutput PixelShader(in VSOutput input)
{
    PSOutput output;

    output.color = skybox.Sample(state, input.texCoords);

    return output;
}