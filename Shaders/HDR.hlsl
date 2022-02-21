
#include "Common.h"

static const int MAX_SAMPLES = 16;
//static const float3 LUMINANCE_VECTOR  = float3(0.2125f, 0.7154f, 0.0721f);
static const float3 LUMINANCE_VECTOR = float3(0.22f, 0.707f, 0.071f);

struct PS_IN
{
    float4 Position : SV_Position;
    float2 TexCoord : TEXCOORD;
};

Texture2D<float4> texture_frameBuffer : register(t0);
Texture2D<float> texture_prevLumBuffer : register(t1);

SamplerState sampler0_s;

cbuffer CommonConstants : register(b0)
{
	float4x4 g_invProjMatrix;
	float4x4 g_invViewMatrix;
	float3 g_cameraPos;
	float g_renderMode;
	float2 g_invScreenSize;
	float2 g_invDeviceZToWorldZTransform;
	float4 g_exposureMultipliers;
	float4x3 g_normalBasisTangents[6];
}

// ------------------------------------------
// Downscale 4x4
// ------------------------------------------

cbuffer DownScaleConstants : register(b1)
{
	float2 SampleOffsets[MAX_SAMPLES];
	float4 SampleWeights[MAX_SAMPLES];
}

float4 PS_DownScale4x4(PS_IN Input) : SV_Target
{
	float4 sample = 0.0f;

	for( int i=0; i < 16; i++ )
	{
		sample += texture_frameBuffer.Sample( sampler0_s, Input.TexCoord + SampleOffsets[i] );
	}
    
	return sample / 16;
}

// ------------------------------------------
// Sample Lum Initial
// ------------------------------------------

float EV100ToLog2(float EV100)
{
	return EV100 + 0.263f;
}

float4 PS_SampleLumInitial(PS_IN Input) : SV_Target
{
	float3 vSample = 0.0f;
    float  fLogLumSum = 0.0f;

    for(int iSample = 0; iSample < 9; iSample++)
    {
        vSample = texture_frameBuffer.Sample(sampler0_s, Input.TexCoord+SampleOffsets[iSample]).xyz * g_exposureMultipliers.y;
		vSample = max(float3(0.0001f, 0.0001f, 0.0001f), vSample);
		
        fLogLumSum += log2(dot(vSample, float3(0.2126, 0.7152, 0.0722)));
    }

    fLogLumSum /= 9;

    float3 result = float3(fLogLumSum, fLogLumSum, fLogLumSum);
	return float4(result, 1.0f);
}

// ------------------------------------------
// Sample Lum Iterative
// ------------------------------------------

float4 PS_SampleLumIterative(PS_IN Input) : SV_Target
{
	float fResampleSum = 0.0f; 
    
    for(int iSample = 0; iSample < 16; iSample++)
    {
        fResampleSum += texture_prevLumBuffer.Sample(sampler0_s, Input.TexCoord+SampleOffsets[iSample]).x;
    }
    
    fResampleSum /= 16;
	
	float3 result = float3(fResampleSum, fResampleSum, fResampleSum);
	return float4(result, 1.0f);
}

// ------------------------------------------
// Sample Lum Final
// ------------------------------------------

float4 PS_SampleLumFinal(PS_IN Input) : SV_Target
{
	float fResampleSum = 0.0f;
    
    for(int iSample = 0; iSample < 16; iSample++)
    {
        fResampleSum += texture_prevLumBuffer.Sample(sampler0_s, Input.TexCoord+SampleOffsets[iSample]);
    }
    
    fResampleSum = fResampleSum/16;
	
	//float HistogramLogMax = EV100ToLog2(g_exposureMultipliers.w);
	//float HistogramLogMin = EV100ToLog2(g_exposureMultipliers.z);
	
	//float deltaLog = HistogramLogMax-HistogramLogMin;
	//float multiply = 1.0f / deltaLog;
	//float add = -HistogramLogMin * multiply;
	//float minIntensity = exp2(HistogramLogMin);
	
	//fResampleSum = max(fResampleSum, minIntensity);
	
	return float4(exp2(fResampleSum).xxx, 1.0f);
}

// ------------------------------------------
// Calculate Adapted Lum
// ------------------------------------------

Texture2D<float> texture_adaptedLumTexturer : register(t0);
Texture2D<float> texture_currentLumTexture : register(t1);

cbuffer CalculateAdaptedLumConstants : register(b1)
{
	float ElapsedTime;
}

