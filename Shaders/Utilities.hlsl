
#include "Common.h"

// ------------------------------------------
// Fullscreen Quad
// ------------------------------------------

struct VS_IN
{
    uint VertexID : SV_VertexID;
};

struct VS_OUT
{
    float4 Position : SV_Position;
    float2 TexCoords : TEXCOORD0;
	float2 ScreenCoords : TEXCOORD1;
};

cbuffer CommonConstants : register(b0)
{
	float4x4 g_invViewProjMatrix;
	float4x4 g_invProjMatrix;
	float3 g_cameraPos;
	float g_renderMode;
	float2 g_invScreenSize;
	float2 g_invDeviceZToWorldZTransform;
	float4 g_exposureMultipliers;
	float4x3 g_normalBasisTangents[6];
}

VS_OUT VS_FullscreenQuad(VS_IN In)
{
    VS_OUT Out;

    if (In.VertexID == 0 || In.VertexID == 3)
    {
        Out.Position = float4(-1, 1, 0, 1);
        Out.TexCoords = float2(0, 0);
    }
    else if (In.VertexID == 1)
    {
        Out.Position = float4(1, 1, 0, 1);
        Out.TexCoords = float2(1, 0);
    }
    else if (In.VertexID == 2 || In.VertexID == 4)
    {
        Out.Position = float4(1, -1, 0, 1);
        Out.TexCoords = float2(1, 1);
    }
    else if (In.VertexID == 5)
    {
        Out.Position = float4(-1, -1, 0, 1);
        Out.TexCoords = float2(0, 1);
    }
	Out.ScreenCoords = Out.TexCoords / g_invScreenSize.xy;
    return Out;
}

struct PS_IN
{
    float4 Position : SV_Position;
    float2 TexCoord : TEXCOORD0;
	float2 ScreenCoords : TEXCOORD1;
};

// ------------------------------------------
// Color Lookup Table
// ------------------------------------------

cbuffer LookupTableConstants : register(b1)
{
	float lutSize;
	float flipY;
};

Texture2D<float4> g_resolveTexture : register(t0);
Texture2DArray<float4> g_lookupTableTexture : register(t1);

SamplerState sampler0_s : register(s0);
SamplerState sampler1_s : register(s1);

float4 PS_LookupTable(PS_IN In) : SV_Target
{
	float3 scale = (lutSize - 1.0f) / lutSize;
	float3 offset = 1.0f / (2.0f * lutSize);
	
	float3 lut = scale * g_resolveTexture.Sample(sampler0_s, In.TexCoord).rgb + offset;
	lut = float3(lut.x, (flipY > 0.5f) ? 1.0f - lut.y : lut.y, lut.z * lutSize);
		
	return float4(g_lookupTableTexture.Sample(sampler1_s, lut).rgb, 1.0f);
}

// ------------------------------------------
// Editor Composite
// ------------------------------------------

Texture2D<float4> g_editorCompositeTexture : register(t1);

float4 PS_EditorComposite(PS_IN In) : SV_Target
{
	float3 color = g_resolveTexture.Sample(sampler0_s, In.TexCoord).rgb;
	float4 composite = g_editorCompositeTexture.Sample(sampler0_s, In.TexCoord);
	
	float3 outColor = (composite.a >= 1) ? color.rgb : color.rgb * (1-composite.a) + composite.rgb;
	return float4(outColor, 0);
}

// ------------------------------------------
// Selection Outline
// ------------------------------------------

Texture2D<float> g_selectionDepth : register(t1);

float3 getWorldPosition(float2 texCoords, float depth)
{
	float4 worldPos;
	worldPos.x = texCoords.x * 2 - 1;
	worldPos.y = (1 - texCoords.y) * 2 - 1;
	worldPos.z = depth;
	worldPos.w = 1.0f;
	
	worldPos = mul(worldPos, g_invViewProjMatrix);
	worldPos.w += 0.00001f;
	worldPos.xyz /= worldPos.w;
	
	// @todo: no idea why i need the -1 scale on X, but if i dont, then lighting
	//        is all wrong
	
	return worldPos.xyz * float3(-1,1,1);
}

float getWorldDepth(float2 texCoords)
{
	float depth = g_selectionDepth.Sample(sampler0_s, texCoords);
	return length(getWorldPosition(texCoords, depth));
}

float4 PS_SelectionOutline(PS_IN In) : SV_Target
{
	float3 color = g_resolveTexture.Sample(sampler0_s, In.TexCoord).rgb;
	
	float sample1 = getWorldDepth(In.TexCoord + float2(g_invScreenSize.x, 0));
	float sample2 = getWorldDepth(In.TexCoord - float2(g_invScreenSize.x, 0));
	float sample3 = getWorldDepth(In.TexCoord + float2(0, g_invScreenSize.y));
	float sample4 = getWorldDepth(In.TexCoord - float2(0, g_invScreenSize.y));
	
	float x = floor(abs((sample1 - sample2) / 10000.0f));
	float y = floor(abs((sample3 - sample4) / 10000.0f));
	
	
	
	return float4(lerp(color, float3(1,1,0), clamp(x+y, 0, 1)), 1.0f);
}

