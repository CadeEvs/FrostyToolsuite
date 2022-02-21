
float3 F_Schlick(float3 f0, float f90, float u)
{
	return f0 + (f90-f0) * pow(1.f - u, 5.f);
}

float Fr_DisneyDiffuse(float NoV, float NoL, float LoH, float linearRoughness)
{
	float energyBias = lerp(0, 0.5f,linearRoughness);
	float energyFactor = lerp(1.0f, 1.0f/1.51f, linearRoughness);
	float fd90 = energyBias + 2.0f * LoH*LoH * linearRoughness;
	float3 f0 = float3(1,1,1);
	float lightScatter = F_Schlick(f0, fd90, NoL).r;
	float viewScatter = F_Schlick(f0, fd90, NoV).r;
	
	return lightScatter*viewScatter*energyFactor;
}

float V_SmithGGXCorrelated(float NoL, float NoV, float alphaG)
{
	float alphaG2 = alphaG*alphaG;
	float Lambda_GGXV = NoL * sqrt((-NoV * alphaG2 + NoV) * NoV + alphaG2);
	float Lambda_GGXL = NoV * sqrt((-NoL * alphaG2 + NoL) * NoL + alphaG2);
	
	return 0.5f / (Lambda_GGXV + Lambda_GGXL);
}

float D_GGX(float NoH, float m)
{
	float m2 = m*m;
	float f = (NoH * m2 - NoH)*NoH + 1;
	return m2/(f*f);
}