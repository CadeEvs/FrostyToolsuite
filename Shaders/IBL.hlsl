
#define PI (3.1415926535897932f)
#define INV_PI (0.31830988618379067543484808035204f)

#include "Common.h"

struct PS_IN
{
    float4 Position : SV_Position;
    float2 TexCoords : TEXCOORD;
	float2 ScreenCoords : TEXCOORD1;
};

cbuffer CubeMapConstants : register(b0)
{
	int CubeFace;
	uint MipIndex;
	uint NumMips;
}

TextureCube IBLCube : register(t0);
SamplerState incomingLightSampler : register(s0);

float3 F_Schlick(float3 f0, float f90, float u)
{
	return f0 + (f90-f0) * pow(1.f - u, 5.f);
}

float Fr_DisneyDiffuse(float NoV, float NoL, float LoH, float linearRoughness)
{
	float energyBias = lerp(0, 0.5f,linearRoughness);
	float energyFactor = lerp(1.0f, 1.0f/1.51f, linearRoughness);
	float fd90 = energyBias + 2.0f * LoH*LoH * linearRoughness;
	float3 f0 = float3(1,1,1);
	float lightScatter = F_Schlick(f0, fd90, NoL).r;
	float viewScatter = F_Schlick(f0, fd90, NoV).r;
	
	return lightScatter*viewScatter*energyFactor;
}

float D_GGX(float NoH, float m)
{
	float m2 = m*m;
	float f = (NoH * m2 - NoH)*NoH + 1;
	return m2/(f*f);
}

float D_GGX_Divide_Pi(float NoH, float m)
{
	float m2 = m*m;
	float f = (NoH * m2 - NoH)*NoH + 1;
	return (m2/(PI*f*f));
}

float G_SmithGGX( float NoL, float NoV, float a2 )
{
	float a = sqrt(a2);
	float Vis_SmithV = NoL * ( NoV * ( 1 - a ) + a );
	float Vis_SmithL = NoV * ( NoL * ( 1 - a ) + a );
	return 0.5 * rcp( Vis_SmithV + Vis_SmithL );
}

float2 getSample( uint Index, uint NumSamples, uint2 Random )
{
	float E1 = frac( (float)Index / NumSamples + float( Random.x & 0xffff ) / (1<<16) );
	float E2 = float( reversebits(Index) ^ Random.y ) * 2.3283064365386963e-10;
	return float2( E1, E2 );
}

void importanceSampleCosDir(in float2 u, out float3 L, out float NdotL, out float pdf)
{
	float u1 = u.x;
	float u2 = u.y;

	float r = sqrt(u1);
	float phi = u2 * PI * 2;

	L = float3(r * cos(phi), r * sin(phi), sqrt(max(0.0f ,1.0f-u1)));

	NdotL = saturate(L.z);
	pdf = NdotL * INV_PI;
}

void importanceSampleCosDir_N(in float2 u, in float3 N, out float3 L, out float NdotL, out float pdf)
{
	float3 upVector = abs(N.z) < 0.999 ? float3(0,0,1) : float3(1,0,0);
	float3 tangentX = normalize(cross(upVector, N));
	float3 tangentY = cross(N, tangentX);
	
	float u1 = u.x;
	float u2 = u.y;

	float r = sqrt(u1);
	float phi = u2 * PI * 2;

	L = float3(r * cos(phi), r * sin(phi), sqrt(max(0.0f, 1.0f-u1)));
	L = normalize(tangentX * L.y + tangentY * L.x + N * L.z);

	NdotL = dot(L, N);
	pdf = NdotL * INV_PI;
}

void importanceSampleGGX_G(float2 u, float3 V, float NdotV, float Roughness, out float NdotH, out float LdotH, out float NdotL, out float G)
{
	// GGX NDF sampling
	float cosThetaH = sqrt((1-u.x) / (1 + (Roughness*Roughness - 1) * u.x));
	float sinThetaH = sqrt(1 - min(1.0, cosThetaH * cosThetaH));
	float phiH = u.y * PI * 2;
	
	float3 H = float3( sinThetaH * cos(phiH), sinThetaH * sin(phiH), cosThetaH);
	float3 L = normalize(2.0f * dot(V,H) * H - V);
	
	NdotH = saturate(H.z);
	NdotL = saturate(L.z);
	LdotH = saturate(dot(H, L));
	
	G = G_SmithGGX(NdotL, NdotV, Roughness);
}

