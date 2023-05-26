struct VSInput
{
    float2 normalizedPosition: POSITION0;
    float2 screenPosition:     POSITION1;
    float2 texCoord:           TEXCOORD0;
    float4 tint:               COLOR0;
    float1 rotation:           TEXCOORD1;
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

    // Calculate a rotation matrix based on our input.
    const float sinRot = sin(input.rotation);
    const float cosRot = cos(input.rotation);
    const float2x2 rotMatrix = float2x2(
        float2(cosRot, -sinRot),
        float2(sinRot, cosRot)
    );

    // First, transform the input normalized coordinates by the matrix.
    // It must be done like this as the rotation MUST be done at (0, 0) to appear correct.
    // After the rotation is done, *then* we can add on our screen position. 
    const float2 position = mul(rotMatrix, input.normalizedPosition) + input.screenPosition;

    // Multiply by the projection and transform matrices.
    output.position = mul(projection, mul(transform, float4(position, 0.0, 1.0)));
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