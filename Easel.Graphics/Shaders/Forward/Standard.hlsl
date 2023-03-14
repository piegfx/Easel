#include "../Types.hlsl"
#include "../Lighting.hlsl"

struct VSInput
{
    float3 position:  POSITION;
    float2 texCoords: TEXCOORD;
    float3 normals:   NORMAL;
    float3 tangents:  TANGENT;
};

struct VSOutput
{
    float4 position:   SV_Position;
    float2 texCoords:  TEXCOORD0;
    float3 normal:     NORMAL;
    float3 fragPos:    TEXCOORD1;
    float3 lightSpace: TEXCOORD2;
};

struct PSOutput
{
    float4 color : SV_Target0;
};

cbuffer ProjViewModel : register(b0)
{
    float4x4 projection;
    float4x4 view;
    float4x4 model;
    float4x4 lightSpace;
}

cbuffer SceneInfo : register(b1)
{
    float4 cameraPos;
    Material material;
    DirectionalLight sun;
}

// TODO: Probably can make this a single sampler state.

Texture2D shadowTex      : register(t2);
SamplerState shadowState : register(s2);
Texture2D albedoTex      : register(t3);
SamplerState albedoState : register(s3);
Texture2D normalTex      : register(t4);
SamplerState normalState : register(s4);

Texture2D metallicTex       : register(t5);
SamplerState metallicState  : register(s5);
Texture2D roughnessTex      : register(t6);
SamplerState roughnessState : register(s6);
Texture2D aoTex             : register(t7);
SamplerState aoState        : register(s7);

// Various options during shader compilation.
// 0x1 = Lighting
// 0x2 = Combine textures
// 0x4 = Alpha transparency
[[vk::constant_id(0)]] const uint options = 0;

VSOutput VertexShader(in VSInput input)
{
    VSOutput output;

    float4 fragPos = float4(input.position, 1.0) * model;
    
    output.position = fragPos * view * projection;
    output.texCoords = input.texCoords * material.tiling.xy;
    output.normal = input.normals * (float3x3) model;
    output.fragPos = (float3) fragPos;
    output.lightSpace = fragPos * lightSpace;

    return output;
}

PSOutput PixelShader(in VSOutput input)
{
    PSOutput output;

    float4 albedo = pow(albedoTex.Sample(albedoState, input.texCoords), (float4) 2.2) * material.albedo;
    float3 normal = TempNormal(normalTex, normalState, input.fragPos, input.texCoords, input.normal);

    float metallic, roughness, ao;

    // if()
    metallic = metallicTex.Sample(metallicState, input.texCoords).r * material.metallic;
    roughness = roughnessTex.Sample(roughnessState, input.texCoords).r * material.roughness;
    ao = aoTex.Sample(aoState, input.texCoords).r * material.ao;

    float3 viewDir = normalize((float3) cameraPos - input.fragPos);
    float3 result = ProcessDirLight(sun, viewDir, albedo.rgb, normal, metallic, roughness);

    float shadow = 0.0;

    float3 ambient = (float3) 0.03 * albedo.rgb * ao;
    float3 color = ambient + (1.0 - shadow) * result;

    color = color / (color + (float3) 1.0);
    color = pow(color, (float3) (1.0 / 2.2));

    output.color = float4(color, 1.0);

    return output;
}