void importanceSampleGGX_Dir(float2 u, float3 V, float3 N, float Roughness, out float3 H, out float3 L)
{
	float3 upVector = abs(N.z) < 0.999 ? float3(0,0,1) : float3(1,0,0);
	float3 tangentX = normalize(cross(upVector, N));
	float3 tangentY = cross(N, tangentX);
	
	float a = Roughness * Roughness;
	float a2 = a*a;
	
	// GGX NDF sampling
	float cosThetaH = sqrt((1-u.x) / (1 + (Roughness*Roughness - 1) * u.x));
	float sinThetaH = sqrt(1 - min(1.0, cosThetaH * cosThetaH));
	float phiH = u.y * PI * 2;
	
	H = float3( sinThetaH * cos(phiH), sinThetaH * sin(phiH), cosThetaH);
	H = normalize(tangentX * H.y + tangentY * H.x + N * H.z);
	L = 2.0f * dot(V,H) * H - V;
}

float4 importanceSampleGGX( float2 E, float a2 )
{
	float Phi = 2 * PI * E.x;
	float CosTheta = sqrt( (1 - E.y) / ( 1 + (a2 - 1) * E.y ) );
	float SinTheta = sqrt( 1 - CosTheta * CosTheta );

	float3 H;
	H.x = SinTheta * cos( Phi );
	H.y = SinTheta * sin( Phi );
	H.z = CosTheta;
	
	float d = ( CosTheta * a2 - CosTheta ) * CosTheta + 1;
	float D = a2 / ( PI*d*d );
	float PDF = D * CosTheta;

	return float4( H, PDF );
}

// frostbite DFG integrate (doesnt work yet)
float4 integrateDFGOnly(in float NdotV, in float roughness)
{
	uint sampleCount=32;
	
	float3 V = float3(0,0,0);
	V.x = sqrt(1-NdotV * NdotV);
	V.y = 0.0f;
	V.z = NdotV;
	
	float4 acc = 0;
	float accWeight = 0;
	
	for(uint i = 0; i < sampleCount; ++i)
	{
		float2 u = getSample(i, sampleCount, 0);
		float NdotL = 0;
		float NdotH = 0;
		float LdotH = 0;
		float3 L = 0;
		float G = 0;
		
		importanceSampleGGX_G(u, V, NdotV, roughness, NdotH, LdotH, NdotL, G);
		if (NdotL > 0 && G > 0)
		{
			float GVis = G * LdotH / (NdotH * NdotV);
			float Fc = pow(1-LdotH, 5.f);
			acc.x += (1-Fc)*GVis;
			acc.y += Fc*GVis;
		}
		
		u = frac(u + 0.5);
		float pdf = 0;
		
		importanceSampleCosDir(u, L, NdotL, pdf);
		if (NdotL > 0)
		{
			LdotH = saturate(dot(L, normalize(V+L)));
			acc.z += Fr_DisneyDiffuse(NdotV, NdotL, LdotH, sqrt(roughness));
		}
		
		accWeight += 1.0;
	}
	
	return acc * (1.0f / accWeight);
}

