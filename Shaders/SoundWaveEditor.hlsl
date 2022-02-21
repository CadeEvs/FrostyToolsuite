
cbuffer Constants
{
	float4   color;
	float4x4 waveForms[8];
}

float4 VS_Main(float2 Pos : POSITION, uint vid : SV_VertexID) : SV_Position
{
	uint x = 0;
	uint y = 0;
	uint z = vid / 16;
	uint w = z * 16;
	
	vid -= w;
	x = vid / 4;
	y = vid % 4;
	
	float PosY = -1.0f + color.w + waveForms[z][x][y] * 0.2857f;
	return float4(Pos.x, PosY, 0, 1);
}

float4 PS_Main(float4 Pos : SV_Position) : SV_Target
{
	return float4(color.rgb, 1.0f);
}