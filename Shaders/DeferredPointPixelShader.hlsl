#pragma pack_matrix(row_major)

#define PI (3.1415926535897932f)

struct PS_IN
{
    float4 Position : SV_Position;
    float2 TexCoord : TEXCOORD;
};

Texture2D<float> DepthBuffer : register(t0);
Texture2D<float4> DiffuseBuffer : register(t1);
Texture2D<float4> NormalBuffer : register(t2);
Texture2D<float4> MaterialInfoBuffer : register(t3);

SamplerState GBufferSampler
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

cbuffer CameraConstants : register(b0)
{
    float4x4 InvProjection;
    float4x4 InvView;
    float3 ViewDirection;
    float nearClip;
    float farClip;
    float3 cameraPos;
}

cbuffer PointConstants : register(b1)
{
    float3 LightPosition;
    float LightRadius;
    float3 LightColor;
    float LightFalloffExponent;
	float3 Padding;
	float DebugMode;
}

//#define nearClip 0.03f
//#define farClip 1000.0f

// NOTE: Don't worry about the extra computations that never get used, those will be removed by the aggressively optimizing compiler.

float linearDepth(float depthSample)
{
    float A = farClip / (farClip - nearClip);
    float B = (-farClip * nearClip) / (farClip - nearClip);
    return B / (depthSample - A);
}

float3 PositionFromDepth2(float2 uv, float depth)
{
    // First, unproject the ray to the back clip plane
    float4 rayToNearClip = mul(float4(uv.x * 2 - 1, (1 - uv.y) * 2 - 1, 1.0, 1.0), InvProjection);
    // Then, divide by .z to obtain the point on the ray where z = 1
    rayToNearClip.xyz /= rayToNearClip.z;
    // Multiply by the z value from the g-buffer shader to get the view-space position
    float4 viewpos = float4(rayToNearClip.xyz * depth, 1.0f);
    viewpos = mul(viewpos, InvView);
    return viewpos.xyz / viewpos.w;
}

float RadialAttenuation(float3 WorldLightVector, half FalloffExponent)
{
	float NormalizeDistanceSquared = dot(WorldLightVector, WorldLightVector);
	return pow(1.0f - saturate(NormalizeDistanceSquared), FalloffExponent);
}

float Square(float x)
{
	return x*x;
}

float Pow5(float x)
{
	float x2 = x*x;
	return x2 * x2 * x;
}

float3 Diffuse_OrenNayar(float3 DiffuseColor, float Roughness, float NoV, float NoL, float VoH)
{
	float a = Roughness * Roughness;
	float s = a;
	float s2 = s * s;
	float VoL = 2 * VoH * VoH - 1;
	float Cosri = VoL - NoV * NoL;
	float C1 = 1 - 0.5 * s2 / (s2 + 0.33);
	float C2 = 0.45 * s2 / (s2 + 0.09) * Cosri * (Cosri >= 0 ? rcp(max(NoL, NoV)) : 1);
	return DiffuseColor / PI * (C1 + C2) * (1 + Roughness * 0.5);
}

float D_GGX(float Roughness, float NoH)
{
	float a = Roughness * Roughness;
	float a2 = a * a;
	float d = (NoH * a2 - NoH) * NoH + 1;
	return a2 / (PI*d*d);
}

float Vis_SmithJointApprox(float Roughness, float NoV, float NoL)
{
	float a = Square(Roughness);
	float Vis_SmithV = NoL * (NoV * (1 - a) + a);
	float Vis_SmithL = NoV * (NoL * (1 - a) + a);
	return 0.5 * rcp(Vis_SmithV + Vis_SmithL);
}

float3 F_Schlick(float3 SpecularColor, float VoH)
{
	float Fc = Pow5(1 - VoH);
	return saturate(50.0 * SpecularColor.g) * Fc + (1 - Fc) * SpecularColor;
}

float4 main(PS_IN input) : SV_Target
{
    float DepthBufferValue = DepthBuffer.Sample(GBufferSampler, input.TexCoord);
    float4 DiffuseBufferValue = DiffuseBuffer.Sample(GBufferSampler, input.TexCoord);
    float4 NormalBufferValue = NormalBuffer.Sample(GBufferSampler, input.TexCoord);
    float4 MaterialInfoBufferValue = MaterialInfoBuffer.Sample(GBufferSampler, input.TexCoord);

    float Depth = linearDepth(DepthBufferValue);
	if(MaterialInfoBufferValue.a < 1.0f)
		discard;
		
    float3 Position = PositionFromDepth2(input.TexCoord.xy, Depth);
    float3 Diffuse = (DebugMode > 0.0f) ? float3(1,1,1) : DiffuseBufferValue.xyz;
    float3 Normal = normalize(NormalBufferValue.xyz * 2 - 1);
    
	//float3 Emissive = float3(DiffuseBufferValue.w, NormalBufferValue.w, MaterialInfoBufferValue.z);
	
	float SpecularMult = DiffuseBufferValue.w;
	float Smoothness = MaterialInfoBufferValue.y;
	float MaterialAO = (DebugMode > 0.0f) ? 1.0f : MaterialInfoBufferValue.x;
	float Metallic = MaterialInfoBufferValue.z;
	
	float3 SpecularColor = float3(0.04f, 0.04f, 0.04f);
	SpecularColor = lerp(SpecularColor, Diffuse, Metallic) * SpecularMult;
	
	Diffuse = (Metallic > 0.0f) ? Diffuse - (Diffuse * Metallic) : Diffuse;
	
    float4 campos = float4(0, 0, 0, 1); // the origin in view space
    campos = mul(campos, InvView);
    campos /= campos.w;

	float3 lightPosition = LightPosition.xyz;
	float  invLightRadius = 1.0f / LightRadius;
	
	float3 toLight = LightPosition - Position.xyz;
	float distanceSqr = dot(toLight, toLight);
	float distanceAttenuation = 1.0f;
	
	float3 L = normalize(toLight * rsqrt(distanceSqr));
	float3 V = normalize(campos - Position.xyz);
	float3 N = Normal;
	
	float lightRadiusMask = 1.0f;//RadialAttenuation(toLight * invLightRadius, LightFalloffExponent);
	if (lightRadiusMask > 0.0f)
	{
		float  NoL = saturate(dot(N, L));
		float3 H = normalize(V + L);
		float  NoV = max(dot(N, V), 1e-5);
		float  NoH = saturate(dot(N, H));
		float  VoH = saturate(dot(V, H));
			
		float actualRoughness = clamp(1.0f - Smoothness, 0.05f, 1.0f);
		
		float3 colDiffuse = Diffuse_OrenNayar(Diffuse, actualRoughness, NoV, NoL, VoH);	
		float D = D_GGX(actualRoughness, NoH);
		float Vis = Vis_SmithJointApprox(actualRoughness, NoV, NoL);
		float3 F = F_Schlick(SpecularColor, VoH);

		float3 color = float3(((colDiffuse) + (D * Vis) * F) * LightColor * NoL);//(LightColor * (NoL * (lightRadiusMask))));
		return float4(color, 1.0f);
	}
	return float4(0,0,0,1);
}