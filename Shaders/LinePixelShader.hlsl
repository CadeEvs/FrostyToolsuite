struct PS_IN
{
    float4 Position : SV_Position;
    float4 Color : COLOR;
};

float4 main(PS_IN input) : SV_Target
{
    return input.Color;
}