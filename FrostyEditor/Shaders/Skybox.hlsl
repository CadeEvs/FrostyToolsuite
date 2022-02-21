
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
	float3 Normal : NORMAL;
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
	
	float3 worldPos = mul(float4(Input.Pos.xyz, 1.0f), worldMatrix).xyz - cameraPos;
    Out.Pos = mul(float4(worldPos, 1.0f), crViewProjMatrix);
	Out.Normal = mul((float3x3)worldMatrix, normalize(Input.Normal.xyz));
	return Out;
}

// -----------------------------------------------------------------------------------
// Pixel Shader
// -----------------------------------------------------------------------------------

struct PSOutput
{
    float4 GBufferA : SV_Target0;
    float4 GBufferB : SV_Target1;
    float4 GBufferC : SV_Target2;
	float4 GBufferD : SV_Target3;
    float Depth : SV_Target4;
};

cbuffer ExternalPixelParameters : register(b2)
{
	float4 BaseColor;
	float4 SMR;
};

TextureCube<float4> texture_normalBasisCubemapTexture : register(t0);

SamplerState sampler0_s : register(s0);
SamplerState textureSampler : register(s1);

// main entry point
float4 PSMain(VSOutput Input) : SV_Target
{
	//PSOutput Out = (PSOutput)0;
	
	//GBufferValues GBuffer = (GBufferValues)0;
	
	//GBuffer.BaseColor = BaseColor.rgb;
	//GBuffer.WorldNormals = Input.Normal;
	//GBuffer.Reflectance = SMR.b;
	//GBuffer.Smoothness = SMR.r;
	//GBuffer.Metallic = SMR.g;
	//GBuffer.MaterialAO = 1.0f;
	//GBuffer.Radiosity = CalculateRadiosity(lightProbe, GBuffer.WorldNormals);
	//GBuffer.Emissive = float3(0,0,0);
	//Out.Depth = Input.Pos.z;
	
	//PackGBufferValues(GBuffer, Out.GBufferA, Out.GBufferB, Out.GBufferC, Out.GBufferD, normalBasisTransforms, texture_normalBasisCubemapTexture, sampler0_s, exposureMultipliers.x);
	
	//return Out;
	return float4(BaseColor.rgb * exposureMultipliers.x, 1.0f);
}