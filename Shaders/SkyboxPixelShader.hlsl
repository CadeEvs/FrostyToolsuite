struct PS_IN
{
    float4 Position : SV_Position;
    float3 LocalPosition : POSITION;
};

cbuffer GradientConstants : register(b0)
{
    float sRGB;
	float useGradientColors;
	float2 padding;
    float4 TopColor;
    float4 HorizonColor;
}

cbuffer LightConstants : register(b1)
{
	float3 LightColor;
	float Intensity;
	float4 ExposureMultipliers;
	float HasLut;
}

TextureCube<float4> CubemapTexture : register(t0);
SamplerState CubemapSampler;

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

float4 main(PS_IN input) : SV_Target
{
	float4 color = CubemapTexture.Sample(CubemapSampler, input.LocalPosition);
	//if(sRGB > 0.0f)
	//	color.rgb = LinearToSRgb(color.rgb);
		
    return lerp(color, lerp(TopColor, HorizonColor, color.r), useGradientColors) * ExposureMultipliers.x * ExposureMultipliers.x;
}