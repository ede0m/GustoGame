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

float4 camMove;

texture noiseTexture;
sampler2D noiseSampler = sampler_state
{
    Texture = <noiseTexture>;
    MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    AddressU = WRAP;
    AddressV = WRAP;
};

texture water;
sampler2D waterSampler = sampler_state
{
	Texture = <water>;
	MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    AddressU = WRAP;
    AddressV = WRAP;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 texCoord : TEXCOORD0;
};

struct VertexShaderOutput 
{
	float4 Position : POSITION0;
	float2 texCoord : TEXCOORD0;
};

VertexShaderOutput MainVS(VertexShaderInput input)
{
	VertexShaderOutput output;
	output.Position = input.Position;
	output.texCoord = input.texCoord;
	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 noise = tex2D(noiseSampler, (input.texCoord.xy - noiseOffset.xy + camMove.xy) * noiseFrequency);
    float2 offset = (noisePower * (noise.xy - 0.5f));

    float4 color = tex2D(waterSampler, input.texCoord.xy + offset.xy);
    return color;
}


technique oceanRipple
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};