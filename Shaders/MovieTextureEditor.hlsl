
struct VS_IN
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
};

struct VS_OUT
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
};

cbuffer Constants
{
	float2 ViewportDim;
	float2 TextureDim;
	float4x4 ChannelMask;
	float SrgbEnabled;
	float MipLevel;
	float SliceLevel;
	float Padding;
};

Texture2D TextureY : t0;
Texture2D TextureU : t1;
Texture2D TextureV : t2;
SamplerState Sampler;

VS_OUT VS_Main(VS_IN input)
{
	float aspectRatio = (TextureDim.x > TextureDim.y) ? (TextureDim.y / TextureDim.x) : (TextureDim.x / TextureDim.y);
	float viewAspectRatio = (ViewportDim.x > ViewportDim.y) ? (ViewportDim.y / ViewportDim.x) : (ViewportDim.x / ViewportDim.y);

	float x = 1.0f;//(TextureDim.x > TextureDim.y) ? 1.0f : aspectRatio;
	float y = 1.0f;//(TextureDim.x > TextureDim.y) ? aspectRatio : 1.0f;

	//x = (ViewportDim.x > ViewportDim.y && y >= 1.0f) ? x * viewAspectRatio : x;
	//y = (ViewportDim.y >= ViewportDim.x && x >= 1.0f) ? y * viewAspectRatio : y;

	VS_OUT output = (VS_OUT)0;
	output.Position = float4(input.Position.x * x, input.Position.y * y, 0.0f, 1.0f);
	output.TexCoord = input.TexCoord;

	return output;
}

float3 SRgbToLinear(float3 color)
{
	color.r = (color.r <= 0.04045f) ? color.r / 12.92f : pow(((color.r + 0.055f) / 1.055f), 2.4f);
	color.g = (color.g <= 0.04045f) ? color.g / 12.92f : pow(((color.g + 0.055f) / 1.055f), 2.4f);
	color.b = (color.b <= 0.04045f) ? color.b / 12.92f : pow(((color.b + 0.055f) / 1.055f), 2.4f);
	return color;
}

float3 LinearToSRgb(float3 color)
{
	color.r = (color.r <= 0.0031308f) ? color.r * 12.92f : 1.055f * pow(color.r, 1.0f / 2.4f) - 0.055f;
	color.g = (color.g <= 0.0031308f) ? color.g * 12.92f : 1.055f * pow(color.g, 1.0f / 2.4f) - 0.055f;
	color.b = (color.b <= 0.0031308f) ? color.b * 12.92f : 1.055f * pow(color.b, 1.0f / 2.4f) - 0.055f;
	return color;
}

float4 PS_Main(VS_OUT input) : SV_Target
{
	float colorY = TextureY.Sample(Sampler, input.TexCoord).r;
	float colorU = TextureU.Sample(Sampler, input.TexCoord).r;
	float colorV = TextureV.Sample(Sampler, input.TexCoord).r;

	return float4(
		colorY + 1.402 * (colorV - 0.5),
		colorY - 0.344 * (colorU - 0.5) - 0.714 * (colorV - 0.5),
		colorY + 1.772 * (colorU - 0.5),
		1.0f
		);
}