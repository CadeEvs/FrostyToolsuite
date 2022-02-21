
struct VS_IN
{
    uint VertexID : SV_VertexID;
};

struct VS_OUT
{
    float4 Position : SV_Position;
	float4 WorldPos : POSITION;
    float2 TexCoord : TEXCOORD;
	float4 Color : COLOR;
};

cbuffer FrameConstants : register(b0)
{
    float4x4 Projection;
    float4x4 View;
	float4 TimeParams;
	float4 ScreenResolution;
};

VS_OUT VS_Main(VS_IN In)
{
    VS_OUT Out;

	float Scale = 512.0f;
	float UVScale = Scale * 2;
	
    if (In.VertexID == 0 || In.VertexID == 3)
    {
        Out.Position = mul(Projection, mul(View, float4(-Scale, 0, Scale, 1)));
		Out.WorldPos = float4(-Scale, 0, Scale, 1);
        Out.TexCoord = float2(0, 0) + float2(0.5, 0.5f);
		Out.Color = float4(1,1,1,1);
    }
    else if (In.VertexID == 1)
    {
        Out.Position = mul(Projection, mul(View, float4(Scale, 0, Scale, 1)));
		Out.WorldPos = float4(Scale, 0, Scale, 1);
        Out.TexCoord = float2(UVScale, 0) + float2(0.5, 0.5f);
		Out.Color = float4(1,1,1,1);
    }
    else if (In.VertexID == 2 || In.VertexID == 4)
    {
        Out.Position = mul(Projection, mul(View, float4(Scale, 0, -Scale, 1)));
		Out.WorldPos = float4(Scale, 0, -Scale, 1);
        Out.TexCoord = float2(UVScale, UVScale) + float2(0.5, 0.5f);
		Out.Color = float4(1,1,1,1);
    }
    else if (In.VertexID == 5)
    {
        Out.Position = mul(Projection, mul(View, float4(-Scale, 0, -Scale, 1)));
		Out.WorldPos = float4(-Scale, 0, -Scale, 1);
        Out.TexCoord = float2(0, UVScale) + float2(0.5, 0.5f);
		Out.Color = float4(1,1,1,1);
    }

    return Out;
}

struct PS_IN
{
    float4 Position : SV_Position;
	float4 WorldPos : POSITION;
    float2 TexCoord : TEXCOORD;
	float4 Color : COLOR;
};

Texture2D<float> gridTexture;
SamplerState Sampler;

float4 PS_Main(PS_IN In) : SV_Target
{
	float2 pos = float2(In.WorldPos.x, In.WorldPos.z);
	float alpha = lerp(1.0f, 0.0f, length(pos) / 16.0f);
	
	float3 color = float3(0.5,0.5,0.5);
	color = lerp(color, float3(0.25,0,0), 1 - (clamp(abs(pos.y / 0.25), 0, 1)));
	color = lerp(color, float3(0,0.25,0), 1 - (clamp(abs(pos.x / 0.25), 0, 1)));
	
    return gridTexture.Sample(Sampler, In.TexCoord) * float4(color, alpha);
}