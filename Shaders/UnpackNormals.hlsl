
float3x3 CalcTangentSpaceNormal_Quaternion(uint ts)
{
	float3x3 TBN = (float3x3)0;
	float4 r0, r1, r2, r3, r4, r5, r6, r7;

	r0.x = (uint)ts >> 22;
	r0.x = (uint)r0.x;
	r0.x = r0.x * 0.00138241798 + -0.707106769;
	if (2 == 0) r3.x = 0; else if (2 + 1 < 32) { r3.x = (uint)ts << (32 - (2 + 1)); r3.x = (uint)r3.x >> (32 - 2); }
	else r3.x = (uint)ts >> 1;
	if (9 == 0) r3.y = 0; else if (9 + 13 < 32) { r3.y = (uint)ts << (32 - (9 + 13)); r3.y = (uint)r3.y >> (32 - 9); }
	else r3.y = (uint)ts >> 13;
	if (10 == 0) r3.z = 0; else if (10 + 3 < 32) { r3.z = (uint)ts << (32 - (10 + 3)); r3.z = (uint)r3.z >> (32 - 10); }
	else r3.z = (uint)ts >> 3;
	r3.yz = (uint2)r3.zy;
	r0.zw = r3.yz * float2(0.00138241798, 0.00276754121) + float2(-0.707106769, -0.707106769);
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

	r0.xyz = normalize(r7.wxy);
	//TBN[0].xyz = r0.xyz;

	r6.x = r7.z;
	r3.xyz = r7.ywx * r6.yzx;
	r3.xyz = r7.xyw * r6.zxy + -r3.xyz;
	r5.xyz = r7.xyw * r6.zxy;
	r5.xyz = r7.ywx * r6.yzx + -r5.xyz;
	r0.w = (int)ts & 1;
	r3.xyz = r0.www ? r3.xyz : r5.xyz;

	r2.xyz = normalize(-r6.xyz);
	r1.xyz = normalize(r3.xyz);

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

float3x3 CalcTangentSpaceNormal_AxisAngle(float4 ts)
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
	u0.xyz = u0.xxx & uint3(4, 2, 8);
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

	r7.xyz = normalize(r1.xyz);
	r1.xyz = normalize(r2.xyz);
	r2.xyz = normalize(-r0.xyw);
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

StructuredBuffer<float> input : register(t0);
RWStructuredBuffer<float4> output : register(u0);

[numthreads(1, 1, 1)]
void CS_UnpackAxisAngle( uint3 id : SV_DispatchThreadID )
{
	float3x3 tbn = CalcTangentSpaceNormal_AxisAngle(float4(input[(id.x * 4) + 0], input[(id.x * 4) + 1], input[(id.x * 4) + 2], input[(id.x * 4) + 3]));

	output[(id.x * 3) + 0] = float4(tbn[0], 1.0f);
	output[(id.x * 3) + 1] = float4(tbn[1], 1.0f);
	output[(id.x * 3) + 2] = float4(tbn[2], 1.0f);
}

StructuredBuffer<uint> tsInput : register(t0);

[numthreads(1,1,1)]
void CS_UnpackQuaternion(uint3 id : SV_DispatchThreadId)
{
	float3x3 tbn = CalcTangentSpaceNormal_Quaternion(tsInput[id.x]);

	output[(id.x * 3) + 0] = float4(tbn[0], 1.0f);
	output[(id.x * 3) + 1] = float4(tbn[1], 1.0f);
	output[(id.x * 3) + 2] = float4(tbn[2], 1.0f);
}