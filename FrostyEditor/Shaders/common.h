
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
	//B.xyz = normalize(cross( N, T ) * -B.w);
	
	Normals = normalize(Normals);
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

GBufferValues UnpackGBufferValues(float4 GA, float4 GB, float4 GC, float4 GD, float4x3 normalBasisTransforms[6])
{
	GBufferValues GBuffer;
	
	GBuffer.BaseColor = GB.xyz;
	GBuffer.PackedNormals = UnpackNormals(GA.xy);
	GBuffer.Reflectance = GC.z;
	GBuffer.MaterialAO = GC.w;
	GBuffer.Smoothness = GA.z;
	GBuffer.Metallic = GC.y;
	//GBuffer.SpecularMult = GA.w;
	GBuffer.Radiosity = GD.xyz;
	
	GBuffer.SpecularColor = (0.16f * pow(GBuffer.Reflectance, 2.0f)).rrr * (1-GBuffer.Metallic) + GBuffer.BaseColor * GBuffer.Metallic;;
	GBuffer.Roughness = pow(1-GBuffer.Smoothness, 2);
	GBuffer.LinearRoughness = 1-GBuffer.Smoothness;
	
	uint normalBasisId = (uint)(round(6 * GB.w));
	float3 n = normalize(DeriveNormalZ(GBuffer.PackedNormals));
	GBuffer.WorldNormals = mul(normalBasisTransforms[normalBasisId], n).xyz;
	
	return GBuffer;
}