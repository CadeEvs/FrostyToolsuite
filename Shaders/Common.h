
#define PI 3.1415926535897932384626433832795
#define PI2 9.8696044010893586188344909998761

float2 PackNormals(float2 inNormals)
{
	return inNormals * 0.5f + 0.5f;
}

float2 UnpackNormals(float2 inNormals)
{
	return inNormals * 2 - 1;
}

float3 approximationSRGBToLinear(in float3 sRGBCol)
{
	return pow(sRGBCol, 2.2);
}

float3 approximationLinearToSRGB(in float3 linearCol)
{
	return pow(linearCol, 1 / 2.2);
}

float3 accurateSRGBToLinear(in float3 sRGBCol)
{
	float3 linearRGBLo = sRGBCol / 12.92;
	float3 linearRGBHi = pow((sRGBCol + 0.055) / 1.055, 2.4);
	float3 linearRGB = (sRGBCol <= 0.04045) ? linearRGBLo : linearRGBHi;
	return linearRGB;
}

float3 accurateLinearToSRGB(in float3 linearCol)
{
	float3 sRGBLo = linearCol * 12.92;
	float3 sRGBHi = (pow(abs(linearCol), 1.0/2.4) * 1.055) - 0.055;
	float3 sRGB = (linearCol <= 0.0031308) ? sRGBLo : sRGBHi;
	return sRGB;
}

float3 CalcWorldSpaceNormalsFromTangentSpace(float3 Normals, float3x3 TBN)
{
	return normalize(mul(TBN, normalize(Normals)));
}

float3 CalcWorldSpaceNormals(float3 Normals, float3 T, float4 B, float3 N)
{
#if PROFILE_20170321 == 1
	Normals = normalize(Normals * float3(-1,1,1));
#endif

	float3x3 TBN = float3x3(
		normalize(T), 
		normalize(B.xyz), 
		normalize(N)
		);
	TBN = transpose(TBN);
	return normalize(mul(TBN, Normals));
}

float3 DeriveNormalZ(float2 N)
{
	return float3(N.xy, sqrt(1-dot(N.xy, N.xy)));
}

float3 CalculateRadiosity(float4 SHProbeLight[9], float3 WorldNormal)
{
	float4 r0;
	float3 r11;
	
	r0.xw = WorldNormal.zy * WorldNormal.zy;
	r0.x = WorldNormal.x * WorldNormal.x + -r0.x;
	r11.xyz = r0.www * SHProbeLight[4].xyz;
	r11.xyz = float3(3,3,3) * r11.xyz;
	SHProbeLight[8].xyz = SHProbeLight[8].xyz * r0.xxx + r11.xyz;
	SHProbeLight[8].xyz = SHProbeLight[8].xyz + SHProbeLight[0].xyz;
	SHProbeLight[4].xyz = SHProbeLight[8].xyz + -SHProbeLight[4].xyz;
	SHProbeLight[5].xyz = SHProbeLight[5].xyz * WorldNormal.xxx;
	SHProbeLight[6].xyz = SHProbeLight[6].xyz * WorldNormal.yyy;
	SHProbeLight[6].xyz = SHProbeLight[6].xyz * WorldNormal.zzz;
	SHProbeLight[5].xyz = SHProbeLight[5].xyz * WorldNormal.yyy + SHProbeLight[6].xyz;
	SHProbeLight[7].xyz = SHProbeLight[7].xyz * WorldNormal.xxx;
	SHProbeLight[7].xyz = SHProbeLight[7].xyz * WorldNormal.zzz + SHProbeLight[5].xyz;
	SHProbeLight[4].xyz = SHProbeLight[7].xyz + SHProbeLight[4].xyz;
	SHProbeLight[2].xyz = SHProbeLight[2].xyz * WorldNormal.yyy;
	SHProbeLight[1].xyz = SHProbeLight[1].xyz * WorldNormal.xxx + SHProbeLight[2].xyz;
	SHProbeLight[1].xyz = SHProbeLight[3].xyz * WorldNormal.zzz + SHProbeLight[1].xyz;
	SHProbeLight[4].xyz = SHProbeLight[1].xyz + SHProbeLight[4].xyz;
	
	return max(float3(0,0,0), SHProbeLight[4].xyz);
}

