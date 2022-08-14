
struct VertexShaderIntermediates
{
	float4 Pos : SV_POSITION;
	float3 Normal : NORMAL;
	//float3 Tangent : TANGENT;
	//float4 Binormal : BINORMAL;
};

float3 TransformTBN2(VertexShaderInput Input, float3 Value)
{
	return mul(normalize(Value), (float3x3)worldMatrix);
}

VertexShaderIntermediates GetVertexShaderIntermediates(VertexShaderInput Input)
{
	VertexShaderIntermediates Intermediates = (VertexShaderIntermediates)0;
	
	Intermediates.Pos = TransformPosition(Input, Input.Pos.xyz);
	Intermediates.Normal = TransformTBN2(Input, Input.Normal.xyz);
	//Intermediates.Tangent = TransformTBN2(Input, Input.Normal.xyz);
	//Intermediates.Binormal.xyz = TransformTBN2(Input, Input.Binormal.xyz);
	//Intermediates.Binormal.w = Input.Binormal.w;
	
	return Intermediates;
}

GBuffer GetPixelShaderIntermediates(PixelShaderInput Input)
{
	GBuffer values = (GBuffer)0;
	
	//Input.Binormal.xyz = normalize(cross(Input.Normal.xyz, Input.Tangent.xyz) * -Input.Binormal.w);
	
	values.BaseColor = Color.xyz;
	values.WorldNormals = normalize(Input.Normal.xyz); //CalcWorldSpaceNormals(float3(0,0,1), Input.Tangent, Input.Binormal, Input.Normal);
	values.Reflectance = 0.0f;
	values.Smoothness = 0.0f;
	values.Metallic = 0.0f;
	values.MaterialAO = 1.0f;
	values.Radiosity = CalculateRadiosity(values.WorldNormals);
	
	return values;
}