float EV100ToLuminance(float EV100)
{
	return 1.2f * pow(2.0f, EV100);
}

float4 PS_CalculateAdaptedLum(PS_IN Input) : SV_Target
{
	//float HistogramLogMax = EV100ToLog2(g_exposureMultipliers.w);
	//float HistogramLogMin = EV100ToLog2(g_exposureMultipliers.z);
	
	//float deltaLog = HistogramLogMax-HistogramLogMin;
	//float multiply = 1.0f / deltaLog;
	//float add = -HistogramLogMin * multiply;
	//float minIntensity = exp2(HistogramLogMin);
	
	float exposureOffsetMultipler = pow(2.0f, -0.6f);

    float fAdaptedLum = exposureOffsetMultipler / texture_adaptedLumTexturer.Load(int3(0,0,0));
    float fCurrentLum = clamp(texture_currentLumTexture.Load(int3(0,0,0)), EV100ToLuminance(g_exposureMultipliers.z), EV100ToLuminance(g_exposureMultipliers.w));
   	
	float diff = fCurrentLum-fAdaptedLum;
	float adaptionSpeed = (diff > 0) ? 3.0f : 1.0f;
	float factor = 1.0f - exp2(-ElapsedTime * adaptionSpeed);
	
	float retVal = clamp(fAdaptedLum + diff * factor, EV100ToLuminance(g_exposureMultipliers.z), EV100ToLuminance(g_exposureMultipliers.w));
	retVal = exposureOffsetMultipler * (1.0f / max(0.0001f, retVal));
	
	return float4(retVal, retVal, retVal, 1.0f);
}

// ------------------------------------------
// BrightPass
// ------------------------------------------

static const float MiddleGray = 0.5f;
static const float BRIGHT_PASS_THRESHOLD = 5.0f; 
static const float BRIGHT_PASS_OFFSET = 10.0f;

float4 PS_BrightPass(PS_IN Input) : SV_Target
{
	float bloomThreshold = 2.0f;
	
	float4 vSample = texture_frameBuffer.Sample( sampler0_s, Input.TexCoord ) * g_exposureMultipliers.y;
	float  fAdaptedLum = texture_currentLumTexture.Load(int3(0,0,0));
	
	float totalLuminance = dot(vSample.rgb, float3(0.3, 0.59, 0.11)) * fAdaptedLum;
	float bloomLuminance = totalLuminance - bloomThreshold;
	float bloomAmount = saturate(bloomLuminance / 2.0f);
    
	return float4(vSample.rgb * bloomAmount * g_exposureMultipliers.x, 1.0f);
}

// ------------------------------------------
// Gaussian Blur 5x5
// ------------------------------------------

float4 PS_GaussianBlur5x5(PS_IN Input) : SV_Target
{
	float4 sample = 0.0f;

	for( int i=0; i < 12; i++ )
	{
		sample += SampleWeights[i] * texture_frameBuffer.Sample( sampler0_s, Input.TexCoord + SampleOffsets[i] );
	}

	return sample;
}

// ------------------------------------------
// Downsample 2x2
// ------------------------------------------

float4 PS_DownSample2x2(PS_IN Input) : SV_Target
{
	float4 sample = 0.0f;

	for( int i=0; i < 4; i++ )
	{
		sample += texture_frameBuffer.Sample( sampler0_s, Input.TexCoord + SampleOffsets[i] );
	}
    
	return sample / 4;
}

// ------------------------------------------
// Bloom Blur
// ------------------------------------------

float4 PS_BloomBlur(PS_IN Input) : SV_Target
{
	float4 vSample = 0.0f;
    float4 vColor = 0.0f;
        
    float2 vSamplePosition;
    
    // Perform a one-directional gaussian blur
    for(int iSample = 0; iSample < 15; iSample++)
    {
        vSamplePosition = Input.TexCoord + SampleOffsets[iSample];
        vColor = texture_frameBuffer.Sample(sampler0_s, vSamplePosition);
        vSample += SampleWeights[iSample]*vColor;
    }
    
    return vSample;
}

// ------------------------------------------
// Render Bloom
// ------------------------------------------

Texture2D<float4> texture_bloomTexture : register(t0);

float4 PS_RenderBloom(PS_IN Input) : SV_Target
{
	float4 bloom = texture_bloomTexture.Sample(sampler0_s, Input.TexCoord);
	return float4(bloom.rgb, 1.0f);
}