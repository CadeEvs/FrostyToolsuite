
// -----------------------------------------------------------------------------------
// Vertex Shader
// -----------------------------------------------------------------------------------

struct VertexShaderIntermediates
{
	float4 Pos : SV_POSITION;
	float3 Normal : NORMAL;
	float2 TexCoord0 : TEXCOORD0;
};

VertexShaderIntermediates GetVertexShaderIntermediates(VertexShaderInput Input)
{
	VertexShaderIntermediates Intermediates = (VertexShaderIntermediates)0;

	Intermediates.Pos = TransformPosition(Input, Input.Pos.xyz);
	Intermediates.Normal = TransformTBN(Input, Input.Normal);
	Intermediates.TexCoord0 = Input.TexCoord0 * 8;

	return Intermediates;
}

// -----------------------------------------------------------------------------------
// Pixel Shader
// -----------------------------------------------------------------------------------

GBuffer GetPixelShaderIntermediates(PixelShaderInput Input)
{
	float3 color = BaseColor.Sample(sampler1_s, Input.TexCoord0).rgb;
	float3 norm = normalize(Input.Normal);

	GBuffer values = (GBuffer)0;

	values.BaseColor = float3(0.53f, 0.53f, 0.53f);
	values.WorldNormals = norm;
	values.Reflectance = SMR.b;
	values.Smoothness = 0.2f;
	values.Metallic = SMR.g;
	values.MaterialAO = 1.0f;
	values.Radiosity = CalculateRadiosity(values.WorldNormals);
	values.Emissive = float3(0, 0, 0);

	return values;
}
