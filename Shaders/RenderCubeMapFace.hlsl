
struct PS_IN
{
    float4 Position : SV_Position;
    float2 TexCoord : TEXCOORD;
};

cbuffer CameraConstants : register(b0)
{
    row_major float4x4 InvProjection;
    row_major float4x4 InvView;
    float3 ViewDirection;
    float nearClip;
    float farClip;
    float3 Padding;
}

cbuffer RenderCubemapConstants : register(b1)
{
	int CubeMapFace;
}

TextureCube<float4> EnvTexture : register(t0);
SamplerState EnvSampler : register(s0);

float4 PS_Main(PS_IN input) : SV_Target
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