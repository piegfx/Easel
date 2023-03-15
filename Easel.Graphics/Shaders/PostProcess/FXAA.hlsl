float3 FxaaPS(float4 posPos, Texture2D tex, SamplerState state, float2 frame)
{
    const float reduceMin = 1.0 / 128.0;
    const float reduceMul = 1.0 / 800.0;
    const float spanMax = 8.0;

    float3 rgbNW = tex.Sample(state, posPos.zw).rgb;
    float3 rgbNE = tex.Sample(state, posPos.zw, int2(1, 0)).rgb;
    float3 rgbSW = tex.Sample(state, posPos.zw, int2(0, 1)).rgb;
    float3 rgbSE = tex.Sample(state, posPos.zw, int2(1, 1)).rgb;
    float3 rgbM = tex.Sample(state, posPos.xy).rgb;

    const float3 luma = float3(0.299, 0.587, 0.114);
    float lumaNW = dot(rgbNW, luma);
    float lumaNE = dot(rgbNE, luma);
    float lumaSW = dot(rgbSW, luma);
    float lumaSE = dot(rgbSE, luma);
    float lumaM = dot(rgbM, luma);

    float lumaMin = min(lumaM, min(min(lumaNW, lumaNE), min(lumaSW, lumaSE)));
    float lumaMax = max(lumaM, max(max(lumaNW, lumaNE), max(lumaSW, lumaSE)));

    float2 dir = float2(-((lumaNW + lumaNE) - (lumaSW + lumaSE)), (lumaNW + lumaSW) - (lumaNE + lumaSE));

    float dirReduce = max((lumaNW + lumaNE + lumaSW + lumaSE) * (0.25 * reduceMul), reduceMin);

    float rcpDirMin = 1.0 / (min(abs(dir.x), abs(dir.y)) + dirReduce);

    dir = min((float2) spanMax, max(-(float2) spanMax, dir * rcpDirMin)) * frame;

    float3 rgbA = (1.0 / 2.0) * (tex.Sample(state, posPos.xy + dir * (1.0 / 3.0 - 0.5)).rgb + tex.Sample(state, posPos.xy + dir * (2.0 / 3.0 - 0.5)).rgb);

    float3 rgbB = rgbA * (1.0 / 2.0) + (1.0 / 4.0) * (tex.Sample(state, posPos.xy + dir * -0.5).rgb + tex.Sample(state, posPos.xy + dir * (1 - 0.5)).rgb);

    float lumaB = dot(rgbB, luma);
    if (lumaB < lumaMin || lumaB > lumaMax)
        return rgbA;
    else
        return rgbB;
}