// ue4 integrate DFG
float4 integrateDFGOnly_UE4(in float NoV, in float roughness)
{
	uint sampleCount=128;
	
	float3 V = float3(0,0,0);
	V.x = sqrt(1-NoV * NoV);
	V.y = 0.0f;
	V.z = NoV;
	
	float4 acc = 0;
	float accWeight = 0;
	
	float m = roughness * roughness;
	float m2 = m * m;
	
	for(uint i = 0; i < sampleCount; ++i)
	{
		float2 u = getSample(i, sampleCount, 0);
		float Phi = 2.0f * PI * u.x;
		float CosPhi = cos(Phi);
		float SinPhi = sin(Phi);
		float CosTheta = sqrt((1.0f - u.y) / (1.0f + (m2 - 1.0f) * u.y));
		float SinTheta = sqrt(1.0f - CosTheta * CosTheta);
		
		float3 H = float3(SinTheta * cos(Phi), SinTheta * sin(Phi), CosTheta);
		float3 L = 2.0f * dot(V, H) * H - V;
		
		float NoL = saturate(L.z);
		float NoH = saturate(H.z);
		float VoH = saturate(dot(V, H));
		
		if (NoL > 0.0f)
		{
			float Vis_SmithV = NoL * (NoV * (1-m) + m);
			float Vis_SmithL = NoV * (NoL * (1-m) + m);
			float Vis = 0.5f / (Vis_SmithV + Vis_SmithL);
			
			float NoL_Vis_PDF = NoL * Vis * (4.0f * VoH / NoH);
			float Fc = 1.0f - VoH;
			Fc *= ((Fc * Fc) * (Fc * Fc));
			acc.x += NoL_Vis_PDF * (1.0f - Fc);
			acc.y += NoL_Vis_PDF * Fc;
		}
		
		u = frac(u + 0.5);
		float pdf = 0;
		float NdotL;
		
		importanceSampleCosDir(u, L, NdotL, pdf);
		if (NdotL > 0)
		{
			float LdotH = saturate(dot(L, normalize(V+L)));
			acc.z += Fr_DisneyDiffuse(NoV, NdotL, LdotH, sqrt(roughness));
		}
		
		accWeight += 1.0;
	}
	
	return acc * (1.0f / accWeight);
}

float4 integrateDiffuseCube(in float3 N)
{
	uint sampleCount=32;

	float3 accBrdf = 0;
	float accWeight = 0;
	
	for (uint i = 0; i < sampleCount; ++i)
	{
		float2 eta = getSample(i, sampleCount, 0);
		float3 L;
		float NdotL;
		float pdf;
		
		importanceSampleCosDir_N(eta, N, L, NdotL, pdf);
		if (NdotL > 0)
			accBrdf += IBLCube.SampleLevel(incomingLightSampler, L, 0).rgb;
		
		accWeight += 1.0;
	}
	
	return float4(accBrdf * (1.0f / accWeight), 1.0f);
}

float4 integrateCubeLDOnly(in float3 V, in float3 N, in float roughness)
{
	float3 accBrdf = 0;
	float accBrdfWeight = 0;
	uint sampleCount = 32;
	uint cubeSize = 1 << ( NumMips - 1 );
	
	for (uint i = 0; i < sampleCount; ++i)
	{
		float2 eta = getSample(i, sampleCount, 0);
		float3 L;
		float3 H;
		importanceSampleGGX_Dir(eta, V, N, roughness, H, L);
		float NdotL = dot(N, L);
		if (NdotL > 0)
		{
			float NdotH = saturate(dot(N, H));
			float LdotH = saturate(dot(L, H));
			float pdf = D_GGX_Divide_Pi(NdotH, roughness) * NdotH / (4 * LdotH);
			float omegaS = 1.0 / (sampleCount * pdf);
			float omegaP = 4.0*PI / (6.0 * cubeSize * cubeSize) * 2;
			float mipLevel = 0.5 * log2(omegaS/omegaP);
			float4 Li = IBLCube.SampleLevel(incomingLightSampler, L, mipLevel);
			//Li = float4(lerp(float3(1200, 1700, 1820), Li.rgb, Li.a), 1.0f);
			
			accBrdf += Li.rgb * NdotL;
			accBrdfWeight += NdotL;
		}
	}
	return float4(accBrdf * (1.0f / accBrdfWeight), 1.0f);
}

float Pow4( float x )
{
	float xx = x*x;
	return xx * xx;
}

