struct PS_IN
{
    float4 Position : SV_Position;
    float3 LocalPosition : POSITION;
};

cbuffer GradientConstants : register(b0)
{
    float4 TopColor;
    float4 HorizonColor;
}

TextureCube<float4> CubemapTexture : register(t0);
SamplerState CubemapSampler;

float4 main(PS_IN input) : SV_Target
{
    return float4(lerp(TopColor, HorizonColor, CubemapTexture.Sample(CubemapSampler, input.LocalPosition).r).xyz, 1.0f);
}