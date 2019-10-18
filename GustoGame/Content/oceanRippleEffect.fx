#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float2 noiseOffset;
float2 noisePower;
float noiseFrequency;

float2 camMove;

texture noiseTexture;
sampler2D noiseSampler = sampler_state
{
    Texture = <noiseTexture>;
    MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

texture water;
sampler2D waterSampler = sampler_state
{
	Texture = <water>;
};


float4 MainPS(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : COLOR
{
    float4 noise = tex2D(noiseSampler, (texCoord.xy + noiseOffset.xy - camMove.xy) * noiseFrequency);
    float2 offset = (noisePower * (noise.xy - 0.5f) * 2.0f);

    float4 color = tex2D(waterSampler, texCoord.xy + offset.xy);
    return color;
}

technique oceanRipple
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};