float4 integrateCubeLDOnly_UE4(in float3 V, in float3 N, in float roughness)
{
	float3 accBrdf = 0;
	float accBrdfWeight = 0;
	uint sampleCount = 64;
	uint cubeSize = 1 << ( NumMips - 1 );
	float m = roughness*roughness;
	
	for (uint i = 0; i < sampleCount; ++i)
	{
		float2 eta = getSample(i, sampleCount, 0);
		eta.y *= 0.995;
		
		float3 H = importanceSampleGGX(eta, m*m).xyz;
		float3 L = 2 * H.z * H -float3(0,0,1);
		
		float NdotL = L.z;
		float NdotH = H.z;
		
		if (NdotL > 0)
		{
			float LdotH = saturate(dot(L, H));
			
			float3 upVector = abs(N.z) < 0.999 ? float3(0,0,1) : float3(1,0,0);
			float3 tangentX = normalize(cross(upVector, N));
			float3 tangentY = cross(N, tangentX);
	
			float pdf = D_GGX_Divide_Pi(NdotH, roughness) * NdotH / (4 * LdotH);
			float omegaS = 1.0 / (sampleCount * pdf);
			float omegaP = 4.0*PI / (6.0 * cubeSize * cubeSize) * 2;
			float mipLevel = 0.5 * log2(omegaS/omegaP);
			
			L = tangentX * L.y + tangentY * L.x + N * L.z;
			float4 Li = IBLCube.SampleLevel(incomingLightSampler, L, mipLevel);
			
			accBrdf += Li.rgb * NdotL;
			accBrdfWeight += NdotL;
		}
	}
	return float4(accBrdf * (1.0f / accBrdfWeight), 1.0f);
}

float3 GetCubemapVector(float2 ScaledUVs)
{
	float3 CubeCoordinates;

	if (CubeFace == 0)
	{
		CubeCoordinates = float3(1, -ScaledUVs.y, -ScaledUVs.x);
	}
	else if (CubeFace == 1)
	{
		CubeCoordinates = float3(-1, -ScaledUVs.y, ScaledUVs.x);
	}
	else if (CubeFace == 2)
	{
		CubeCoordinates = float3(ScaledUVs.x, 1, ScaledUVs.y);
	}
	else if (CubeFace == 3)
	{
		CubeCoordinates = float3(ScaledUVs.x, -1, -ScaledUVs.y);
	}
	else if (CubeFace == 4)
	{
		CubeCoordinates = float3(ScaledUVs.x, -ScaledUVs.y, 1);
	}
	else
	{
		CubeCoordinates = float3(-ScaledUVs.x, -ScaledUVs.y, -1);
	}

	return CubeCoordinates;
}

#define REFLECTION_CAPTURE_ROUGHEST_MIP 1
#define REFLECTION_CAPTURE_ROUGHNESS_MIP_SCALE 1.2

float ComputeReflectionCaptureRoughnessFromMip(float Mip, float CubemapMaxMip)
{
	float LevelFrom1x1 = CubemapMaxMip - 1 - Mip;
	return exp2( ( REFLECTION_CAPTURE_ROUGHEST_MIP - LevelFrom1x1 ) / REFLECTION_CAPTURE_ROUGHNESS_MIP_SCALE );
}

float ComputeReflectionCaptureMipFromRoughness(float Roughness, float MaxMips)
{
	float LevelFrom1x1 = 1 - 1.2 * log2(Roughness);
	return MaxMips - 1 - LevelFrom1x1;
}

// ---------------------------------------------------
// DFG
// ---------------------------------------------------

float4 PS_DFG(PS_IN In) : SV_Target
{
	float NdotV = In.TexCoords.x;
	float Roughness = In.TexCoords.y;
	return integrateDFGOnly_UE4(NdotV, Roughness*Roughness);
}

// ---------------------------------------------------
// Diffuse LD
// ---------------------------------------------------

float4 PS_DiffuseLD(PS_IN In) : SV_Target
{
	float2 ScaledUVs = In.TexCoords * 2 - 1;
	float3 CubeCoordinates = GetCubemapVector(ScaledUVs);
	
	return integrateDiffuseCube(normalize(CubeCoordinates));
}

