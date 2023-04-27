/*
    (C) 2023 PIEGFX
    Provides the base interface for all Easel shader functions, as well as important includes.
*/

#pragma once

cbuffer CameraMatrices : register(b0)
{
    float4x4 Easel_ProjectionMatrix;
    float4x4 Easel_ViewMatrix;
    float4x4 Easel_ModelMatrix;
}

