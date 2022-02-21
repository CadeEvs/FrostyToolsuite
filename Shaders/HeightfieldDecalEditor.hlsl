
struct VS_IN
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
};

struct VS_OUT
{
	float4 Position : SV_POSITION;
	float3 WorldPos : WORLDPOS;
	float2 TexCoord : TEXCOORD0;
};

cbuffer Constants
{
	float2 ViewportDim;
	float2 TextureDim;
	float4x4 ChannelMask;
	float SrgbEnabled;
	float MipLevel;
	float SliceLevel;
	float Padding;
	float4x4 ViewProjMatrix;
};

Texture2DArray TextureArray : t0;
SamplerState Sampler;

VS_OUT VS_Main(VS_IN input)
{
	float height = TextureArray.SampleLevel(Sampler, float3(input.TexCoord, 0), 0).r;

	VS_OUT output = (VS_OUT)0;
	output.Position = mul(float4(input.Position.x, -height * 10.0f, input.Position.z, 1.0f), ViewProjMatrix);
	output.WorldPos = float3(input.Position.x, -height * 10.0f, input.Position.z);
	output.TexCoord = input.TexCoord;

	return output;
}

float4 PS_Main(VS_OUT input) : SV_Target
{
	float3 ambientColor = float3(0.2, 0.2, 0.2);
	float3 diffuseColor = float3(0.5, 0.5, 0.5);
	float3 specColor 	= float3(1.0, 1.0, 1.0);
	float3 lightPos     = float3(50, 150, 0);

	float3 normal = normalize(cross(ddx(input.WorldPos), ddy(input.WorldPos)));
	float3 lightDir = normalize(lightPos - input.WorldPos);
	float lambertian = max(dot(lightDir,normal), 0.0);
	
	float specular = 0.0;

	if(lambertian > 0.0) 
	{
		float3 viewDir = normalize(-input.WorldPos);
		float3 halfDir = normalize(lightDir + viewDir);
		float specAngle = max(dot(halfDir, normal), 0.0);
		specular = pow(specAngle, 80.0);
	}
	
	//return float4(float3(0.25f,0.25f,0.25f) + normal.y * (float3(0.5f,0.5f,0.5f)-float3(0.25f,0.25f,0.25f)),1);
	return float4(ambientColor + lambertian * diffuseColor, 1.0f);
}