// ---------------------------------------------------
// Specular LD
// ---------------------------------------------------

float4 PS_SpecularLD(PS_IN In) : SV_Target
{
	float2 ScaledUVs = In.TexCoords * 2 - 1;
	float3 CubeCoordinates = GetCubemapVector(ScaledUVs);
	float Roughness = ComputeReflectionCaptureRoughnessFromMip(MipIndex, NumMips);
	
	if (MipIndex == 0)
	{
		float4 color = IBLCube.SampleLevel(incomingLightSampler, CubeCoordinates, 0);
		return float4(color.rgb, 1.0f);//float4(lerp(float3(1200, 1700, 1820), color.rgb, color.a), 1.0f);
	}
		
	return integrateCubeLDOnly(normalize(CubeCoordinates), normalize(CubeCoordinates), Roughness*Roughness);
}

// ---------------------------------------------------
// IBL Reflection Environment
// ---------------------------------------------------

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
	float g_lightProbeIntensity;
}

Texture2D<float4> g_gbufferTexture0 : register(t0);
Texture2D<float4> g_gbufferTexture1 : register(t1);
Texture2D<float4> g_gbufferTexture2 : register(t2);
Texture2D<float4> g_gbufferTexture3 : register(t3);
Texture2D<float> g_depthTexture : register(t4);
Texture2D<float4> g_preintegratedDFG : register(t5);
TextureCube<float4> g_preintegratedDiffuseLD : register(t6);
TextureCube<float4> g_preintegratedSpecularLD : register(t7);
TextureCube<float4> g_distantProbe : register(t8);

SamplerState sampler0_s : register(s0);
SamplerState sampler1_s : register(s1);

float3 getWorldPosition(float2 texCoords, float depth)
{
	float4 worldPos;
	worldPos.x = texCoords.x * 2 - 1;
	worldPos.y = (1 - texCoords.y) * 2 - 1;
	worldPos.z = depth;
	worldPos.w = 1.0f;
	
	worldPos = mul(worldPos, g_invProjMatrix);
	worldPos.xyz /= worldPos.w;
	
	// @todo: no idea why i need the -1 scale on X, but if i dont, then lighting
	//        is all wrong
	
	return worldPos.xyz * float3(-1,1,1);
}

float3 getSpecularDominantDir(float3 N, float3 R, float roughness)
{
	float smoothness = saturate(1-roughness);
	float lerpFactor = smoothness * (sqrt(smoothness) + roughness);
	return lerp(N, R, lerpFactor);
}

float3 getDiffuseDominantDir(float3 N, float3 V, float NdotV, float roughness)
{
	float a = 1.02341f * roughness - 1.51174f;
	float b = -0.511705f * roughness + 0.755868f;
	float lerpFactor = saturate((NdotV * a + b) * roughness);
	return lerp(N, V, lerpFactor);
}

float3 evaluateIBLDiffuse(float3 N, float3 V, float NdotV, float roughness)
{
	float3 dominantN = getDiffuseDominantDir(N, V, NdotV, roughness);
	float3 diffuseLighting = g_preintegratedDiffuseLD.Sample(sampler1_s, dominantN).rgb;
	
	float diffF = g_preintegratedDFG.SampleLevel(sampler0_s, float2(NdotV, roughness), 0).z;
	return diffuseLighting * diffF;
}

float3 evaluateIBLSpecular(float3 N, float3 R, float NdotV, float roughness, float linearRoughness, float3 f0, float fd90)
{
	float3 dominantR = getSpecularDominantDir(N, R, roughness);
	
	NdotV = max(NdotV, 0.5f / 128.0f);
	float mipLevel = ComputeReflectionCaptureMipFromRoughness(linearRoughness, 9);
	float3 preLD = g_preintegratedSpecularLD.SampleLevel(sampler1_s, dominantR, mipLevel).rgb;
	
	float2 preDFG = g_preintegratedDFG.SampleLevel(sampler0_s, float2(NdotV, roughness), 0).xy;
	return preLD * (f0 * preDFG.x + fd90 * preDFG.y);
}

