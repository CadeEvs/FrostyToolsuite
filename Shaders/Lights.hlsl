
#define PI (3.1415926535897932f)
#define INV_PI (0.31830988618379067543484808035204f)

#include "Common.h"
#include "BRDF.h"

struct PS_IN
{
    float4 Position : SV_Position;
    float2 TexCoords : TEXCOORD0;
	float2 ScreenCoords : TEXCOORD1;
};

Texture2D<float4> g_gbufferTexture0 : register(t0);
Texture2D<float4> g_gbufferTexture1 : register(t1);
Texture2D<float4> g_gbufferTexture2 : register(t2);
Texture2D<float4> g_gbufferTexture3 : register(t3);
Texture2D<float> g_depthTexture : register(t4);
Texture2D<float> g_shadowTexture : register(t5);

SamplerState sampler0_s : register(s0);

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

cbuffer LightConstants : register(b1)
{
	float4 g_lightPosAndInvSqrRadius;
	float4 g_lightColorAndIntensity;
}

float smoothDistanceAtt(float squaredDistance, float invSqtAttRadius)
{
	float factor = squaredDistance * invSqtAttRadius;
	float smoothFactor = saturate(1.0f - factor * factor);
	return smoothFactor * smoothFactor;
}

float getDistanceAtt(float3 unormalizedLightVector, float invSqtAttRadius)
{
	float sqrDist = dot(unormalizedLightVector, unormalizedLightVector);
	float attenuation = 1.0f / (max(sqrDist, 0.01*0.01));
	attenuation *= smoothDistanceAtt(sqrDist, invSqtAttRadius);
	return attenuation;
}

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

float3 specularBSDF(float3 V, float3 L, GBufferValues values)
{
	float3 N = values.WorldNormals;
	
	float  NoL = saturate(dot(N, L));
	float3 H = normalize(V + L);
	float  NoV = abs(dot(N, V)) + 1e-5f;
	float  NoH = saturate(dot(N, H));
	float  VoH = saturate(dot(V, H));
	float  LoH = saturate(dot(L, H));
	
	float energyBias = lerp(0, 0.5f,values.LinearRoughness);
	float energyFactor = lerp(1.0f, 1.0f/1.51f, values.LinearRoughness);
	float fd90 = energyBias + 2.0f * LoH*LoH * values.LinearRoughness;
	float3 f0 = values.SpecularColor;
	
	// specular BRDF
	float3 F = F_Schlick(f0, fd90, LoH);
	float  Vis = V_SmithGGXCorrelated(NoV, NoL, values.Roughness);
	float  D = D_GGX(NoH, values.Roughness);
	float3 Fr = D * F * Vis / PI;
	
	return Fr;
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

float3 BSDF(float3 V, float3 diffuseL, float3 specularL, GBufferValues values)
{
	return diffuseBSDF(V, diffuseL, values) + specularBSDF(V, specularL, values);
}

float4 PS_SunLight(PS_IN In) : SV_Target
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
		
	float3 N = GBuffer.WorldNormals;
	float3 V = normalize(g_cameraPos - worldPosition);
	float3 D = normalize(g_lightPosAndInvSqrRadius.xyz);
	float3 R = 2 * dot(V, N) * N - V;
	
	float r = sin(g_lightPosAndInvSqrRadius.w);
	float d = cos(g_lightPosAndInvSqrRadius.w);
	
	float DdotR = dot(D, R);
	float3 S = R - DdotR * D;
	float3 L = DdotR < d ? normalize(d * D + normalize(S) * r) : R;
	
	float illuminance = g_lightColorAndIntensity.w * saturate(dot(N,D));
	float shadow = g_shadowTexture.Sample(sampler0_s, In.TexCoords).x;
	
	float3 finalColor = epilogueLighting(BSDF(V,D,L, GBuffer) * illuminance * shadow, g_exposureMultipliers.x);
	finalColor = -min(-finalColor, 0);
	finalColor = min(float3(65504,65504,65504), finalColor);
	
	return float4(finalColor, 1.0f);
}

