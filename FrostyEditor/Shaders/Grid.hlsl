
#include "common.h"

// -----------------------------------------------------------------------------------
// Vertex Shader
// -----------------------------------------------------------------------------------

struct VSInput
{
	float3 Pos : POS;
	float3 Normal : NORMAL;
	float2 TexCoord0 : TEXCOORD0;
};

struct VSOutput
{
	float4 Pos : SV_POSITION;
	float3 WorldPos: POSITION;
	float3 Normal : NORMAL;
	float2 TexCoord0 : TEXCOORD0;
};

cbuffer viewConstants : register(b0)
{
    float4 time;
	float4 screenSize;
	float4x4 viewMatrix;
	float4x4 projMatrix;
	float4x4 viewProjMatrix;
	float4x4 crViewProjMatrix;
	float4x4 prevViewProjMatrix;
	float4x4 crPrevViewProjMatrix;
	float4x3 normalBasisTransforms[6];
	float4 exposureMultipliers;
	float3 cameraPos;
};

cbuffer functionConstants : register(b1)
{
    float4x4 worldMatrix;
	float4 lightProbe[9];
};

// main entry point
VSOutput VSMain(VSInput Input)
{
	VSOutput Out = (VSOutput)0;
	
	Out.WorldPos = mul(float4(Input.Pos.xyz * 512.0f, 1.0f), worldMatrix).xyz;
    Out.Pos = mul(float4(Out.WorldPos, 1.0f), viewProjMatrix);
	Out.Normal = mul((float3x3)worldMatrix, normalize(Input.Normal.xyz));
	Out.TexCoord0 = (Input.TexCoord0 * 1024.0f) + 0.5f;
	return Out;
}

// -----------------------------------------------------------------------------------
// Pixel Shader
// -----------------------------------------------------------------------------------

Texture2D<float4> gridTexture : register(t1);
SamplerState textureSampler : register(s1);

// main entry point
float4 PSMain(VSOutput Input) : SV_Target
{
	float2 pos = float2(Input.WorldPos.x, Input.WorldPos.z);
	float alpha = lerp(1.0f, 0.0f, length(pos) / 16.0f);
	
	float3 color = float3(0.5,0.5,0.5);
	color = lerp(color, float3(0.25,0,0), 1 - (clamp(abs(pos.y / 0.25), 0, 1)));
	color = lerp(color, float3(0,0,0.25), 1 - (clamp(abs(pos.x / 0.25), 0, 1)));
	
    return gridTexture.Sample(textureSampler, Input.TexCoord0) * float4(color, alpha);
}