float3 evaluateSpecularIBLReference(in float3 N, in float3 V, in float roughness, in float3 f0, in float f90)
{
	// Build local referential
	float3 upVector = abs(N.z) < 0.999 ? float3(0,0,1) : float3(1,0,0);
	float3 tangentX = normalize( cross( upVector, N));
	float3 tangentY = cross( N, tangentX);

	float3 accLight = 0;
	for( uint i =0; i< 128 ; ++i)
	{
		float2 u = getSample(i, 128, 0);

		// GGX NDF sampling
		float cosThetaH = sqrt((1-u.x) /(1 +( roughness * roughness -1)*u.x));
		float sinThetaH = sqrt(1 - min(1.0, cosThetaH * cosThetaH));
		float phiH = u.y * PI * 2;

		// Convert sample from half angle to incident angle
		float3 H, L;
		H = float3( sinThetaH * cos( phiH), sinThetaH *sin( phiH), cosThetaH);
		H = normalize( tangentX * H.y + tangentY * H.x + N * H.z);
		L = normalize(2.0f * dot(V,H) * H - V);

		float LdotH = saturate( dot(H, L));
		float NdotH = saturate( dot(H, N));
		float NdotV = saturate( dot(V, N));
		float NdotL = saturate( dot(L, N));

		NdotV = max(NdotV, 0.5f / 128.0f);
		
		// Importance sampling weight for each sample
		//
		// weight = fr .(N.L)
		//
		// with :
		// fr = D(H).F(H).G(V, L) /( 4(N.L)(N.O))
		//
		// Since we integrate in the microfacet space, we include the
		// Jacobian of the transform
		//
		// pdf = D(H) .(N.H) /( 4(L.H))
		float D = D_GGX(NdotH, roughness);
		float pdfH = D * NdotH ;
		float pdf = pdfH /(4.0f * LdotH);

		// Implicit weight(N.L canceled out)
		float3 F = F_Schlick(f0, f90, LdotH);
		float G = G_SmithGGX(NdotL, NdotV, roughness);
		float3 weight = F * G * D /(4.0 * NdotV);

		if( dot(L,N) >0 && pdf > 0)
		{
			accLight += g_distantProbe.SampleLevel(sampler1_s, L, 0). rgb * weight / pdf ;
		}
	}
	return accLight / 128;
}

float3 evaluateIBLDiffuseCubeReference(in TextureCube<float4> incomingLight, in SamplerState incomingLightSampler, in float3 V, in float3 worldNormal, float roughness, in float3 brdfWeight = float3(0, 0, 0), in uint sampleCount = 128)
{
	float3 accLight = 0;

	for(uint i = 0; i < sampleCount; ++i)
	{
		float2 eta = getSample(i, sampleCount, 0);
		float3 L;
		float NdotL;
		float pdf;
		importanceSampleCosDir_N(eta, worldNormal, L, NdotL, pdf);
		if (NdotL >0)
		{
			// Each sample should be weighted by L * weight / pdf .
			// With :
			// - weight = NdotL
			// - pdf = NdoL / Pi
			// However the NdoLs (in weight and pdf ) and Pi cancel out
			// This is why all terms disappear here
			float f = 1.0f;

			//#if FB_DIFFUSE_MODEL == FB_DIFFUSE_DISNEY
			// Half angle formula :
			// cos (2 theta ) = 2 cos ^2( theta ) - 1
			float cosD = sqrt((dot(V, L) + 1.0f) * 0.5);
			float NdotV = saturate(dot(worldNormal, V));
			float NdotL_sat = saturate(NdotL);
			// Disney diffuse BRDF operates in linear roughness ,
			// which is the sqrt of the GGX alpha roughness term
			float fd90 = 0.5 + 2 * cosD * cosD * sqrt(roughness);
			float lightScatter = 1 + (fd90 -1) * pow(1 - NdotL_sat, 5);
			float viewScatter = 1 + (fd90 -1) * pow(1 - NdotV, 5);
			f = lightScatter * viewScatter ;
			//# endif

			accLight += incomingLight.SampleLevel(incomingLightSampler, L, 0).rgb * f;
		}
	}

	return accLight * (1.0f / sampleCount);
}