float computeEV100FromAvgLuminance(float avgLuminance)
{
	return log2(avgLuminance * 100.0f / 12.5f);
}

float convertEV100toExposure(float EV100)
{
	float maxLuminance = 1.2f * pow(2.0f, EV100);
	return 1.0f / maxLuminance;
}

float3 logToLinear(float3 logColor)
{
	const float linearRange = 14;
	const float linearGrey = 0.18;
	const float exposureGrey = 444;
	
	float3 linearColor = exp2((logColor - exposureGrey / 1023.0) * linearRange) * linearGrey;
	return linearColor;
}

float3 linearToLog(float3 linearColor)
{
	const float linearRange = 14;
	const float linearGrey = 0.18;
	const float exposureGrey = 444;
	
	float3 logColor = log2(linearColor) / linearRange - log2(linearGrey) / linearRange + exposureGrey / 1023.0;
	return saturate(logColor);
}

float3 epilogueLighting(float3 color, float exposureMultiplier)
{
	return color * exposureMultiplier;
}

struct GBufferValues
{
	float3 BaseColor;
	float3 WorldNormals;
	float Reflectance;
	float MaterialAO;
	float Smoothness;
	float Metallic;
	float SpecularMult;
	float3 Radiosity;
	float3 Emissive;
	
	// only used with packedNormals
	float2 PackedNormals;
	
	// only used with unpack
	float3 SpecularColor;
	float Roughness;
	float LinearRoughness;
	float Mask;
};

void PackGBufferValues(GBufferValues values, inout float4 GBufferA, inout float4 GBufferB, inout float4 GBufferC, inout float4 GBufferD, in float4x3 normalBasisTransforms[6], in TextureCube<float4> normalBasisCubemap, in SamplerState normalBasisSampler, float exposureMultiplier)
{
	// pack those normals
	uint normalBasisId = (uint)(normalBasisCubemap.Sample(normalBasisSampler, values.WorldNormals).x * 255.5);
	values.PackedNormals = mul(normalBasisTransforms[normalBasisId], values.WorldNormals).xy;
	
	GBufferA = float4(PackNormals(values.PackedNormals), values.Smoothness, 0);
	GBufferB = float4(values.BaseColor, normalBasisId / 6.0f);
	GBufferC = float4(0, values.Metallic, values.Reflectance, values.MaterialAO);
	GBufferD = float4((values.Radiosity + values.Emissive) * exposureMultiplier, 1.0f);
}

GBufferValues UnpackGBufferValues(float4 GA, float4 GB, float4 GC, float4 GD, float4x3 normalBasisTransforms[6], float exposureMultiplier)
{
	GBufferValues GBuffer;
	
	GBuffer.BaseColor = GB.xyz;
	GBuffer.PackedNormals = UnpackNormals(GA.xy);
	GBuffer.Reflectance = GC.z;
	GBuffer.MaterialAO = GC.w;
	GBuffer.Smoothness = clamp(GA.z, 0.00001f, 0.99999f);
	GBuffer.Metallic = GC.y;
	//GBuffer.SpecularMult = GA.w;
	GBuffer.Radiosity = GD.xyz * exposureMultiplier;
	
	GBuffer.SpecularColor = (0.16f * pow(GBuffer.Reflectance, 2.0f)).rrr * (1-GBuffer.Metallic) + GBuffer.BaseColor * GBuffer.Metallic;
	GBuffer.BaseColor = (1-GBuffer.Metallic) * GBuffer.BaseColor;
	GBuffer.Roughness = pow(1-GBuffer.Smoothness, 2);
	GBuffer.LinearRoughness = 1-GBuffer.Smoothness;
	
	uint normalBasisId = (uint)(round(6 * GB.w));
	float3 n = normalize(DeriveNormalZ(GBuffer.PackedNormals));
	GBuffer.WorldNormals = mul(normalBasisTransforms[normalBasisId], n).xyz;
	
	return GBuffer;
}