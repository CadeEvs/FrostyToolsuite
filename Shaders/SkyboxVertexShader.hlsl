#pragma pack_matrix(row_major)

struct VS_IN
{
    float3 Position : POSITION;
};

struct VS_OUT
{
    float4 Position : SV_Position;
    float3 LocalPosition : POSITION;
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
    result.Position = mul(mul(mul(float4(input.Position, 1.0f), Model), View), Projection).xyww; // force w/z to = 1, so at back of depth range.
    result.LocalPosition = input.Position;
    return result;
}