float3 diffuseBSDF(float3 V, float3 L, GBufferValues values)
{
	float3 N = values.WorldNormals;
	
	float  NoL = saturate(dot(N, L));
	float3 H = normalize(V + L);
	float  NoV = abs(dot(N, V)) + 1e-5f;
	float  NoH = saturate(dot(N, H));
	float  VoH = saturate(dot(V, H));
	float  LoH = saturate(dot(L, H));
	
	// diffuse BRDF
	float  Fd = Fr_DisneyDiffuse(NoV, NoL, LoH, values.LinearRoughness) / PI;
	
	return values.BaseColor * Fd;
}

float4 PS_IBL(PS_IN In) : SV_Target
{
	GBufferValues GBuffer = UnpackGBufferValues(
		g_gbufferTexture0.Load(int3(In.ScreenCoords, 0)),
		g_gbufferTexture1.Load(int3(In.ScreenCoords, 0)),
		g_gbufferTexture2.Load(int3(In.ScreenCoords, 0)),
		g_gbufferTexture3.Load(int3(In.ScreenCoords, 0)),
		g_normalBasisTangents,
		g_exposureMultipliers.y
		);
		
	float depth = g_depthTexture.Load(int3(In.ScreenCoords, 0));
	float3 worldPosition = getWorldPosition(In.TexCoords, depth);
	
	if (depth >= 1)
		discard;
		
	float3 V = normalize(g_cameraPos - worldPosition);
	float3 N = normalize(GBuffer.WorldNormals);
	float3 R = 2 * dot(V, N) * N - V;
	float NdotV = saturate(dot(N,V));
	float3 L = getSpecularDominantDir(N, R, GBuffer.Roughness);
	float3 H = normalize(V + L);
	float  LoH = saturate(dot(L, H));
	
	//float energyBias = lerp(0, 0.5f,GBuffer.LinearRoughness);
	//float energyFactor = lerp(1.0f, 1.0f/1.51f, GBuffer.LinearRoughness);
	//float fd90 = energyBias + 2.0f * LoH*LoH * GBuffer.LinearRoughness;
	
	//float cosD = sqrt((dot(V, L) + 1.0f) * 0.5);
	//float fd90 = 0.5 + 2 * cosD * cosD * sqrt(GBuffer.Roughness);
	
	float fd90 = saturate(50.0f * GBuffer.SpecularColor.g);
	
	//float3 diffuseColor = evaluateIBLDiffuse(N, V, NdotV, GBuffer.Roughness);
	//float3 diffuseColor = evaluateIBLDiffuseCubeReference(g_distantProbe, sampler1_s, V, GBuffer.WorldNormals, GBuffer.Roughness).rgb;
	//float3 diffuseColor = GBuffer.Radiosity;
	float3 diffuseColor = diffuseBSDF(V, getDiffuseDominantDir(N, V, NdotV, GBuffer.Roughness), GBuffer);
	
	float3 specularColor = evaluateIBLSpecular(N, R, NdotV, GBuffer.Roughness, GBuffer.LinearRoughness, GBuffer.SpecularColor, fd90).rgb;
	//float3 specularColor = evaluateSpecularIBLReference(N, V, GBuffer.Roughness, GBuffer.SpecularColor, fd90).rgb;
	
	// @temp: temporary light probe intensity stored in g_exposureMultipliers.z
	float3 finalColor = epilogueLighting(((diffuseColor * GBuffer.Radiosity) + specularColor) * g_lightProbeIntensity, g_exposureMultipliers.x);// * g_exposureMultipliers.z;	
	finalColor = -min(-finalColor, 0);
	finalColor = min(float3(65504,65504,65504), finalColor);
	
	return float4(finalColor, 1.0f);
}
