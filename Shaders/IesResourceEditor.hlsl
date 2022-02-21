
#define PI (3.1415926535897932f)
#define INV_TWO_PI (1.0f/(2.0f*PI))

struct VS_OUT
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
};

Texture2D iesTexture : t0;
SamplerState iesSampler;

float4 PS_Main(VS_OUT input) : SV_Target
{
	float4 lightPos = float4(512.0f,512.0f,128.0f,1.0f);
	matrix m = matrix(
		float4(1,0,0,0),
		float4(0,1,0,0),
		float4(0,0,1,0),
		lightPos
		);
		
	float3 L = normalize(lightPos.xyz - input.Position.xyz);
	float3 iesSampleDirection = mul(m, -L);
	
	float phiCoord = (iesSampleDirection.z * 0.5f) + 0.5f;
	float theta = atan2(iesSampleDirection.x, iesSampleDirection.y);
	float thetaCoord = theta * INV_TWO_PI;
	float3 texCoord = float3(thetaCoord, phiCoord, 0);
	float color = iesTexture.SampleLevel(iesSampler, texCoord, 0).r;
	return float4(color.rrrr);
}