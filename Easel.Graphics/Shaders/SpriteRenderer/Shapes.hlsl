struct SDFResult
{
    float dist;
    float blendAmount;
};

float RectSDF(float2 p, float2 b, float r)
{
    float2 d = abs(p) - b + (float2) r;
    return min(max(d.x, d.y), 0.0) + length(max(d, 0.0)) - r;
}

SDFResult RoundedRect(float2 size, float2 position, float2 texCoords)
{
    float2 pos = size * texCoords;
    float fDist = RectSDF(pos - size / 2.0, size / 2.0 - position.x / 2.0 - 1.0, position.y);
    float fBlendAmount = smoothstep(-1.0, 1.0, abs(fDist) - position.x / 2.0);

    SDFResult result;
    result.dist = fDist;
    result.blendAmount = fBlendAmount;

    return result;
}