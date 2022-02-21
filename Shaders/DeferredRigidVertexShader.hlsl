
struct VS_IN
{
    float3 Position : POSITION;
    float3 Normal : NORMAL;
    float3 Tangent : TANGENT;
    float3 Bitangent : BINORMAL;
	float  BitangentSign : BINORMALSIGN;
    float2 TexCoord0 : TEXCOORD0;
    float2 TexCoord1 : TEXCOORD1;
    float2 TexCoord2 : TEXCOORD2;
    float4 Color0 : COLOR0;
    float4 Color1 : COLOR1;
    uint4 BoneIndices0 : BLENDINDICES0;
    uint4 BoneIndices1 : BLENDINDICES1;
    float4 BoneWeights0 : BLENDWEIGHT0;
    float4 BoneWeights1 : BLENDWEIGHT1;
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

cbuffer FrameConstants : register(b0)
{
    float4x4 Projection;
    float4x4 View;
	float4 TimeParams;
	float4 ScreenResolution;
};

cbuffer ObjectConstants : register(b1)
{
    float4x4 Model;
};

Buffer<float4> BonePartMatrices : register(t0);

float3x4 GetBoneMatrix(int Index)
{
	float4 A = BonePartMatrices[Index * 3];
	float4 B = BonePartMatrices[Index * 3 + 1];
	float4 C = BonePartMatrices[Index * 3 + 2];
	return float3x4(A, B, C);
}

float3x3 CalcTangentSpaceNormal(uint ts, float3x3 world)
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
	
	r0.xyz = normalize(mul(r7.wxy, world));
	//TBN[0].xyz = r0.xyz;
	
	r6.x = r7.z;
	r3.xyz = r7.ywx * r6.yzx;
	r3.xyz = r7.xyw * r6.zxy + -r3.xyz;
	r5.xyz = r7.xyw * r6.zxy;
	r5.xyz = r7.ywx * r6.yzx + -r5.xyz;
	r0.w = (int)ts & 1;
	r3.xyz = r0.www ? r3.xyz : r5.xyz;
	
	r2.xyz = normalize(mul(-r6.xyz, world));
	r1.xyz = normalize(mul(r3.xyz, world));
	
	TBN[0].y = r1.x;
	TBN[0].x = r2.x;
	TBN[0].z = r0.x;
	TBN[1].y = r1.y;
	TBN[2].y = r1.z;
	TBN[1].x = r2.y;
	TBN[2].x = r2.z;
	TBN[1].z = r0.y;
	TBN[2].z = r0.z;
	
	return TBN;
}

VS_OUT main(VS_IN input)
{
    VS_OUT output = (VS_OUT) 0;
	
	//float3x4 PartMatrix = GetBoneMatrix(input.BoneIndices0.x);
	//float4 PosWorld = float4(mul(PartMatrix, float4(input.Position.xyz, 1.0f)), 1.0f);
	
    output.Position = mul(Projection, mul(View, mul(Model, float4(input.Position.xyz, 1.0f))));
    output.Normal = mul((float3x3)Model, input.Normal);
    output.Tangent = mul((float3x3)Model, input.Tangent);
    output.Bitangent.xyz = mul((float3x3)Model, input.Bitangent);
    output.TexCoord0 = input.TexCoord0;
    output.TexCoord1 = input.TexCoord1;
    output.TexCoord2 = input.TexCoord2;
    output.Color0 = input.Color0;
    output.Color1 = input.Color1;
	output.Bitangent.w = input.BitangentSign;
	output.TBN = CalcTangentSpaceNormal(input.TangentSpace, (float3x3)Model);
	
    return output;
}