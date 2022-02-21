
#include "Common.h"

#if PROFILE_20190729 == 1
#define PROFILE_20180807 1
#endif

// ------------------------------
// Vertex Shader
// ------------------------------

struct VS_IN
{
    float3 Position : POSITION;
    float4 Normal : NORMAL;
    float4 Tangent : TANGENT;
    float4 Bitangent : BINORMAL;
    float2 TexCoord0 : TEXCOORD0;
    float2 TexCoord1 : TEXCOORD1;
    float2 TexCoord2 : TEXCOORD2;
    float4 Color0 : COLOR0;
    float4 Color1 : COLOR1;
    uint4 BoneIndices : BLENDINDICES0;
    uint4 BoneIndices2 : BLENDINDICES1;
    float4 BoneWeights : BLENDWEIGHT0;
    float4 BoneWeights2 : BLENDWEIGHT1;
	uint TangentSpace : TANGENTSPACE;
};

struct VS_OUT
{
    float4 Position : SV_Position;
    float3 Normal : NORMAL;
    float3 Tangent : TANGENT;
    float4 Bitangent : BINORMAL;
    float2 TexCoord0 : TEXCOORD0;
    float2 TexCoord1 : TEXCOORD1;
    float2 TexCoord2 : TEXCOORD2;
    float4 Color0 : COLOR0;
    float4 Color1 : COLOR1;
	float3x3 TBN : TANGENTBASIS;
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

Buffer<float4> bonePartMatrices : register(t0);

float3x4 getBoneMatrix(int Index)
{
	float boneCount = bonePartMatrices[0].x;
	if (Index & 0x8000)
		Index = (Index & 0x7FFF) + boneCount;
		
	float4 A = bonePartMatrices[Index * 3 + 1];
	float4 B = bonePartMatrices[Index * 3 + 2];
	float4 C = bonePartMatrices[Index * 3 + 3];
	
	return float3x4(A, B, C);
}

void applyBoneMatrix(float3 value, int boneIndex, float boneWeight, inout float3 outValue)
{
	float3x4 boneMatrix = getBoneMatrix(boneIndex);
	outValue += mul(boneMatrix, float4(value, 1.0f)).xyz * boneWeight;
}

void applyBoneRotation(float3 value, int boneIndex, float boneWeight, inout float3 outValue)
{
	float3x4 boneMatrix = getBoneMatrix(boneIndex);
	outValue += mul((float3x3)boneMatrix, value).xyz * boneWeight;
}

#define SHADER_BONES_PER_VERTEX 8
float4 TransformPosition(VS_IN Input, float3 Position)
{
	uint BoneIndices[SHADER_BONES_PER_VERTEX];
	float BoneWeights[SHADER_BONES_PER_VERTEX];

	BoneIndices[0] = Input.BoneIndices.x;
	BoneIndices[1] = Input.BoneIndices.y;
	BoneIndices[2] = Input.BoneIndices.z;
	BoneIndices[3] = Input.BoneIndices.w;
	BoneWeights[0] = Input.BoneWeights.x;
	BoneWeights[1] = Input.BoneWeights.y;
	BoneWeights[2] = Input.BoneWeights.z;
	BoneWeights[3] = Input.BoneWeights.w;

	#if SHADER_BONES_PER_VERTEX > 4
	BoneIndices[4] = Input.BoneIndices2.x;
	BoneIndices[5] = Input.BoneIndices2.y;
	BoneIndices[6] = Input.BoneIndices2.z;
	BoneIndices[7] = Input.BoneIndices2.w;
	BoneWeights[4] = Input.BoneWeights2.x;
	BoneWeights[5] = Input.BoneWeights2.y;
	BoneWeights[6] = Input.BoneWeights2.z;
	BoneWeights[7] = Input.BoneWeights2.w;
	#endif

	float3 skinnedPosition = float3(0,0,0);
	for (int i = 0; i < SHADER_BONES_PER_VERTEX; i++)
		applyBoneMatrix(Position, BoneIndices[i], BoneWeights[i], skinnedPosition);
	
	float3 worldPosition = mul(float4(skinnedPosition, 1.0f), worldMatrix).xyz - cameraPos;
	return mul(float4(worldPosition, 1.0f), crViewProjMatrix);
}
float3 TransformTBN(VS_IN Input, float3 Value)
{
	uint BoneIndices[SHADER_BONES_PER_VERTEX];
	float BoneWeights[SHADER_BONES_PER_VERTEX];

	BoneIndices[0] = Input.BoneIndices.x;
	BoneIndices[1] = Input.BoneIndices.y;
	BoneIndices[2] = Input.BoneIndices.z;
	BoneIndices[3] = Input.BoneIndices.w;
	BoneWeights[0] = Input.BoneWeights.x;
	BoneWeights[1] = Input.BoneWeights.y;
	BoneWeights[2] = Input.BoneWeights.z;
	BoneWeights[3] = Input.BoneWeights.w;

	#if SHADER_BONES_PER_VERTEX > 4
	BoneIndices[4] = Input.BoneIndices2.x;
	BoneIndices[5] = Input.BoneIndices2.y;
	BoneIndices[6] = Input.BoneIndices2.z;
	BoneIndices[7] = Input.BoneIndices2.w;
	BoneWeights[4] = Input.BoneWeights2.x;
	BoneWeights[5] = Input.BoneWeights2.y;
	BoneWeights[6] = Input.BoneWeights2.z;
	BoneWeights[7] = Input.BoneWeights2.w;
	#endif

	float3 skinnedValue = float3(0,0,0);
	for (int i = 0; i < SHADER_BONES_PER_VERTEX; i++)
		applyBoneRotation(Value, BoneIndices[i], BoneWeights[i], skinnedValue);
	
	return mul((float3x3)worldMatrix, normalize(skinnedValue));
}

float3x3 CalcTangentSpaceNormal_AxisAngle(float4 ts, float3x3 world)
{
	float4 r0, r1, r2, r3, r4, r7;
	uint4 u0;
	
	r0.xyzw = ts.wxyz * float4(255, 0.707107, 0.707107, 1.5708) + float4(0.25, -0.707107, -0.707107, 0);
	u0.x = (uint)r0.x;
	r1.xyzw = (float4)(u0.xxxx & uint4(16, 32, 192, 1));
	r2.yzw = r1.xyz * float3(0.0441942, 0.0220971, 0.0245437) + r0.yzw;
	r0.y = dot(r2.yz, r2.yz);
	r0.y = -r0.y + 1;
	r0.y = sqrt(abs(r0.y));
	r2.x = (r1.w) ? -r0.y : r0.y;
	u0.xyz = u0.xxx & uint3(4,2,8);
	r1.xyz = (u0.x) ? r2.xyz : r2.yxz;
	r1.xyz = (u0.y) ? r1.xyz : r1.yzx;
	r2.xy = r1.yx * float2(-1, 1);
	r2.z = 0;
	r3.xz = r1.zx * float2(1, -1);
	r3.y = 0;
	r0.xyw = (u0.y) ? r2.xyz : r3.xyz; 

	r0.xyw = normalize(r0.xyw);
	r2.xyz = r0.ywx * r1.zxy;
	r2.xyz = r1.yzx * r0.wxy - r2.xyz;
	r3.x = sin(abs(r2.w));
	r4.x = cos(abs(r2.w));
	
	r2.xyz = r2.xyz * r3.xxx;
	r0.xyw = r0.xyw * r4.xxx + r2.xyz;
	r2.xyz = r0.ywx * r1.zxy;
	r2.xyz = r1.yzx * r0.wxy - r2.xyz;
	r2.xyz = (u0.z) ? -r2.xyz : r2.xyz;
	
	r7.xyz = normalize(mul(r1.xyz, world));
	r1.xyz = normalize(mul(r2.xyz, world));
	r2.xyz = normalize(mul(-r0.xyw, world));
	r0.xyz = r7.xyz;
	
	float3x3 TBN = (float3x3)0;
	
	TBN[0].x = r2.x;
	TBN[0].y = r1.x;
	TBN[0].z = r0.x;
	
	TBN[1].x = r2.y;
	TBN[1].y = r1.y;
	TBN[1].z = r0.y;
	
	TBN[2].x = r2.z;
	TBN[2].y = r1.z;
	TBN[2].z = r0.z;
	
	return TBN;
}

float3x3 CalcTangentSpaceNormal(uint ts, float3x3 world)
{
	float3x3 TBN = (float3x3)0;
	float4 r0, r1, r2, r3, r4, r5, r6, r7;
	
	float4 packedQuaternion = float4(
		((uint)(ts >> 22) & 0x3FF) * (1/1023.0f),
		((uint)(ts >> 13) & 0x1FF) * (1/511.0f),
		((uint)(ts >> 3) & 0x3FF) * (1/1023.0f),
		((uint)(ts >> 1) & 0x03) * 3.0f
	);
	
	uint index = (uint)(ts >> 1) & 0x03;
	
	float4 quaternion;
	quaternion.xyz = packedQuaternion.xyz * sqrt(2.0f) - (1.0f / sqrt(2.0f));
	quaternion.w = sqrt(1.0f - saturate(dot(quaternion.xyz, quaternion.xyz)));
	
	if (index == 0) quaternion = quaternion.wxyz;
	if (index == 1) quaternion = quaternion.xwyz;
	if (index == 2) quaternion = quaternion.xywz;
	
	// ---------------
	// normal
	// ---------------
 
	r7.x = 1 - (((quaternion.y * quaternion.y) * 2.0f) + ((2.0f * quaternion.z) * quaternion.z));
	r7.y = quaternion.y * (2 * quaternion.x) + ((2 * quaternion.w) * quaternion.z);
	r7.z = quaternion.z * (2 * quaternion.x) - ((2 * quaternion.w) * quaternion.y);
	
	// ---------------
	// tangent
	// ---------------
	
	r6.x = quaternion.y * (2 * quaternion.z) + ((2 * quaternion.w) * quaternion.x);
	r6.y = 1 - (quaternion.x * (2 * quaternion.x) + ((2 * quaternion.z) * quaternion.z));
	r6.z = quaternion.y * (2 * quaternion.x) - ((2 * quaternion.w) * quaternion.z);

	// ---------------
	// binormal
	// ---------------
	
	r0.w = (int)ts & 1;
	r3.xyz = cross(r7.xyz, r6.xyz) * (r0.w) ? -1 : 1;

	r0.xyz = normalize(mul(r7.xyz, world));
	r2.xyz = normalize(mul(r6.xyz, world));
	r1.xyz = normalize(mul(r3.xyz, world));
	
	TBN[0].x = r2.x;
	TBN[0].y = r1.x;
	TBN[0].z = r0.x;
	
	TBN[1].x = r2.y;
	TBN[1].y = r1.y;
	TBN[1].z = r0.y;
	
	TBN[2].x = r2.z;
	TBN[2].y = r1.z;
	TBN[2].z = r0.z;
	
	return TBN;
}

float3x3 CalcTangentSpaceNormal_Backup(uint ts, float3x3 world)
{
	float3x3 TBN = (float3x3)0;
	float4 r0, r1, r2, r3, r4, r5, r6, r7;
	
	r0.x = (uint)ts >> 22;
	r0.x = (uint)r0.x;
	r0.x = r0.x * 0.00138241798 + -0.707106769;
	if (2 == 0) r3.x = 0; else if (2+1 < 32) {   r3.x = (uint)ts << (32-(2 + 1)); r3.x = (uint)r3.x >> (32-2);  } else r3.x = (uint)ts >> 1;
	if (9 == 0) r3.y = 0; else if (9+13 < 32) {   r3.y = (uint)ts << (32-(9 + 13)); r3.y = (uint)r3.y >> (32-9);  } else r3.y = (uint)ts >> 13;
	if (10 == 0) r3.z = 0; else if (10+3 < 32) {   r3.z = (uint)ts << (32-(10 + 3)); r3.z = (uint)r3.z >> (32-10);  } else r3.z = (uint)ts >> 3;
	r3.yz = (uint2)r3.zy;
	r0.zw = r3.yz * float2(0.00138241798,0.00276754121) + float2(-0.707106769,-0.707106769);
	r1.w = dot(r0.xzw, r0.xzw);
	r1.w = min(1, r1.w);
	r1.w = 1 + -r1.w;
	r0.y = sqrt(r1.w);
	r0.xyzw = r3.xxxx ? r0.xyzw : r0.yzwx;
	r0.xyzw = (r3.x == 1) ? r0.xyzw : r0.xwyz;
	r0.xyzw = (r3.x == 2) ? r0.xyzw : r0.xywz;
	r3.yz = r0.yz;
	r3.x = 2;
	r5.xyzw = r3.xyxx * r0.xyzw;
	r3.xw = r5.yz * r3.xz;
	r0.w = r3.x + r3.w;
	r1.w = r0.x * r5.x + r3.w;
	r6.y = 1 + -r1.w;
	r7.w = 1 + -r0.w;
	r0.xyz = r5.www * r0.xyz;
	r7.x = r3.y * r5.x + r0.z;
	r7.yz = r3.zy * r5.xx + -r0.yz;
	r6.z = r3.y * r5.z + r0.x;
	
	r6.x = r7.z;
	r3.xyz = r7.ywx * r6.yzx;
	r3.xyz = r7.xyw * r6.zxy + -r3.xyz;
	r5.xyz = r7.xyw * r6.zxy;
	r5.xyz = r7.ywx * r6.yzx + -r5.xyz;
	r0.w = (int)ts & 1;
	r3.xyz = r0.www ? r3.xyz : r5.xyz;
	
	r0.xyz = normalize(mul(r7.wxy, world));
	r2.xyz = normalize(mul(-r6.xyz, world));
	r1.xyz = normalize(mul(r3.xyz, world));
	
	TBN[0].x = r2.x;
	TBN[0].y = r1.x;
	TBN[0].z = r0.x;
	TBN[1].x = r2.y;
	TBN[1].y = r1.y;
	TBN[1].z = r0.y;
	TBN[2].x = r2.z;
	TBN[2].y = r1.z;
	TBN[2].z = r0.z;
	
	//TBN[1] = normalize(TBN[1]);
	//TBN[0] = normalize(TBN[0]);
	
	return TBN;
}

VS_OUT VS_MeshFallback(VS_IN In)
{
    VS_OUT Out = (VS_OUT)0;
		
    Out.Position = TransformPosition(In, In.Position.xyz);
    Out.Normal = TransformTBN(In, In.Normal.xyz); 
#if PROFILE_20170321 == 1
	Out.Tangent = TransformTBN(In, In.Tangent.xyz * float3(-1,1,1));
    Out.Bitangent.xyz = TransformTBN(In, In.Bitangent.xyz * float3(-1,1,1));
#else
    Out.Tangent = TransformTBN(In, In.Tangent.xyz);
    Out.Bitangent.xyz = TransformTBN(In, In.Bitangent.xyz);
#endif
	Out.Bitangent.w = In.Bitangent.w;
	
    Out.TexCoord0 = In.TexCoord0;
    Out.TexCoord1 = In.TexCoord1;
    Out.TexCoord2 = In.TexCoord2;
    Out.Color0 = In.Color0;
    Out.Color1 = In.Color1;
	
#if OPTIMIZE == 1
#else
	#if PROFILE_20191101==1 || PROFILE_20190729==1
		Out.TBN = CalcTangentSpaceNormal_AxisAngle(In.Bitangent, (float3x3)worldMatrix);
	#else
		Out.TBN = CalcTangentSpaceNormal(In.TangentSpace, (float3x3)worldMatrix);
	#endif
#endif
	
    return Out;
}

// ------------------------------
// Pixel Shader
// ------------------------------

struct PS_IN
{
    float4 Position : SV_Position;
    float3 Normal : NORMAL;
    float3 Tangent : TANGENT;
    float4 Bitangent : BINORMAL;
    float2 TexCoord0 : TEXCOORD0;
    float2 TexCoord1 : TEXCOORD1;
    float2 TexCoord2 : TEXCOORD2;
    float4 Color0 : COLOR0;
    float4 Color1 : COLOR1;
	float3x3 TBN : TANGENTBASIS;
};

struct PS_OUT
{
    float4 GBufferA : SV_Target0; //           BaseColor+SpecularMult
    float4 GBufferB : SV_Target1; //           Normals+Reflectance
    float4 GBufferC : SV_Target2; //           MaterialAO+Smoothness+Metallic+Mask
	float4 GBufferD : SV_Target3; //           Radiosity+Emissive
    float Depth : SV_Target4; //               Depth
};

cbuffer MaterialConstants : register(b2)
{
#if PROFILE_20170321 == 1
	float MetallicChannel;
	float TwoChannelNormals;
	float Padding;
#endif
#if PROFILE_20160607 == 1
	float Usestexture_RSM;
	float Usestexture_Ex;
	float Padding;
#endif
#if PROFILE_20151117 == 1
	float Hastexture_MSR;
	float TwoChannelNormals;
	float MultiUV;
#endif
#if PROFILE_20141118 == 1
	float IsBodyMat;
	float2 Padding;
#endif
#if PROFILE_20160927 == 1
	float HasSpecmask;
	float2 Padding;
#endif
#if PROFILE_20161021 == 1
	float3 Padding;
#endif
#if PROFILE_20171117 == 1
	float MetallicChannel; // 0 = None, 1 = Norm.b, 2 = Norm.a
	float AOChannel; // 0 = None, 1 = AOSlice.r, 2 = Norm.b, 3 = Norm.a
	float MultiUV;
#endif
#if PROFILE_20170929 == 1
	float HasSpecmask;
	float2 Padding;
#endif
#if PROFILE_20180807 == 1
	float MetallicChannel; // 0 = None, 1 = RSM.b, 2 = NSM.a (NSM.b = Smoothness)
	float UseStdNormals;
	float MultiUV;
#endif
#if PROFILE_20151103 == 1
	float3 Padding;
#endif
#if PROFILE_20150223 == 1
	float3 Padding;
#endif
#if PROFILE_20140225 == 1
	float3 Padding;
#endif
#if PROFILE_20171110 == 1
	float3 Padding;
#endif
#if PROFILE_20180628 == 1
	float SmoothnessChannel;
	float2 Padding;
#endif
#if PROFILE_20181207 == 1
	float3 Padding;
#endif
#if PROFILE_20131115 == 1
	float3 Padding;
#endif
#if PROFILE_20191101 == 1
	float HasSpecmask;
	float StdNormals;
	float Padding;
#endif
#if PROFILE_20190905 == 1
	float3 Padding;
#endif
    float sRGB = 1.0f;
	float4 OverlayColor;
	float4 TintColorA;
	float4 TintColorB;
	float4 TintColorC;
	float4 TintColorD;
#if PROFILE_20171117
	float4 Detail_Tiling;
	float4 NormalDetail_Intensity;
	float4 SmoothnessDetail_Intensity;
	float2 SmoothnessChannel;
#endif
#if PROFILE_20170321
	float3 MP_Light;
	float Emissive_Intensity;
#endif
};

TextureCube<float4> texture_normalBasisCubemapTexture : register(t0);
Texture2D<float4> texture_Diffuse : register(t1);

#if PROFILE_20170321==1
Texture2D<float4> texture_Normal : register(t2);
Texture2D<float4> texture_Mask : register(t3);
Texture2D<float4> texture_Tint : register(t4);
#endif

#if PROFILE_20160607==1
Texture2D<float4> texture_NRS : register(t2);
Texture2D<float4> texture_RSM : register(t3);
Texture2D<float4> texture_Ex : register(t4);
#endif

#if PROFILE_20151117==1
Texture2D<float4> texture_Normal : register(t2);
Texture2D<float4> texture_MSR : register(t3);
#endif

#if PROFILE_20141118==1
Texture2D<float4> texture_Normal : register(t2);
Texture2D<float4> texture_Specular : register(t3);
Texture2D<float4> texture_Tint : register(t4);
#endif

#if PROFILE_20160927==1 || PROFILE_20191101==1
Texture2D<float4> texture_Normal : register(t2);
Texture2D<float4> texture_Coeff : register(t3);
Texture2D<float4> texture_SpecMask : register(t4);
#endif

#if PROFILE_20161021==1
Texture2D<float4> texture_Normal : register(t2);
#endif

#if PROFILE_20171117==1
Texture2D<float4> texture_Normal : register(t2);
Texture2D<float4> texture_AOSlice : register(t3);
// t4 is not used
Texture2D<float4> texture_Markings : register(t5);
Texture2D<float4> texture_Markings2 : register(t6);
Texture2D<float4> texture_MarkingsCamo : register(t7);
Texture2DArray<float4> texture_NormalDetailTextureArray : register(t8);
Texture2D<float4> texture_RSSSAO : register(t9);
#endif

#if PROFILE_20170929==1
Texture2D<float4> texture_Normal : register(t2);
Texture2D<float4> texture_Coeff : register(t3);
Texture2D<float4> texture_SpecMask : register(t4);
#endif

#if PROFILE_20180807==1
Texture2D<float4> texture_Normal : register(t2);
Texture2D<float4> texture_RSM : register(t3);
Texture2D<float4> texture_SpecMask : register(t4);
#endif

#if PROFILE_20151103==1
Texture2D<float4> texture_Normal : register(t2);
Texture2D<float4> texture_Other : register(t3);
#endif

#if PROFILE_20150223==1
Texture2D<float4> texture_Normal : register(t2);
Texture2D<float4> texture_ASM : register(t3);
#endif

#if PROFILE_20140225==1
Texture2D<float4> texture_Normal : register(t2);
Texture2D<float4> texture_PSA : register(t3);
#endif

#if PROFILE_20171110 == 1
Texture2D<float4> texture_Normal : register(t2);
#endif

#if PROFILE_20180628 == 1
Texture2D<float4> texture_Normal : register(t2);
#endif

#if PROFILE_20181207 == 1
Texture2D<float4> texture_Normal : register(t2);
Texture2D<float4> texture_AMMS : register(t3);
#endif

#if PROFILE_20131115==1
Texture2D<float4> texture_Normal : register(t2);
#endif

#if PROFILE_20190905 == 1
Texture2D<float4> texture_Normal : register(t2);
#endif

SamplerState sampler0_s : register(s0);
SamplerState sampler1_s : register(s1);

PS_OUT PS_MeshFallback(PS_IN In)
{
    PS_OUT Out = (PS_OUT)0;
    GBufferValues GBuffer = (GBufferValues)0;
	
    float4 Diffuse = texture_Diffuse.Sample(sampler1_s, In.TexCoord0);
	
// MEA
#if PROFILE_20170321==1
	float3 Norm = texture_Normal.Sample(sampler1_s, In.TexCoord0).xyz * 2 - 1;
	if(TwoChannelNormals)
	{
		Norm = DeriveNormalZ(Norm.xy);
	}
	float4 Mask = texture_Mask.Sample(sampler1_s, In.TexCoord0);
	float4 Tint = texture_Tint.Sample(sampler1_s, In.TexCoord0);

	//Tint.a = 1.0f;
	//Mask.g = 1.0f;
	
	float AO = Mask.r;
	float Smoothness = max(Mask.g, 0.08f);
	float Reflectance = 0.5f;
	
	float Metallic = (MetallicChannel > 0.5f) ? Mask.a : 0.0f;
	Metallic = (MetallicChannel > 1.5f) ? Tint.a : Metallic;
	Metallic = (MetallicChannel > 2.5f) ? Mask.b : Metallic;
	float EmissiveMask = (MetallicChannel > 2.5f) ? 0.0f : Mask.b;
	
	//AO = 0.0f;
	//Smoothness = 1.0f;
	//Metallic = 1.0f;
	//Norm = float3(0,0,1);
	//Reflectance = 1.0f;
	
	float3 TintA = (TintColorA * Tint.r);
	float3 TintB = (TintColorB * Tint.g);
	float3 TintC = (TintColorC * Tint.b);
	float3 LeftOver = max(lerp(Diffuse.rgb, float3(0,0,0), Tint.r+Tint.g+Tint.b), float3(0,0,0));
	
	float3 TintColor = TintA+TintB+TintC;
	if(MetallicChannel > 1.5f && MetallicChannel < 2.5f)
	{
		if(TintColor.r+TintColor.g+TintColor.b > 0.0f && Tint.a > 0.0f)
		{
			TintColor = lerp(TintColor, (float3(1,1,1) + TintColor) / 4, Tint.a);
		}
	}
	
	Norm = CalcWorldSpaceNormals(Norm, In.Tangent, In.Bitangent, In.Normal);
	
	GBuffer.BaseColor = (Diffuse.rgb * TintColor) + LeftOver;
	GBuffer.SpecularMult = 1.0f;
	GBuffer.WorldNormals = Norm.xyz;
	GBuffer.Reflectance = Reflectance;
	GBuffer.MaterialAO = AO;
	GBuffer.Smoothness = Smoothness;
	GBuffer.Metallic = Metallic;
	GBuffer.Radiosity = CalculateRadiosity(lightProbe, Norm) * AO;
	GBuffer.Emissive = MP_Light * EmissiveMask * Emissive_Intensity;
#endif

// MEC
#if PROFILE_20160607==1
	float DetailNormalTiling = TintColorB.y;
	float DetailNormalStrength = TintColorB.x;
	float HasDiffuseColor = TintColorB.z;
	float Hastexture_Diffuse = TintColorA.w;
	
	if (TintColorB.x <= 0.0f)
	{
		DetailNormalTiling = 1.0f;
		DetailNormalStrength = 1.0f;
	}

	float4 NRS = texture_NRS.Sample(sampler1_s, In.TexCoord0 * DetailNormalTiling);
	float4 RSM = texture_RSM.Sample(sampler1_s, In.TexCoord0);
	float4 OSSSMT = texture_Ex.Sample(sampler1_s, In.TexCoord0);
	
	float3 Norm = DeriveNormalZ((NRS.xy * 2 - 1) * DetailNormalStrength);
	
	float Smoothness = (Usestexture_RSM > 0.5f) ? RSM.y : NRS.w;
	float Reflectance = (Usestexture_RSM > 0.5f) ? RSM.x : NRS.z;
	
	float Metallic = (Usestexture_RSM > 0.5f) ? RSM.z : 0.0f;
	Metallic = (Usestexture_Ex > 0.5f) ? OSSSMT.b : Metallic;
	
	float3 tintColor = lerp(float3(1,1,1), TintColorA.rgb, HasDiffuseColor);
	Diffuse.rgb = lerp(float3(1,1,1), Diffuse.rgb, Hastexture_Diffuse);
	Diffuse.rgb *= tintColor.rgb;
	
	Norm = CalcWorldSpaceNormals(Norm, In.Tangent, In.Bitangent, In.Normal);
	
	GBuffer.BaseColor = Diffuse.rgb;
	GBuffer.SpecularMult = 1.0f;
	GBuffer.WorldNormals = Norm.xyz;
	GBuffer.Reflectance = Reflectance;
	GBuffer.MaterialAO = Diffuse.a;
	GBuffer.Smoothness = Smoothness;
	GBuffer.Metallic = Metallic;
	GBuffer.Radiosity = CalculateRadiosity(lightProbe, Norm) * Diffuse.a;
#endif

// SWBF
#if PROFILE_20151117==1
	if (MultiUV > 7999.0f)
	{
		In.TexCoord0 = clamp(In.TexCoord0, 0.0f, 1.0f);
	}
	
	float4 Norm = texture_Normal.Sample(sampler1_s, In.TexCoord0);
	float4 MSRAO = texture_MSR.Sample(sampler1_s, In.TexCoord0);
	
	float3 Normal = Norm.xyz * 2 - 1;
	
	if (MultiUV - 8000.0f > 0.5f)
	{
		Diffuse.xyzw = texture_Diffuse.Sample(sampler1_s, In.TexCoord1);
		MSRAO = texture_MSR.Sample(sampler1_s, In.TexCoord1);
	}
	
	float Metallic = (Hastexture_MSR > 0.5f) ? MSRAO.r : 0.0f;
	
	if (TwoChannelNormals > 2.5f)
	{
		Normal = DeriveNormalZ(Normal.xy);
		Metallic = Norm.b;
	}
	else if (TwoChannelNormals > 1.5f)
	{
		Normal = DeriveNormalZ(Norm.zw * 2 - 1);
	}
	else if (TwoChannelNormals > 0.5f)
	{
		Normal = DeriveNormalZ(Normal.xy);
	}
	
	float Specular = 1.0f;
	float Smoothness = (Hastexture_MSR > 0.5f) ? MSRAO.g : Diffuse.a;
	float Reflectance = (Hastexture_MSR > 1.5f) ? MSRAO.b : 0.5f;
	float MaterialAO = (Hastexture_MSR > 2.5f) ? MSRAO.a : 1.0f;
	
	Normal = CalcWorldSpaceNormals(Normal, In.Tangent, In.Bitangent, In.Normal);
	
	GBuffer.BaseColor = Diffuse.rgb;
	GBuffer.SpecularMult = 1.0f;
	GBuffer.WorldNormals = Normal.xyz;
	GBuffer.Reflectance = Reflectance;
	GBuffer.MaterialAO = MaterialAO;
	GBuffer.Smoothness = Smoothness;
	GBuffer.Metallic = Metallic;
	GBuffer.Radiosity = CalculateRadiosity(lightProbe, Normal) * MaterialAO;
#endif

// DAI
#if PROFILE_20141118==1	
	float3 Norm = texture_Normal.Sample(sampler1_s, In.TexCoord0).xyz * 2 - 1;
	float3 Mask = texture_Specular.Sample(sampler1_s, In.TexCoord0).rgb;
	float4 Tint = texture_Tint.Sample(sampler1_s, In.TexCoord0);
	
	Norm = DeriveNormalZ(Norm.xy);
	//float Roughness = Mask.r;
	float Smoothness = Mask.g;
	float Reflectance = Mask.r;
	
	float Metallic = (IsBodyMat < 0.5f) ? Mask.b : 0.0f;
	//float Specular = (Metallic > 0.0f) ? max(Mask.g * 1.5f, 1.0f) : Mask.g;
	
	float3 TintA = (TintColorA.rgb * Tint.r * TintColorA.w);
	float3 TintB = (TintColorB.rgb * Tint.g * TintColorB.w);
	float3 TintC = (TintColorC.rgb * Tint.b * TintColorC.w);
	float3 TintD = (TintColorD.rgb * Tint.a * TintColorD.w);
	float3 LeftOver = max(lerp(Diffuse.rgb, float3(0,0,0), (Tint.r * TintColorA.w)+(Tint.g * TintColorB.w)+(Tint.b * TintColorC.w)+(Tint.a * TintColorD.w)), float3(0,0,0));
	float3 TintColor = TintA+TintB+TintC+TintD;
	
	Norm = CalcWorldSpaceNormals(Norm, In.Tangent, In.Bitangent, In.Normal);
	
	GBuffer.BaseColor = (Diffuse.rgb * TintColor) + LeftOver;
	GBuffer.SpecularMult = 1.0f;
	GBuffer.WorldNormals = Norm.xyz;
	GBuffer.Reflectance = Reflectance;
	GBuffer.MaterialAO = 1.0f;
	GBuffer.Smoothness = Smoothness;
	GBuffer.Metallic = Metallic;
	GBuffer.Radiosity = CalculateRadiosity(lightProbe, Norm);
#endif

// FIFA
#if PROFILE_20160927==1 || PROFILE_20191101==1
	float3 Norm = texture_Normal.Sample(sampler1_s, In.TexCoord0).xyz * 2 - 1;
	float4 Coeff = texture_Coeff.Sample(sampler1_s, In.TexCoord0);
	float  SpecMask = texture_SpecMask.Sample(sampler1_s, In.TexCoord0);
	Norm = DeriveNormalZ(Norm.xy);
	
	SpecMask = (HasSpecmask > 0.5f) ? SpecMask : 1.0f;
	
	float Reflectance = Coeff.r;
	float Smoothness = Coeff.g;
	float Metallic = (HasSpecmask > 0.5f) ? 0.0f : Coeff.b;
	float MaterialAO = Coeff.a;
	
#if PROFILE_20191101==1
	if (StdNormals > 0.0f)
		Norm = CalcWorldSpaceNormals(Norm, In.Tangent, In.Bitangent, In.Normal);
	else
		Norm = CalcWorldSpaceNormalsFromTangentSpace(Norm.xyz, In.TBN);
#else
	Norm = CalcWorldSpaceNormals(Norm, In.Tangent, In.Bitangent, In.Normal);
#endif
	
	GBuffer.BaseColor = Diffuse.rgb;
	GBuffer.SpecularMult = 1.0f;
	GBuffer.WorldNormals = Norm.xyz;
	GBuffer.Reflectance = Reflectance;
	GBuffer.MaterialAO = MaterialAO;
	GBuffer.Smoothness = Smoothness;
	GBuffer.Metallic = Metallic;
	GBuffer.Radiosity = CalculateRadiosity(lightProbe, Norm) * MaterialAO;
#endif
    
// BF1    
#if PROFILE_20161021==1
	float4 Norm = texture_Normal.Sample(sampler1_s, In.TexCoord0);
	
	float MaterialAO = 1.0f;
	float Reflectance = Norm.z;
	float Metallic = Norm.a;
	float Smoothness = Diffuse.a;
	
	Norm.xyz = normalize(DeriveNormalZ(Norm.xy * 2 - 1));
	Norm.xyz = CalcWorldSpaceNormals(Norm.xyz, In.Tangent, In.Bitangent, In.Normal);
	
	GBuffer.BaseColor = Diffuse.rgb;
	GBuffer.SpecularMult = 1.0f;
	GBuffer.WorldNormals = Norm.xyz;
	GBuffer.Reflectance = Reflectance;
	GBuffer.MaterialAO = MaterialAO;
	GBuffer.Smoothness = Smoothness;
	GBuffer.Metallic = Metallic;
	GBuffer.Radiosity = CalculateRadiosity(lightProbe, Norm) * MaterialAO;
#endif

// SWBF2 
#if PROFILE_20171117==1
	float MarkingsChannel = TintColorA.w;
	float MarkingsUse2Masks = TintColorB.w;
	float MarkingsCamoScaling = TintColorC.w;
	float MarkingsUseTexture = TintColorA.b;
	float Hastexture_Markings = TintColorA.r;
	float HasMarkings2Texture = TintColorA.g;
	
	float4 Norm = texture_Normal.Sample(sampler1_s, In.TexCoord0);
	float4 AOSlice = texture_AOSlice.Sample(sampler1_s, In.TexCoord0);
	float4 RSSSAO = texture_RSSSAO.Sample(sampler1_s, In.TexCoord0);

	float Metallic = (MetallicChannel > 0.5f) ? Norm.b : 0.0f;
	float MaterialAO = (AOChannel > 0.5f) ? AOSlice.r : 1.0f;
	
	uint sliceIndex = (uint)(clamp(AOSlice.g, 0, 0.999f) * 3);
		
	Metallic = (MetallicChannel > 1.5f) ? Norm.a : Metallic;
	MaterialAO = (AOChannel > 1.5f) ? Norm.b : MaterialAO;
	MaterialAO = (AOChannel > 2.5f) ? Norm.a : MaterialAO;
	MaterialAO = (SmoothnessChannel.y > 0.5f) ? RSSSAO.b : MaterialAO;
	
	sliceIndex = (AOChannel > 1.5f) ? (uint)(clamp(Norm.a, 0, 0.999f) * 3) : sliceIndex;
	float4 DetailNS = texture_NormalDetailTextureArray.Sample(sampler1_s, float3(In.TexCoord0 * Detail_Tiling[sliceIndex], sliceIndex));
	
	if (MultiUV > 0.5f)
	{
		Diffuse.xyzw = texture_Diffuse.Sample(sampler1_s, In.TexCoord1);
		Norm.zw = texture_Normal.Sample(sampler1_s, In.TexCoord1).zw;
		
		Metallic = (MetallicChannel > 0.5f) ? Norm.b : 0.0f;
		Metallic = (MetallicChannel > 1.5f) ? Norm.a : Metallic;
		
		MaterialAO = (AOChannel > 1.5f) ? Norm.a : MaterialAO;
		MaterialAO = (AOChannel > 2.5f) ? Norm.b : MaterialAO;
	}
	
	if (Hastexture_Markings > 0.5f)
	{
		float4 Markings1 = texture_Markings.Sample(sampler1_s, In.TexCoord0);
		if (TintColorD.r < 0.5f)
			Markings1 = texture_Markings.Sample(sampler1_s, In.TexCoord1);
			
		float3 tintColor = TintColorB.rgb;
		if (MarkingsUseTexture > 0.5f)
		{
			tintColor.rgb = texture_MarkingsCamo.Sample(sampler1_s, In.TexCoord0 * MarkingsCamoScaling).rgb * TintColorB.rgb;
		}
		
		Diffuse.rgb = lerp(Diffuse.rgb, Diffuse.rgb * (Markings1.r * tintColor.rgb), MarkingsChannel == 0.0f ? Markings1.r : 0.0f);
		Diffuse.rgb = lerp(Diffuse.rgb, Diffuse.rgb * (Markings1.g * tintColor.rgb), MarkingsChannel == 1.0f ? Markings1.g : 0.0f);
		Diffuse.rgb = lerp(Diffuse.rgb, Diffuse.rgb * (Markings1.b * tintColor.rgb), MarkingsChannel == 2.0f ? Markings1.b : 0.0f);
	}
	if (HasMarkings2Texture > 0.5f && MarkingsUse2Masks > 0.5f)
	{
		float4 Markings2 = texture_Markings2.Sample(sampler1_s, In.TexCoord0);
		if (TintColorD.r < 0.5f)
			Markings2 = texture_Markings2.Sample(sampler1_s, In.TexCoord1);
			
		float3 tintColor = TintColorC.rgb;
		if (MarkingsUseTexture > 0.5f)
		{
			tintColor.rgb = texture_MarkingsCamo.Sample(sampler1_s, In.TexCoord0 * MarkingsCamoScaling).rgb * TintColorB.rgb;
		}
		
		Diffuse.rgb = lerp(Diffuse.rgb, Diffuse.rgb * (Markings2.r * TintColorC.rgb), MarkingsChannel == 0.0f ? Markings2.r : 0.0f);
		Diffuse.rgb = lerp(Diffuse.rgb, Diffuse.rgb * (Markings2.g * TintColorC.rgb), MarkingsChannel == 1.0f ? Markings2.g : 0.0f);
		Diffuse.rgb = lerp(Diffuse.rgb, Diffuse.rgb * (Markings2.b * TintColorC.rgb), MarkingsChannel == 2.0f ? Markings2.b : 0.0f);
	}

	float Smoothness = Diffuse.a;
	if (SmoothnessChannel.x > 0.5f)
		Smoothness = Norm.b;
		
	float Reflectance = (SmoothnessChannel.y > 0.5f) ? RSSSAO.r : 0.5f;
	
	Norm.xyz = DeriveNormalZ(Norm.xy * 2 - 1).xyz;
	
	if ((Detail_Tiling[0] + Detail_Tiling[1] + Detail_Tiling[2]) > 0)
	{
		Norm.xyz += (DeriveNormalZ(DetailNS.xy * 2 - 1) * NormalDetail_Intensity[sliceIndex]) * (1-Metallic);
		Smoothness += (DetailNS.z * SmoothnessDetail_Intensity[sliceIndex]) * (1-Metallic);
	}
	
	Norm.xyz = CalcWorldSpaceNormals(normalize(Norm.xyz), In.Tangent, In.Bitangent, In.Normal);
	
	GBuffer.BaseColor = Diffuse.rgb;
	GBuffer.SpecularMult = 1.0f;
	GBuffer.WorldNormals = Norm.xyz;
	GBuffer.Reflectance = Reflectance;
	GBuffer.MaterialAO = MaterialAO;
	GBuffer.Smoothness = Smoothness;
	GBuffer.Metallic = Metallic;
	GBuffer.Radiosity = CalculateRadiosity(lightProbe, Norm.xyz) * MaterialAO;
#endif

// FIFA18
#if PROFILE_20170929==1
	float3 Norm = texture_Normal.Sample(sampler1_s, In.TexCoord0).xyz * 2 - 1;
	float4 Coeff = texture_Coeff.Sample(sampler1_s, In.TexCoord0);
	float  SpecMask = texture_SpecMask.Sample(sampler1_s, In.TexCoord0);
	
	SpecMask = (HasSpecmask > 0.5f) ? SpecMask : 1.0f;
	
	float Reflectance = Coeff.r;
	float Smoothness = Coeff.g;
	float Metallic = (HasSpecmask > 0.5f) ? 0.0f : Coeff.b;
	float MaterialAO = 1.0f;
	
	Norm.xyz = DeriveNormalZ(Norm.xy);
	Norm.xyz = CalcWorldSpaceNormals(Norm.xyz, In.Tangent, In.Bitangent, In.Normal);
	
	GBuffer.BaseColor = Diffuse.rgb;
	GBuffer.SpecularMult = 1.0f;
	GBuffer.WorldNormals = Norm.xyz;
	GBuffer.Reflectance = Reflectance;
	GBuffer.MaterialAO = MaterialAO;
	GBuffer.Smoothness = Smoothness;
	GBuffer.Metallic = Metallic;
	GBuffer.Radiosity = CalculateRadiosity(lightProbe, Norm) * MaterialAO;
#endif

// MADDEN
#if PROFILE_20180807==1
	float2 texCoords = (MultiUV > 0.5f) ? In.TexCoord1 : In.TexCoord0;
	Diffuse = texture_Diffuse.Sample(sampler1_s, texCoords);
		
	float4 Norm = texture_Normal.Sample(sampler1_s, texCoords);
	float4 RSM = texture_RSM.Sample(sampler1_s, texCoords);
	
	float Reflectance = RSM.r;
	float Smoothness = (MetallicChannel > 1.5f) ? Norm.b : RSM.g;
	float Metallic = (MetallicChannel > 1.5f) ? Norm.a : (RSM.b * MetallicChannel);
	float MaterialAO = 1.0f;
	
	Norm.xyz = DeriveNormalZ(Norm.xy * 2 - 1);
	
	float3 norm = CalcWorldSpaceNormalsFromTangentSpace(Norm.xyz, In.TBN);
	if (UseStdNormals > 0.5f)
		norm = CalcWorldSpaceNormals(Norm.xyz, In.Tangent, In.Bitangent, In.Normal);
		
	GBuffer.BaseColor = Diffuse.rgb;
	GBuffer.SpecularMult = 1.0f;
	GBuffer.WorldNormals = norm.xyz;
	GBuffer.Reflectance = Reflectance;
	GBuffer.MaterialAO = MaterialAO;
	GBuffer.Smoothness = Smoothness;
	GBuffer.Metallic = Metallic;
	GBuffer.Radiosity = CalculateRadiosity(lightProbe, GBuffer.WorldNormals) * MaterialAO;
#endif

// NFS
#if PROFILE_20151103==1
	float4 NRS = texture_Normal.Sample(sampler1_s, In.TexCoord0);
	float4 Mask = texture_Other.Sample(sampler1_s, In.TexCoord0);
	float3 Norm = DeriveNormalZ(NRS.xy * 2 - 1);
	
	float Smoothness = 0.0f;
	float Metallic = 0.0f;
	float Specular = 1.0f;
	float MaterialAO = 1.0f;
	float Reflectance = 0.5f;
	
	Norm = CalcWorldSpaceNormalsFromTangentSpace(Norm, In.TBN);
	
	GBuffer.BaseColor = Diffuse * 1.5f;
	GBuffer.SpecularMult = 1.0f;
	GBuffer.WorldNormals = Norm.xyz;
	GBuffer.Reflectance = Reflectance;
	GBuffer.MaterialAO = MaterialAO;
	GBuffer.Smoothness = Smoothness;
	GBuffer.Metallic = Metallic;
	GBuffer.Radiosity = CalculateRadiosity(lightProbe, Norm) * MaterialAO;
#endif

// PVZ2
#if PROFILE_20150223==1
	float3 Norm = texture_Normal.Sample(sampler1_s, In.TexCoord0).xyz * 2 - 1;
	float3 Mask = texture_ASM.Sample(sampler1_s, In.TexCoord0).xyz;
	Norm = DeriveNormalZ(Norm.xy);
	
	float MaterialAO = Mask.r;
	float Smoothness = Mask.g;
	float Reflectance = 0.5f;
	float Metallic = Mask.b;
	
	Norm = CalcWorldSpaceNormals(Norm, In.Tangent, In.Bitangent, In.Normal);
	
	GBuffer.BaseColor = Diffuse.rgb;
	GBuffer.SpecularMult = 1.0f;
	GBuffer.WorldNormals = Norm.xyz;
	GBuffer.Reflectance = Reflectance;
	GBuffer.MaterialAO = MaterialAO;
	GBuffer.Smoothness = Smoothness;
	GBuffer.Metallic = Metallic;
	GBuffer.Radiosity = CalculateRadiosity(lightProbe, Norm) * MaterialAO;
#endif

// PVZ
#if PROFILE_20140225==1
	float3 Norm = texture_Normal.Sample(sampler1_s, In.TexCoord0).xyz;
	float4 PSA = texture_PSA.Sample(sampler1_s, In.TexCoord0);
	Norm = DeriveNormalZ(Norm.xy * 2 - 1);
	
	float MaterialAO = 1.0f;
	float Smoothness = PSA.b;
	float Reflectance = PSA.g;
	float Metallic = 0.0f;
	
	Norm = CalcWorldSpaceNormals(Norm, In.Tangent, In.Bitangent, In.Normal);
	
	GBuffer.BaseColor = Diffuse.rgb;
	GBuffer.SpecularMult = 1.0f;
	GBuffer.WorldNormals = Norm.xyz;
	GBuffer.Reflectance = Reflectance;
	GBuffer.MaterialAO = MaterialAO;
	GBuffer.Smoothness = Smoothness;
	GBuffer.Metallic = Metallic;
	GBuffer.Radiosity = CalculateRadiosity(lightProbe, Norm) * MaterialAO;
#endif

// NFS Payback
#if PROFILE_20171110==1
	float4 NRS = texture_Normal.Sample(sampler1_s, In.TexCoord0);
	float3 Norm = DeriveNormalZ(NRS.xy * 2 - 1);
	
	float Smoothness = 0.0f;
	float Metallic = 0.0f;
	float Specular = 1.0f;
	float MaterialAO = 1.0f;
	float Reflectance = 0.5f;
	
	Norm = CalcWorldSpaceNormalsFromTangentSpace(Norm, In.TBN);
	
	GBuffer.BaseColor = Diffuse * 1.5f;
	GBuffer.SpecularMult = 1.0f;
	GBuffer.WorldNormals = Norm.xyz;
	GBuffer.Reflectance = Reflectance;
	GBuffer.MaterialAO = MaterialAO;
	GBuffer.Smoothness = Smoothness;
	GBuffer.Metallic = Metallic;
	GBuffer.Radiosity = CalculateRadiosity(lightProbe, Norm) * MaterialAO;
#endif

// BFV
#if PROFILE_20180628==1
	float4 Norm = texture_Normal.Sample(sampler1_s, In.TexCoord0);
	
	float MaterialAO = lerp(1.0f, Norm.b, 1-SmoothnessChannel);
	float Reflectance = 0.5f;
	float Metallic = 0.0f;
	float Smoothness = lerp(0.5f, Norm.b, SmoothnessChannel);
	
	Norm.xyz = normalize(DeriveNormalZ(Norm.xy * 2 - 1));
	Norm.xyz = CalcWorldSpaceNormals(Norm.xyz, In.Tangent, In.Bitangent, In.Normal);
	
	GBuffer.BaseColor = Diffuse.rgb;
	GBuffer.SpecularMult = 1.0f;
	GBuffer.WorldNormals = Norm.xyz;
	GBuffer.Reflectance = Reflectance;
	GBuffer.MaterialAO = MaterialAO;
	GBuffer.Smoothness = Smoothness;
	GBuffer.Metallic = Metallic;
	GBuffer.Radiosity = CalculateRadiosity(lightProbe, Norm.xyz) * MaterialAO;
#endif

// Anthem
#if PROFILE_20181207==1
	float4 Norm = texture_Normal.Sample(sampler1_s, In.TexCoord0);
	float4 AMMS = texture_AMMS.Sample(sampler1_s, In.TexCoord0);
	
	float MaterialAO = 1.0f;
	float Reflectance = 0.5f;
	float Metallic = AMMS.b;
	float Smoothness = AMMS.a;
	
	Norm.xyz = normalize(DeriveNormalZ(Norm.xy * 2 - 1));
	Norm.xyz = CalcWorldSpaceNormals(Norm.xyz, In.Tangent, In.Bitangent, In.Normal);
	
	GBuffer.BaseColor = Diffuse.rgb;
	GBuffer.SpecularMult = 1.0f;
	GBuffer.WorldNormals = Norm.xyz;
	GBuffer.Reflectance = Reflectance;
	GBuffer.MaterialAO = MaterialAO;
	GBuffer.Smoothness = Smoothness;
	GBuffer.Metallic = Metallic;
	GBuffer.Radiosity = CalculateRadiosity(lightProbe, Norm) * MaterialAO;
#endif
	
// NFS14
#if PROFILE_20131115==1
	float4 NRS = texture_Normal.Sample(sampler1_s, In.TexCoord0);
	
	float Smoothness = 0.0f;
	float Metallic = 0.0f;
	float Specular = 1.0f;
	float MaterialAO = 1.0f;
	float Reflectance = 0.5f;
	
	GBuffer.BaseColor = Diffuse * 1.5f;
	GBuffer.SpecularMult = 1.0f;
	GBuffer.WorldNormals = CalcWorldSpaceNormals(NRS.xyz, In.Tangent, In.Bitangent, In.Normal);
	GBuffer.Reflectance = Reflectance;
	GBuffer.MaterialAO = MaterialAO;
	GBuffer.Smoothness = Smoothness;
	GBuffer.Metallic = Metallic;
	GBuffer.Radiosity = CalculateRadiosity(lightProbe, GBuffer.WorldNormals) * MaterialAO;
#endif

// PVZ3
#if PROFILE_20190905==1
	float4 Norm = texture_Normal.Sample(sampler1_s, In.TexCoord0);
	
	float MaterialAO = 1.0f;
	float Reflectance = 0.5f;
	float Metallic = 0.0f;
	float Smoothness = 0.0f;
	
	Norm.xyz = normalize(DeriveNormalZ(Norm.xy * 2 - 1));
	Norm.xyz = CalcWorldSpaceNormals(Norm.xyz, In.Tangent, In.Bitangent, In.Normal);
	
	GBuffer.BaseColor = Diffuse.rgb;
	GBuffer.SpecularMult = 1.0f;
	GBuffer.WorldNormals = Norm.xyz;
	GBuffer.Reflectance = Reflectance;
	GBuffer.MaterialAO = MaterialAO;
	GBuffer.Smoothness = Smoothness;
	GBuffer.Metallic = Metallic;
	GBuffer.Radiosity = CalculateRadiosity(lightProbe, Norm) * MaterialAO;
#endif

	// store into gbuffers
	PackGBufferValues(GBuffer, Out.GBufferA, Out.GBufferB, Out.GBufferC, Out.GBufferD, normalBasisTransforms, texture_normalBasisCubemapTexture, sampler0_s, exposureMultipliers.x);
	Out.Depth = In.Position.z;
	
    return Out;
}