float4 PS_Point(PS_IN In) : SV_Target
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
		
	float3 lightPos = g_lightPosAndInvSqrRadius.xyz;
	float lightInvSqtAttRadius = g_lightPosAndInvSqrRadius.w;
	float3 lightColor = g_lightColorAndIntensity.xyz * g_lightColorAndIntensity.w / (4 * PI);
	
	float3 unormalizedLightVector = lightPos - worldPosition;
	
	float3 V = normalize((g_cameraPos) - worldPosition);
	float3 L = normalize(unormalizedLightVector);
	float3 N = GBuffer.WorldNormals;
	
	float att = 1;
	att *= getDistanceAtt(unormalizedLightVector, lightInvSqtAttRadius);
	
	if (g_renderMode == 4.0)
		return float4(GBuffer.WorldNormals * 0.5 + 0.5, 1.0f);
		
	float3 finalColor = epilogueLighting(BSDF(V, L, L, GBuffer) * saturate(dot(N, L)) * lightColor * att, g_exposureMultipliers.x);
	finalColor = -min(-finalColor, 0);
	finalColor = min(float3(65504,65504,65504), finalColor);
	
	return float4(finalColor, 1.0f);
}

float illuminanceSphereOrDisk(float cosTheta, float sinSigmaSqr)
{
	float sinTheta = sqrt(1.0f - cosTheta * cosTheta);
	float illuminance = 0.0f;
	
	if (cosTheta * cosTheta > sinSigmaSqr)
	{
		illuminance = PI * sinSigmaSqr * saturate(cosTheta);
	}
	else
	{
		float x = sqrt(1.0f / sinSigmaSqr - 1.0f);
		float y = -x * (cosTheta / sinTheta);
		float sinThetaSqrtY = sinTheta * sqrt(1.0f - y * y);
		illuminance = (cosTheta * acos(y) - x * sinThetaSqrtY) * sinSigmaSqr + atan(sinThetaSqrtY / x);
	}
	
	return max(illuminance, 0.0f);
}

float4 PS_Sphere(PS_IN In) : SV_Target
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
		
	float3 lightPos = g_lightPosAndInvSqrRadius.xyz;
	float lightInvSqtAttRadius = g_lightPosAndInvSqrRadius.w;	
	float3 unormalizedLightVector = lightPos - worldPosition;
	
	float3 V = normalize(g_cameraPos - worldPosition);
	float3 L = normalize(unormalizedLightVector);
	float3 N = GBuffer.WorldNormals;
	
	float sqrDist = dot(unormalizedLightVector, unormalizedLightVector);
	float cosTheta = clamp(dot(N, L), -0.999, 0.999);
	float sqrLightRadius = g_lightPosAndInvSqrRadius.w * g_lightPosAndInvSqrRadius.w;
	float sinSigmaSqr = min(sqrLightRadius / sqrDist, 0.9999f);
	
	float illuminance = PI * sqrLightRadius * saturate(cosTheta) / sqrDist;
	if (sqrDist < 100.0f * sqrLightRadius)
	{
		illuminance = illuminanceSphereOrDisk(cosTheta, sinSigmaSqr);
	}
	
	float3 lightColor = g_lightColorAndIntensity.xyz * g_lightColorAndIntensity.w * illuminance;
	
	if (g_renderMode == 4.0)
		return float4(GBuffer.WorldNormals * 0.5 + 0.5, 1.0f);
		
	float3 finalColor = epilogueLighting(BSDF(V, L, L, GBuffer) * saturate(dot(N, L)) * lightColor, g_exposureMultipliers.x);
	finalColor = -min(-finalColor, 0);
	finalColor = min(float3(65504,65504,65504), finalColor);
	
	return float4(finalColor, 1.0f);
}
