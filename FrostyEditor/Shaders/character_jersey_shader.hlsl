
// -----------------------------------------------------------------------------------
// Vertex Shader
// -----------------------------------------------------------------------------------

struct VertexShaderIntermediates
{
	float4 Pos : SV_POSITION;
	float3 Normal : NORMAL;
    float3 Tangent : TANGENT;
    float4 Binormal : BINORMAL;
	float2 TexCoord0 : TEXCOORD0;
	float4 Color0 : COLOR;
};

VertexShaderIntermediates GetVertexShaderIntermediates(VertexShaderInput Input)
{
	VertexShaderIntermediates Intermediates = (VertexShaderIntermediates)0;
	
    Intermediates.Pos = TransformPosition(Input, Input.Pos.xyz);
	Intermediates.Normal = TransformTBN(Input, Input.Normal.xyz);
    Intermediates.Tangent = TransformTBN(Input, Input.Tangent.xyz);
    Intermediates.Binormal.xyz = TransformTBN(Input, Input.Binormal.xyz);
	Intermediates.Binormal.w = Input.Binormal.w;
	Intermediates.TexCoord0 = Input.TexCoord0;
	Intermediates.Color0 = Input.Color0;
	
	return Intermediates;
}

// -----------------------------------------------------------------------------------
// Pixel Shader
// -----------------------------------------------------------------------------------

GBuffer GetPixelShaderIntermediates(PixelShaderInput Input)
{
	float2 crestCoords = float2(0,0);
	crestCoords.x = clamp((Input.TexCoord0.x - debugCrestHotspot.x) / (debugCrestHotspot.z - debugCrestHotspot.x), 0, 1);
	crestCoords.y = clamp((Input.TexCoord0.y - debugCrestHotspot.y) / (debugCrestHotspot.w - debugCrestHotspot.y), 0, 1);
	
	float2 numberBackCoords = float2(0,0);
	numberBackCoords.x = clamp((Input.TexCoord0.x - debugNumberBackHotspot.x) / ((debugNumberBackHotspot.z - debugNumberBackHotspot.x) * 0.5f), 0, 1);
	numberBackCoords.y = clamp((Input.TexCoord0.y - debugNumberBackHotspot.y) / ((debugNumberBackHotspot.w - debugNumberBackHotspot.y)), 0, 1);
	
	float4 numberBackTen = numberTensTexture.Sample(sampler2_s, numberBackCoords);
	numberBackCoords.x = clamp((Input.TexCoord0.x - (debugNumberBackHotspot.x + ((debugNumberBackHotspot.z - debugNumberBackHotspot.x) * 0.5f))) / ((debugNumberBackHotspot.z - debugNumberBackHotspot.x) * 0.5f), 0, 1);
	float4 numberBackUnit = numberUnitsTexture.Sample(sampler2_s, numberBackCoords);
	
	float2 numberFrontCoords = float2(0,0);
	numberFrontCoords.x = clamp((Input.TexCoord0.x - debugNumberFrontHotspot.x) / ((debugNumberFrontHotspot.z - debugNumberFrontHotspot.x) * 0.5f), 0, 1);
	numberFrontCoords.y = clamp((Input.TexCoord0.y - debugNumberFrontHotspot.y) / ((debugNumberFrontHotspot.w - debugNumberFrontHotspot.y)), 0, 1);
	
	float4 numberFrontTen = numberTensTexture.Sample(sampler2_s, numberFrontCoords);
	numberFrontCoords.x = clamp((Input.TexCoord0.x - (debugNumberFrontHotspot.x + ((debugNumberFrontHotspot.z - debugNumberFrontHotspot.x) * 0.5f))) / ((debugNumberFrontHotspot.z - debugNumberFrontHotspot.x) * 0.5f), 0, 1);
	float4 numberFrontUnit = numberUnitsTexture.Sample(sampler2_s, numberFrontCoords);
	
	float4 color = colorTexture.Sample(sampler1_s, Input.TexCoord0);
	
	float3 norm = normalTexture.Sample(sampler1_s, Input.TexCoord0).xyz * 2 - 1;	
	norm = DeriveNormalZ(norm.xy);
	
	float4 coeff = coefficientTexture.Sample(sampler1_s, Input.TexCoord0);
	float4 crest = crestTexture.Sample(sampler2_s, crestCoords);
	
	color.rgb = lerp(color.rgb, crest.rgb, crest.a);
	color.rgb = lerp(color.rgb, (numberBackTen.r * debugJerseyNumberColorPrimary.rgb) + (numberBackTen.g * debugJerseyNumberColorSecondary.rgb) + (numberBackTen.b * debugJerseyNumberColorTertiary.rgb), numberBackTen.a);
	color.rgb = lerp(color.rgb, (numberBackUnit.r * debugJerseyNumberColorPrimary.rgb) + (numberBackUnit.g * debugJerseyNumberColorSecondary.rgb) + (numberBackUnit.b * debugJerseyNumberColorTertiary.rgb), numberBackUnit.a);
	color.rgb = lerp(color.rgb, (numberFrontTen.r * debugJerseyNumberColorPrimary.rgb) + (numberFrontTen.g * debugJerseyNumberColorSecondary.rgb) + (numberFrontTen.b * debugJerseyNumberColorTertiary.rgb), numberFrontTen.a);
	color.rgb = lerp(color.rgb, (numberFrontUnit.r * debugJerseyNumberColorPrimary.rgb) + (numberFrontUnit.g * debugJerseyNumberColorSecondary.rgb) + (numberFrontUnit.b * debugJerseyNumberColorTertiary.rgb), numberFrontUnit.a);
	
	GBuffer values = (GBuffer)0;
	
	values.BaseColor = lerp(color.rgb, crest.rgb, crest.a);
	values.WorldNormals = CalcWorldSpaceNormals(norm, Input.Tangent, Input.Binormal, Input.Normal);
	values.Reflectance = coeff.r;
	values.Smoothness = coeff.g;
	values.Metallic = coeff.b;
	values.MaterialAO = 1.0f;
	values.Radiosity = CalculateRadiosity(values.WorldNormals);
	
	return values;
}