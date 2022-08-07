
struct VertexShaderIntermediates
{
	float4 Pos : SV_POSITION;
	float3 Normal : NORMAL;
	float2 TexCoord0 : TEXCOORD0;
	float3 Color0 : COLOR;
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
	Intermediates.TexCoord0 = Input.TexCoord0;
	Intermediates.Color0 = Input.Color0;
	
	return Intermediates;
}

float GetMaskValue(float maskIndex, float4 color1, float4 color2, float4 color3, float4 color4, float4 color5, float4 color6, float defaultValue = 1.0f)
{
	float maskValue = defaultValue;
	if (maskIndex > 22.0f) { maskValue = color6.a; }
	else if (maskIndex > 21.0f) { maskValue = color6.b; }
	else if (maskIndex > 20.0f) { maskValue = color6.g; }
	else if (maskIndex > 19.0f) { maskValue = color6.r; }
	else if (maskIndex > 18.0f) { maskValue = color5.a; }
	else if (maskIndex > 17.0f) { maskValue = color5.b; }
	else if (maskIndex > 16.0f) { maskValue = color5.g; }
	else if (maskIndex > 15.0f) { maskValue = color5.r; }
	else if (maskIndex > 14.0f) { maskValue = color4.a; }
	else if (maskIndex > 13.0f) { maskValue = color4.b; }
	else if (maskIndex > 12.0f) { maskValue = color4.g; }
	else if (maskIndex > 11.0f) { maskValue = color4.r; }
	else if (maskIndex > 10.0f) { maskValue = color3.a; }
	else if (maskIndex > 9.0f) { maskValue = color3.b; }
	else if (maskIndex > 8.0f) { maskValue = color3.g; }
	else if (maskIndex > 7.0f) { maskValue = color3.r; }
	else if (maskIndex > 6.0f) { maskValue = color2.a; }
	else if (maskIndex > 5.0f) { maskValue = color2.b; }
	else if (maskIndex > 4.0f) { maskValue = color2.g; }
	else if (maskIndex > 3.0f) { maskValue = color2.r; }
	else if (maskIndex > 2.0f) { maskValue = color1.a; }
	else if (maskIndex > 1.0f) { maskValue = color1.b; }
	else if (maskIndex > 0.0f) { maskValue = color1.g; }
	else if (maskIndex > -1.0f) { maskValue = color1.r; }
	return maskValue;
}

GBuffer GetPixelShaderIntermediates(PixelShaderInput Input)
{
	GBuffer values = (GBuffer)0;
	
	clip(Input.TexCoord0.x <= 0 || Input.TexCoord0.x >= 1 ? -1 : 1);
	clip(Input.TexCoord0.y <= 0 || Input.TexCoord0.y >= 1 ? -1 : 1);
	
	float4 color1 = float4(0,0,0,0);
	float4 color2 = float4(0,0,0,0);
	float4 color3 = float4(0,0,0,0);
	
	color1.r = LayerGroup1.Sample(sampler0_s, (Input.TexCoord0 * LayerOffsets1.zw) + LayerOffsets1.xy).r;
	color1.g = LayerGroup1.Sample(sampler0_s, (Input.TexCoord0 * LayerOffsets2.zw) + LayerOffsets2.xy).g;
	color1.b = LayerGroup1.Sample(sampler0_s, (Input.TexCoord0 * LayerOffsets3.zw) + LayerOffsets3.xy).b;
	color1.a = LayerGroup1.Sample(sampler0_s, (Input.TexCoord0 * LayerOffsets4.zw) + LayerOffsets4.xy).a;
	
	color2.r = LayerGroup2.Sample(sampler0_s, (Input.TexCoord0 * LayerOffsets5.zw) + LayerOffsets5.xy).r;
	color2.g = LayerGroup2.Sample(sampler0_s, (Input.TexCoord0 * LayerOffsets6.zw) + LayerOffsets6.xy).g;
	color2.b = LayerGroup2.Sample(sampler0_s, (Input.TexCoord0 * LayerOffsets7.zw) + LayerOffsets7.xy).b;
	color2.a = LayerGroup2.Sample(sampler0_s, (Input.TexCoord0 * LayerOffsets8.zw) + LayerOffsets8.xy).a;
	
	color3.r = LayerGroup3.Sample(sampler0_s, (Input.TexCoord0 * LayerOffsets9.zw) + LayerOffsets9.xy).r;
	color3.g = LayerGroup3.Sample(sampler0_s, (Input.TexCoord0 * LayerOffsets10.zw) + LayerOffsets10.xy).g;
	color3.b = LayerGroup3.Sample(sampler0_s, (Input.TexCoord0 * LayerOffsets11.zw) + LayerOffsets11.xy).b;
	color3.a = LayerGroup3.Sample(sampler0_s, (Input.TexCoord0 * LayerOffsets12.zw) + LayerOffsets12.xy).a;

	float4 color4 = LayerGroup4.Sample(sampler0_s, Input.TexCoord0);
	float4 color5 = LayerGroup5.Sample(sampler0_s, Input.TexCoord0);
	float4 color6 = LayerGroup6.Sample(sampler0_s, Input.TexCoord0);
	
	float maskValue = GetMaskValue(HoleMaskIndex.r, color1, color2, color3, color4, color5, color6);
	float holeValue = GetMaskValue(HoleMaskIndex.g, color1, color2, color3, color4, color5, color6, 0.0f);
	
	clip (holeValue > 0 ? -1 : 1);
	
	values.BaseColor = maskValue.rrr;// * Input.Color0.rgb;
	values.WorldNormals = normalize(Input.Normal.xyz);
	values.Reflectance = 0.0f;
	values.Smoothness = 0.0f;
	values.Metallic = 0.0f;
	values.MaterialAO = 1.0f;
	values.Radiosity = CalculateRadiosity(values.WorldNormals);
	
	return values;
}