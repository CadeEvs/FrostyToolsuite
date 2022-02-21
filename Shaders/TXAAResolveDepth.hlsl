
Texture2DMS<float4> SceneDepthTextureMS : register(t0);

void ResolveDepthMinAndAvgLinear(out float minDepth, float2 InUVs) 
{
	float2 dimensions; int samples;
	SceneDepthTextureMS.GetDimensions(dimensions.x, dimensions.y, samples);
	
	int2 loadAddress = int2(InUVs * dimensions);
	minDepth = 1000000;
	
	for(int index = 0;index < samples;++index)
	{
		float s = SceneDepthTextureMS.Load(loadAddress,index).r;
		minDepth = min(minDepth,s);
	}
}

struct PS_IN
{
    float4 Position : SV_Position;
    float2 TexCoord : TEXCOORD;
};

// Writes to SV_Target
void PS_Main(in PS_IN In, out float minDepth : SV_Target0) 
{
	ResolveDepthMinAndAvgLinear(minDepth, In.TexCoord);
}

Texture2D<float4> nonMsaaTexture : register(t0);
SamplerState textureSampler;

float PS_Resolve(PS_IN In) : SV_Depth
{
	return nonMsaaTexture.Sample(textureSampler, In.TexCoord).r;
}