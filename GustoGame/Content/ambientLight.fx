#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
sampler s0;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};


float ambient = 1.0f;

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 pixelColor = tex2D(s0, input.TextureCoordinates);
    float4 outputColor = pixelColor;
    
	// Sepia
	//outputColor.r = (pixelColor.r * 0.393) + (pixelColor.g * 0.769) + (pixelColor.b * 0.189);
    //outputColor.g = (pixelColor.r * 0.349) + (pixelColor.g * 0.686) + (pixelColor.b * 0.168);    
    //aaoutputColor.b = (pixelColor.r * 0.272) + (pixelColor.g * 0.534) + (pixelColor.b * 0.131);

	// Greyscale
	//float value = (pixelColor.r + pixelColor.g + pixelColor.b) / 3;
	
	outputColor.r = outputColor.r / ambient;
	outputColor.g = outputColor.g / ambient;
	outputColor.b = outputColor.b / ambient;

	return outputColor;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile ps_2_0 MainPS();
	}
};