// ------------------------------------------
// Debug Render Mode
// ------------------------------------------

Texture2D<float4> g_gbufferTexture0 : register(t0);
Texture2D<float4> g_gbufferTexture1 : register(t1);
Texture2D<float4> g_gbufferTexture2 : register(t2);
Texture2D<float4> g_gbufferTexture3 : register(t3);
Texture2D<float> g_depthTexture : register(t4);

float4 PS_DebugRenderMode(PS_IN In) : SV_Target
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
	float3 worldPosition = getWorldPosition(In.TexCoord, depth);
	
	if (depth >= 1)
		discard;
		
	if (g_renderMode <= 1.0f) // Wireframe
		return ((GBuffer.BaseColor.r + GBuffer.BaseColor.g + GBuffer.BaseColor.b + GBuffer.SpecularColor.r + GBuffer.SpecularColor.g + GBuffer.SpecularColor.b) > 0.0f) ? float4(1,1,1,1) : float4(0,0,0,0);
	else if (g_renderMode <= 2.0f) // BaseColor
		return float4(GBuffer.BaseColor, 1.0f);
	else if (g_renderMode <= 3.0f) // SpecularColor
		return float4(GBuffer.SpecularColor, 1.0f);
	else if (g_renderMode <= 4.0f) // Normals
		return float4(GBuffer.WorldNormals * 0.5f + 0.5f, 1.0f);
	else if (g_renderMode <= 5.0f) // MaterialAO
		return float4(GBuffer.MaterialAO, GBuffer.MaterialAO, GBuffer.MaterialAO, 1.0f);
	else if (g_renderMode <= 6.0f) // Smoothness
		return float4(GBuffer.Smoothness, GBuffer.Smoothness, GBuffer.Smoothness, 1.0f);
	else if (g_renderMode <= 7.0f) // Metallic
		return float4(GBuffer.Metallic, GBuffer.Metallic, GBuffer.Metallic, 1.0f);
	else if (g_renderMode <= 8.0f) // Reflectance
		return float4(GBuffer.Reflectance, GBuffer.Reflectance, GBuffer.Reflectance, 1.0f);
	//else if (g_renderMode <= 9.0f) // Reflections
	//	return float4(float3(0,0,0), 1.0f);
	else // Ambient
		return float4(GBuffer.Radiosity * g_exposureMultipliers.x, 1.0f);
	return float4(0,0,0,1);
}

// ------------------------------------------
// Resolve Depth to MSAA Depth
// ------------------------------------------

float PS_ResolveDepthToMsaa(PS_IN In) : SV_Depth
{
	return g_resolveTexture.Sample(sampler0_s, In.TexCoord).r;
}

// ------------------------------------------
// Resolve
// ------------------------------------------

float4 PS_Resolve(PS_IN In) : SV_Target
{
	float3 color = g_resolveTexture.Sample(sampler0_s, In.TexCoord).rgb;	
	return float4(color.rgb, 1.0f);
}

// ------------------------------------------
// Resolve World Normals
// ------------------------------------------

float4 PS_ResolveWorldNormals(PS_IN In) : SV_Target
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
	float3 worldPosition = getWorldPosition(In.TexCoord, depth);
	
	if (depth >= 1)
		discard;
		
	return float4((GBuffer.WorldNormals) * 0.5f + 0.5f, 1.0f);
}

// ------------------------------------------
// Resolve Cubemap Face
// ------------------------------------------

cbuffer CubeMapConstants : register(b0)
{
	int CubeMapFace;
}

TextureCube<float4> EnvTexture : register(t0);
SamplerState EnvSampler : register(s0);

float4 PS_ResolveCubeMapFace(PS_IN input) : SV_Target
{
	float2 tc = input.TexCoord * 2 - 1;
	
	float3 dir = float3(0,0,0);
	if (CubeMapFace == 0) dir = float3(1.0f, -tc.y, -tc.x);
	else if (CubeMapFace == 1) dir = float3(-1.0f, -tc.y, tc.x);
	else if (CubeMapFace == 2) dir = float3(tc.x, 1.0f, tc.y);
	else if (CubeMapFace == 3) dir = float3(tc.x, -1.0f, -tc.y);
	else if (CubeMapFace == 4) dir = float3(tc.x, -tc.y, 1.0f);
	else if (CubeMapFace == 5) dir = float3(-tc.x, -tc.y, -1.0f);
	return EnvTexture.Sample(EnvSampler, dir);
}