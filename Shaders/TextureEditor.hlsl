
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

Texture2DArray TextureArray : t0;
SamplerState Sampler;

VS_OUT VS_GridMain(VS_IN input)
{	
	VS_OUT output = (VS_OUT)0;
	output.Position = input.Position;
	output.TexCoord = input.TexCoord * ViewportDim;
	return output;
}

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
	float4 color = TextureArray.SampleLevel(Sampler, float3(input.TexCoord, SliceLevel), MipLevel).rgba;
	float origAlpha = color.a;

	float4 red = float4(color.r, color.r, color.r, 1.0f) * ChannelMask[0];
	float4 green = float4(color.g, color.g, color.g, 1.0f) * ChannelMask[1];
	float4 blue = float4(color.b, color.b, color.b, 1.0f) * ChannelMask[2];
	float4 alpha = float4(color.a, color.a, color.a, 1.0f) * ChannelMask[3];

	color = red + green + blue + alpha;
	color.a = (ChannelMask[3][3] != 0.0f) ? origAlpha : 1.0f;

	if (SrgbEnabled > 0.0f)
		color.rgb = LinearToSRgb(color.rgb);

	return color;
}