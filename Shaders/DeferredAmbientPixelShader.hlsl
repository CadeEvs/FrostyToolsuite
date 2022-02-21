#pragma pack_matrix(row_major)
#define PI (3.1415926535897932f)
#define INV_PI (0.31830988618379067543484808035204f)

struct PS_IN
{
    float4 Position : SV_Position;
    float2 TexCoord : TEXCOORD;
};

Texture2D<float> DepthBuffer : register(t0);
Texture2D<float4> GBufferA : register(t1);
Texture2D<float4> GBufferB : register(t2);
Texture2D<float4> GBufferC : register(t3);
Texture2D<float4> GBufferD : register(t4);
Texture2D<float4> ShadowTexture : register(t5);
Texture2D<float4> DFG : register(t6);
TextureCube DiffuseLD : register(t7);
TextureCube SpecularLD : register(t8);
TextureCube<float4> EnvTexture : register(t9);
Texture2DArray LutTexture : register(t10);

SamplerState GBufferSampler : register(s0)
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

SamplerState EnvSampler : register(s1)
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

cbuffer CameraConstants : register(b0)
{
    float4x4 InvProjection;
    float4x4 InvView;
    float3 ViewDirection;
    float nearClip;
    float farClip;
    float3 Padding;
}

cbuffer AmbientConstants : register(b1)
{
    float3 Ambient;
	float UseGradient;
    float3 TopColor;
	float RenderMode;
    float3 HorizonColor;
	float MipCount;
}

cbuffer LightConstants : register(b2)
{
	float3 LightColor;
	float Intensity;
	float4 ExposureMultipliers;
	float HasLut;
}

//#define nearClip 0.03f
//#define farClip 1000.0f

// NOTE: Don't worry about the extra computations that never get used, those will be removed by the aggressively optimizing compiler.

float ComputeReflectionCaptureMipFromRoughness(float Roughness, float MaxMips)
{
	float LevelFrom1x1 = 1 - 1.2 * log2(Roughness);
	return MaxMips - 1 - LevelFrom1x1;
}

float linearDepth(float depthSample)
{
    float A = farClip / (farClip - nearClip);
    float B = (-farClip * nearClip) / (farClip - nearClip);
    return B / (depthSample - A);
}

float3 PositionFromDepth2(float2 uv, float depth)
{
    // First, unproject the ray to the back clip plane
    float4 rayToNearClip = mul(float4(uv.x * 2 - 1, (1 - uv.y) * 2 - 1, 1.0, 1.0), InvProjection);
    // Then, divide by .z to obtain the point on the ray where z = 1
    rayToNearClip.xyz /= rayToNearClip.z;
    // Multiply by the z value from the g-buffer shader to get the view-space position
    float4 viewpos = float4(rayToNearClip.xyz * depth, 1.0f);
    viewpos = mul(viewpos, InvView);
    return viewpos.xyz / viewpos.w;
}

float Square(float x)
{
	return x*x;
}

float Pow5(float x)
{
	float x2 = x*x;
	return x2 * x2 * x;
}

float3 Diffuse_OrenNayar(float3 DiffuseColor, float Roughness, float NoV, float NoL, float VoH)
{
	float a = Roughness * Roughness;
	float s = a;
	float s2 = s * s;
	float VoL = 2 * VoH * VoH - 1;
	float Cosri = VoL - NoV * NoL;
	float C1 = 1 - 0.5 * s2 / (s2 + 0.33);
	float C2 = 0.45 * s2 / (s2 + 0.09) * Cosri * (Cosri >= 0 ? rcp(max(NoL, NoV)) : 1);
	return DiffuseColor / PI * (C1 + C2) * (1 + Roughness * 0.5);
}

float D_GGX(float NoH, float m)
{
	float m2 = m*m;
	float f = (NoH * m2 - NoH)*NoH + 1;
	return m2/(f*f);
}

