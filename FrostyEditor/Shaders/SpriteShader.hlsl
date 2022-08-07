
struct VertexShaderIntermediates
{
	float4 Pos : SV_POSITION;
	float3 Normal : NORMAL;
	float2 TexCoord0 : TEXCOORD;
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
	Intermediates.Normal = Input.Normal.xyz;// TransformTBN2(Input, Input.Normal.xyz);
	Intermediates.TexCoord0 = Input.TexCoord0;
	
	return Intermediates;
}

//GBuffer GetPixelShaderIntermediates(PixelShaderInput Input)
//{
//	GBuffer values = (GBuffer)0;
	
//	float4 color = Sprite.Sample(sampler0_s, Input.TexCoord0);
//	clip(color.a < 0.01f ? -1 : 1);
	
//	values.BaseColor = lerp(color.rgb, Tint.xyz, color.r);
//	//values.Radiosity = color.rgb * Tint.xyz;
//	values.WorldNormals = normalize(Input.Normal.xyz);
//	values.Reflectance = 0.0f;
//	values.Smoothness = 0.0f;
//	values.Metallic = 0.0f;
//	values.MaterialAO = 1.0f;
//	values.Radiosity = CalculateRadiosity(values.WorldNormals);
	
//	return values;
//}

#define RENDER_PATH
float4 GetPixelShaderIntermediates(PixelShaderInput Input)
{
	float4 color = Sprite.Sample(sampler0_s, Input.TexCoord0);
	//clip(color.a < 0.01f ? -1 : 1);
	
	return float4(color.rgb * clamp(Tint.xyz, float3(0,0,0), float3(1,1,1)), color.a);
}