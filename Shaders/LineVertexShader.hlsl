#pragma pack_matrix(row_major)

struct VS_IN
{
    float3 Position : POSITION;
    float4 Color : COLOR;
};

struct VS_OUT
{
    float4 Position : SV_Position;
    float4 Color : COLOR;
};

cbuffer FrameConstants : register(b0)
{
    float4x4 Projection;
    float4x4 View;
};

cbuffer ObjectConstants : register(b1)
{
    float4x4 Model;
};

VS_OUT main(VS_IN input)
{
    VS_OUT result;
    result.Position = mul(mul(mul(float4(input.Position, 1.0f), Model), View), Projection);
    result.Color = input.Color;
    return result;
}