float Vis_SmithJointApprox(float Roughness, float NoV, float NoL)
{
	float a = Square(Roughness);
	float Vis_SmithV = NoL * (NoV * (1 - a) + a);
	float Vis_SmithL = NoV * (NoL * (1 - a) + a);
	return 0.5 * rcp(Vis_SmithV + Vis_SmithL);
}

float3 F_Schlick(float3 f0, float f90, float u)
{
	return f0 + (f90-f0) * pow(1.f - u, 5.f);
}

float V_SmithGGXCorrelated(float NoL, float NoV, float alphaG)
{
	float alphaG2 = alphaG*alphaG;
	float Lambda_GGXV = NoL * sqrt((-NoV * alphaG2 + NoV) * NoV + alphaG2);
	float Lambda_GGXL = NoV * sqrt((-NoL * alphaG2 + NoL) * NoL + alphaG2);
	
	return 0.5f / (Lambda_GGXV + Lambda_GGXL);
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

float3 SRgbToLinear(float3 color)
{
    color.r = (color.r <= 0.04045f) ? color.r / 12.92f : pow(((color.r + 0.055f) / 1.055f), 2.4f);
    color.g = (color.g <= 0.04045f) ? color.g / 12.92f : pow(((color.g + 0.055f) / 1.055f), 2.4f);
    color.b = (color.b <= 0.04045f) ? color.b / 12.92f : pow(((color.b + 0.055f) / 1.055f), 2.4f);
    return color;
}

float3 LinearToSRgb(float3 color)
{
    color.r = (color.r <= 0.0031308f) ? color.r * 12.92f : 1.055f * pow(color.r, 1.0f / 2.4f) - 0.055f;
    color.g = (color.g <= 0.0031308f) ? color.g * 12.92f : 1.055f * pow(color.g, 1.0f / 2.4f) - 0.055f;
    color.b = (color.b <= 0.0031308f) ? color.b * 12.92f : 1.055f * pow(color.b, 1.0f / 2.4f) - 0.055f;
    return color;
}

static const float C1 = 0.429043f;
static const float C2 = 0.511644f;
static const float C3 = 0.743125f;
static const float C4 = 0.886227f;
static const float C5 = 0.247708f;

static const float3 L00 = float3(0.4928f,0.5204f,0.5703f);
static const float3 L1m1 = float3(-0.0138f,-0.1003f,-0.3168f);
static const float3 L10 = float3(-0.1078f,-0.1108f,-0.1105f);
static const float3 L11 = float3(-0.0576f,-0.0187f,0.0561f);
static const float3 L2m2 = float3(-0.1125f,-0.1299f,-0.1722f);
static const float3 L2m1 = float3(0.0160f,0.0238f,0.0395f);
static const float3 L20 = float3(-0.0737f,-0.0848f,-0.1217f);
static const float3 L21 = float3(0.0201f,0.0178f,0.0060f);
static const float3 L22 = float3(0.0496f,0.0429f,-0.0054f);

float3 SphericalHarmonics(float3 N)
{
	return C4 * L00 + 2.0f * C2 * L11 * N.x + 2.0f * C2 * L1m1 * N.y + 2.0f * C2 * L10 * N.z + C1 * L22 * (N.x * N.x - N.y * N.y) + C3 * L20 * N.z * N.z - C5 * L20 + 2.0f * C1 * L2m2 * N.x * N.y + 2.0f * C1 * L21 * N.x * N.z + 2.0f * C1 * L2m1 * N.y * N.z;
}

float3 Tonemap(float3 x)
{
	float _A = 0.15;
	float _B = 0.50;
	float _C = 0.10;
	float _D = 0.20;
	float _E = 0.02;
	float _F = 0.30;

	return ((x*(_A*x+_C*_B)+_D*_E)/(x*(_A*x+_B)+_D*_F))-_E/_F;
}

float3 CalculateRadiosity(float4 SHProbeLight[9], float3 WorldNormal)
{
	float4 r0;
	float3 r11;
	
	r0.xw = WorldNormal.zy * WorldNormal.zy;
	r0.x = WorldNormal.x * WorldNormal.x + -r0.x;
	r11.xyz = r0.www * SHProbeLight[4].xyz;
	r11.xyz = float3(3,3,3) * r11.xyz;
	SHProbeLight[8].xyz = SHProbeLight[8].xyz * r0.xxx + r11.xyz;
	SHProbeLight[8].xyz = SHProbeLight[8].xyz + SHProbeLight[0].xyz;
	SHProbeLight[4].xyz = SHProbeLight[8].xyz + -SHProbeLight[4].xyz;
	SHProbeLight[5].xyz = SHProbeLight[5].xyz * WorldNormal.xxx;
	SHProbeLight[6].xyz = SHProbeLight[6].xyz * WorldNormal.yyy;
	SHProbeLight[6].xyz = SHProbeLight[6].xyz * WorldNormal.zzz;
	SHProbeLight[5].xyz = SHProbeLight[5].xyz * WorldNormal.yyy + SHProbeLight[6].xyz;
	SHProbeLight[7].xyz = SHProbeLight[7].xyz * WorldNormal.xxx;
	SHProbeLight[7].xyz = SHProbeLight[7].xyz * WorldNormal.zzz + SHProbeLight[5].xyz;
	SHProbeLight[4].xyz = SHProbeLight[7].xyz + SHProbeLight[4].xyz;
	SHProbeLight[2].xyz = SHProbeLight[2].xyz * WorldNormal.yyy;
	SHProbeLight[1].xyz = SHProbeLight[1].xyz * WorldNormal.xxx + SHProbeLight[2].xyz;
	SHProbeLight[1].xyz = SHProbeLight[3].xyz * WorldNormal.zzz + SHProbeLight[1].xyz;
	SHProbeLight[4].xyz = SHProbeLight[1].xyz + SHProbeLight[4].xyz;
	
	return max(float3(0,0,0), SHProbeLight[4].xyz);
}

float3 GetOffSpecularPeakReflectionDir(float3 Normal, float3 ReflectionVector, float Roughness)
{
	float a = Square(Roughness);
	return lerp( Normal, ReflectionVector, (1 - a) * ( sqrt(1 - a) + a ) );	
}

float2 getSample( uint Index, uint NumSamples, uint2 Random )
{
	float E1 = frac( (float)Index / NumSamples + float( Random.x & 0xffff ) / (1<<16) );
	float E2 = float( reversebits(Index) ^ Random.y ) * 2.3283064365386963e-10;
	return float2( E1, E2 );
}

float G_SmithGGX( float NoL, float NoV, float a2 )
{
	float a = sqrt(a2);
	float Vis_SmithV = NoL * ( NoV * ( 1 - a ) + a );
	float Vis_SmithL = NoV * ( NoL * ( 1 - a ) + a );
	return 0.5 * rcp( Vis_SmithV + Vis_SmithL );
}

float3 evaluateSpecularIBLReference(in float3 N, in float3 V, in float roughness, in float3 f0, in float f90)
{
	// Build local referential
	float3 upVector = abs(N.z) < 0.999 ? float3(0,0,1) : float3(1,0,0);
	float3 tangentX = normalize( cross( upVector, N));
	float3 tangentY = cross( N, tangentX);

	float3 accLight = 0;
	for( uint i =0; i< 32 ; ++i)
	{
		float2 u = getSample(i, 32, 0);

		// GGX NDF sampling
		float cosThetaH = sqrt((1-u.x) /(1 +( roughness * roughness -1)*u.x));
		float sinThetaH = sqrt(1 - min(1.0, cosThetaH * cosThetaH));
		float phiH = u.y * PI * 2;

		// Convert sample from half angle to incident angle
		float3 H, L;
		H = float3( sinThetaH * cos( phiH), sinThetaH *sin( phiH), cosThetaH);
		H = normalize( tangentX * H.y + tangentY * H.x + N * H.z);
		L = normalize(2.0f * dot(V,H) * H - V);

		float LdotH = saturate( dot(H, L));
		float NdotH = saturate( dot(H, N));
		float NdotV = saturate( dot(V, N));
		float NdotL = saturate( dot(L, N));

		// Importance sampling weight for each sample
		//
		// weight = fr .(N.L)
		//
		// with :
		// fr = D(H).F(H).G(V, L) /( 4(N.L)(N.O))
		//
		// Since we integrate in the microfacet space, we include the
		// Jacobian of the transform
		//
		// pdf = D(H) .(N.H) /( 4(L.H))
		float D = D_GGX(NdotH, roughness);
		float pdfH = D * NdotH ;
		float pdf = pdfH /(4.0f * LdotH);

		// Implicit weight(N.L canceled out)
		float3 F = F_Schlick(f0, f90, LdotH);
		float G = G_SmithGGX(NdotL, NdotV, roughness);
		float3 weight = F * G * D /(4.0 * NdotV);

		if( dot(L,N) >0 && pdf > 0)
		{
			accLight += EnvTexture.SampleLevel(EnvSampler, L, 0). rgb * weight / pdf ;
		}
	}
	return accLight / 32;
}

void importanceSampleCosDir(in float2 u, in float3 N, out float3 L, out float NdotL, out float pdf)
{
	// Local referencial
	float3 upVector = abs(N.z) < 0.999 ? float3(0 ,0 ,1) : float3(1 ,0 ,0);
	float3 tangentX = normalize(cross(upVector, N));
	float3 tangentY = cross(N, tangentX);

	float u1 = u.x;
	float u2 = u.y;

	float r = sqrt(u1);
	float phi = u2 * PI * 2;

	L = float3(r * cos(phi), r * sin(phi), sqrt(max(0.0f ,1.0f-u1)));
	L = normalize(tangentX * L.y + tangentY * L.x + N * L.z);

	NdotL = dot(L,N);
	pdf = NdotL * INV_PI;
}

float3 evaluateIBLDiffuseCubeReference(in TextureCube<float4> incomingLight, in SamplerState incomingLightSampler, in float3 V, in float3 worldNormal, float roughness, in float3 brdfWeight = float3(0, 0, 0), in uint sampleCount = 1024)
{
	float3 accLight = 0;

	for(uint i = 0; i < sampleCount; ++i)
	{
		float2 eta = getSample(i, sampleCount, 0);
		float3 L;
		float NdotL;
		float pdf;
		importanceSampleCosDir(eta, worldNormal, L, NdotL, pdf);
		if (NdotL >0)
		{
			// Each sample should be weighted by L * weight / pdf .
			// With :
			// - weight = NdotL
			// - pdf = NdoL / Pi
			// However the NdoLs (in weight and pdf ) and Pi cancel out
			// This is why all terms disappear here
			float f = 1.0f;

			//#if FB_DIFFUSE_MODEL == FB_DIFFUSE_DISNEY
			// Half angle formula :
			// cos (2 theta ) = 2 cos ^2( theta ) - 1
			float cosD = sqrt((dot(V, L) + 1.0f) * 0.5);
			float NdotV = saturate(dot(worldNormal, V));
			float NdotL_sat = saturate(NdotL);
			// Disney diffuse BRDF operates in linear roughness ,
			// which is the sqrt of the GGX alpha roughness term
			float fd90 = 0.5 + 2 * cosD * cosD * sqrt(roughness);
			float lightScatter = 1 + (fd90 -1) * pow(1 - NdotL_sat, 5);
			float viewScatter = 1 + (fd90 -1) * pow(1 - NdotV, 5);
			f = lightScatter * viewScatter ;
			//# endif

			accLight += incomingLight.SampleLevel(incomingLightSampler, L, 0).rgb * f;
		}
	}

	return accLight * (1.0f / sampleCount);
}

float3 getSpecularDominantDir(float3 N, float3 R, float roughness)
{
	float smoothness = saturate(1-roughness);
	float lerpFactor = smoothness * (sqrt(smoothness) + roughness);
	return lerp(N, R, lerpFactor);
}

float3 getDiffuseDominantDir(float3 N, float3 V, float NdotV, float roughness)
{
	float a = 1.02341f * roughness - 1.51174f;
	float b = -0.511705f * roughness + 0.755868f;
	float lerpFactor = saturate((NdotV * a + b) * roughness);
	return lerp(N, V, lerpFactor);
}

float3 evaluateIBLDiffuse(float3 N, float3 V, float NdotV, float roughness)
{
	float3 dominantN = getDiffuseDominantDir(N, V, NdotV, roughness);
	float3 diffuseLighting = DiffuseLD.Sample(EnvSampler, dominantN).rgb;
	
	float diffF = DFG.SampleLevel(GBufferSampler, float2(NdotV, roughness), 0).z;
	return diffuseLighting * diffF;
}

float3 evaluateIBLSpecular(float3 N, float3 R, float NdotV, float roughness, float linearRoughness, float3 f0, float fd90)
{
	float3 dominantR = getSpecularDominantDir(N, R, roughness);
	
	//NdotV = max(NdotV, 0.5f / 128.0f);
	float mipLevel = ComputeReflectionCaptureMipFromRoughness(linearRoughness, MipCount);
	//float3 preLD = EnvTexture.SampleLevel(EnvSampler, dominantR, mipLevel).rgb;
	float3 preLD = SpecularLD.SampleLevel(EnvSampler, dominantR, mipLevel).rgb;
	
	float2 preDFG = DFG.SampleLevel(GBufferSampler, float2(NdotV, roughness), 0).xy;
	return preLD * (f0 * preDFG.x + fd90 * preDFG.y);
}

float4 main(PS_IN input) : SV_Target
{
    float DepthBufferValue = DepthBuffer.Sample(GBufferSampler, input.TexCoord);
    float4 DiffuseBufferValue = GBufferA.Sample(GBufferSampler, input.TexCoord);
    float4 NormalBufferValue = GBufferB.Sample(GBufferSampler, input.TexCoord);
    float4 MaterialInfoBufferValue = GBufferC.Sample(GBufferSampler, input.TexCoord);

    float Depth = linearDepth(DepthBufferValue);
    float3 Position = PositionFromDepth2(input.TexCoord.xy, Depth);
    float3 Diffuse = DiffuseBufferValue.xyz;
	float SpecularMult = DiffuseBufferValue.w;
    float3 Normal = normalize(NormalBufferValue.xyz * 2 - 1);
    float Reflectance = NormalBufferValue.w;
    float MaterialAO = MaterialInfoBufferValue.x;
    float Smoothness = clamp(MaterialInfoBufferValue.y, 0, 0.99f);
	float Roughness = pow(1-Smoothness, 2.0f);
	float LinearRoughness = 1-Smoothness;
	float Metallic = MaterialInfoBufferValue.z;
    float Mask = MaterialInfoBufferValue.w;
	float3 SpecularColor = (0.16f * pow(Reflectance, 2.0f)).rrr;
	
	float3 Radiosity = GBufferD.Sample(GBufferSampler, input.TexCoord).rgb;
	float3 indirectDiffuse = Radiosity * Diffuse;
	
    if (Mask < 0.5f)
        discard;
	
	float4 campos = float4(0, 0, 0, 1); // the origin in view space
    campos = mul(campos, InvView);
    campos /= campos.w;
	
	float4 LightWorldPos = float4(0,0,0,1);
	LightWorldPos = mul(LightWorldPos, InvView);
	LightWorldPos /= LightWorldPos.w;
	
	float3 LightPos = LightWorldPos.xyz;
	float3 toLight = float3(-0.68733f, 0.50698f, -0.52014f);// LightPos - Position.xyz;
	
	float distanceSqr = dot(toLight, toLight);
	float distanceAttenuation = 1.0f;
	
	float Up = Normal.y * 0.5 + 0.5;
    float3 NewAmbient = float3(0.25f,0.25f,0.25f) + Up * (float3(0.5f,0.5f,0.5f)-float3(0.25f,0.25f,0.25f));
	

	float3 L = normalize(toLight * rsqrt(distanceSqr));
	float3 V = normalize(campos.xyz - Position.xyz);
	float3 N = Normal;
	float3 R = 2 * dot(V, N) * N - V;
	
	SpecularColor = 0.16f * (Reflectance*Reflectance) * (1-Metallic) + Diffuse * Metallic;
	Diffuse = (1-Metallic) * Diffuse;
	
	float shadow = ShadowTexture.Sample(GBufferSampler, input.TexCoord).r;
	float3 Color = float3(0,0,0);
	float3 BaseColor = Diffuse;
	
	float  NoL = saturate(dot(N, L));
	float3 H = normalize(V + L);
	float  NoV = abs(dot(N, V)) + 1e-5f;
	float  NoH = saturate(dot(N, H));
	float  VoH = saturate(dot(V, H));
	float  LoH = saturate(dot(L, H));
	
	// ----------------------------
	// lighting
	// ----------------------------
	
	float energyBias = lerp(0, 0.5f,LinearRoughness);
	float energyFactor = lerp(1.0f, 1.0f/1.51f, LinearRoughness);
	float fd90 = energyBias + 2.0f * LoH*LoH * LinearRoughness;
	float3 f0 = SpecularColor;
	
	float3 F = F_Schlick(f0, fd90, LoH);
	float  Vis = V_SmithGGXCorrelated(NoV, NoL, Roughness);
	float  D = D_GGX(NoH, Roughness);
	float3 Fr = D * F * Vis / PI;
	float  Fd = Fr_DisneyDiffuse(NoV, NoL, LoH, LinearRoughness) / PI;
		
	Color += ( (Diffuse * Fd) + Fr ) * NoL * (Intensity * LightColor) * shadow;
	//Color += indirectDiffuse * (1-Metallic);
	
	// ----------------------------
	// reflection environment
	// ----------------------------
	
	float3 Refl = evaluateIBLSpecular(N, R, NoV, Roughness, LinearRoughness, f0, saturate(50.0f * SpecularColor.g)) * ExposureMultipliers.x;
	float3 IBL = Radiosity * Diffuse; 
	//Refl = evaluateIBLDiffuse(N, V, NoV, Roughness) * Diffuse;
	IBL += Refl;
	Color += IBL;
	Color *= ExposureMultipliers.x;
	Color = -min(-Color.rgb, 0.0);
	
	// ----------------------------
	// post processing
	// ----------------------------
	
	float lutSize = 33.0f;
	float3 scale = (lutSize - 1.0f) / lutSize;
	float3 offset = 1.0f / (2.0f * lutSize);
	
	if (HasLut > 0.5f)
	{
		float3 lut = scale * Color + offset;
		lut = float3(lut.x, 1.0f - lut.y, lut.z * 33.0f);
		
		Color = LutTexture.Sample(GBufferSampler, lut).rgb;
	}
	
	Color = -min(-Color.rgb, 0.0);
	
	if (RenderMode <= 0.0f) // Default Lit
		return float4(Color, 1.0f);
	else if (RenderMode <= 1.0f) // Wireframe
		return float4(1,1,1,1);
	else if (RenderMode <= 2.0f) // BaseColor
		return float4(BaseColor, 1.0f);
	else if (RenderMode <= 3.0f) // SpecularColor
		return float4(SpecularColor, 1.0f);
	else if (RenderMode <= 4.0f) // Normals
		return float4(Normal * 0.5f + 0.5f, 1.0f);
	else if (RenderMode <= 5.0f) // MaterialAO
		return float4(MaterialAO, MaterialAO, MaterialAO, 1.0f);
	else if (RenderMode <= 6.0f) // Smoothness
		return float4(Smoothness, Smoothness, Smoothness, 1.0f);
	else if (RenderMode <= 7.0f) // Metallic
		return float4(Metallic, Metallic, Metallic, 1.0f);
	else if (RenderMode <= 8.0f) // Reflectance
		return float4(Reflectance, Reflectance, Reflectance, 1.0f);
	else if (RenderMode <= 9.0f) // Reflections
		return float4(Refl * Reflectance, 1.0f);
	else // Ambient
		return float4(Radiosity, 1.0f);
	return float4(0,0,0,1);
}