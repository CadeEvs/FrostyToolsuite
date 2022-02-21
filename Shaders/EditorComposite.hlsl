
struct PS_IN
{
    float4 Position : SV_Position;
    float2 TexCoord : TEXCOORD;
};

Texture2D<float4> baseTexture;
Texture2D<float4> compositeTexture;
SamplerState textureSampler;

float4 PS_Main(PS_IN In) : SV_Target
{
	float4 color = baseTexture.Sample(textureSampler, In.TexCoord);
	float4 composite = compositeTexture.Sample(textureSampler, In.TexCoord);
	return float4(color.rgb + (composite.rgb * composite.a), 1.0f);
}