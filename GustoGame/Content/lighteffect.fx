#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler s0;

texture lightMask;
sampler2D lightSampler = sampler_state
{
	Texture = <lightMask>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(s0, input.TextureCoordinates);  
    float4 lightColor = tex2D(lightSampler, input.TextureCoordinates);

	if (lightColor.r == 0.0f || lightColor.g == 0.0f || lightColor.b == 0.0f) {
		lightColor = 1;
	}

    return color * lightColor; 
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};