
struct VertexShaderIntermediates
{
	float4 Pos : SV_POSITION;
	float3 Normal : NORMAL;
	float2 TexCoord0 : TEXCOORD;
	float3 WorldPos : WORLDPOS;
};

float4 TransformPosition2(VertexShaderInput Input, float3 Position)
{
	float3 worldPosition = mul(float4(Position, 1.0f), worldMatrix).xyz;
	return mul(float4(worldPosition, 1.0f), viewProjMatrix);
}

float3 TransformTBN2(VertexShaderInput Input, float3 Value)
{
	return mul(normalize(Value), (float3x3)worldMatrix);
}

VertexShaderIntermediates GetVertexShaderIntermediates(VertexShaderInput Input)
{
	VertexShaderIntermediates Intermediates = (VertexShaderIntermediates)0;
	
	Intermediates.Pos = TransformPosition2(Input, Input.Pos.xyz);
	Intermediates.Normal = TransformTBN2(Input, Input.Normal.xyz);
	Intermediates.TexCoord0 = Input.TexCoord0;
	Intermediates.WorldPos = mul(float4(Input.Pos.xyz, 1.0f), worldMatrix).xyz - cameraPos;
	
	return Intermediates;
}

#define RENDER_PATH
float4 GetPixelShaderIntermediates(PixelShaderInput Input)
{
	float4 ambient = {0.75, 0.75, 0.75, 1.0}; 
	float4 color = Texture.Sample(sampler0_s, Input.TexCoord0);
	
	float selectMask = SelectMask.Sample(sampler0_s, Input.TexCoord0).r;
	int selectIndex = (int)SelectIndex.r;
	
	if (selectIndex == 1 && selectMask > 0.5f) color = float4(1, 1, 0, 1);
	else if (selectIndex == 2 && selectMask > 0.3f && selectMask < 0.5f) color = float4(1, 1, 0, 1);
	else if (selectIndex == 0 && selectMask > 0.0f && selectMask < 0.3f) color = float4(1, 1, 0, 1);
	
	float v = clamp(dot(normalize(Input.Normal), normalize(-Input.WorldPos)), 0, 1);
	float b = lerp(0.5, 1, v);
	float a = pow(v, 20) * 0.5f;
	
	return color * (a + b);
}