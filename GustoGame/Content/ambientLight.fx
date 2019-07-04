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
float percentThroughDay = 0.0f;

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 pixelColor = tex2D(s0, input.TextureCoordinates);
    float4 outputColor = pixelColor;
	
	// lighting intensity is gradient of pixel position
	float Intensity = 1 + (1  - input.TextureCoordinates.y) * 1.3;
	outputColor.r = outputColor.r / ambient * Intensity;
	outputColor.g = outputColor.g / ambient * Intensity;
	outputColor.b = outputColor.b / ambient * Intensity;

	// sun set/rise blending 
	float exposeRed = (1 + (.39 - input.TextureCoordinates.y) * 8); // overexpose red
	float exposeGreen = (1 + (.39 - input.TextureCoordinates.y) * 2); // some extra green for the blue pixels

	// happens on top 3rd of screen
	if (input.TextureCoordinates.y < 0.70f) {

		float redAdder = max(1, (exposeRed * (percentThroughDay/0.25f))); // be at full exposure at 25% of day gone
		float greenAdder = max(1, (exposeGreen * (percentThroughDay/0.25f))); // be at full exposure at 25% of day gone

		// begin reducing adders
		if (percentThroughDay >= 0.25f && percentThroughDay < 0.50f) {
			redAdder = max(1, (exposeRed * (1-(percentThroughDay - 0.25f)/0.25f)));
			greenAdder = max(1, (exposeGreen * (1-(percentThroughDay - 0.25f)/0.25f)));
		}
		
		//mid day
		else if (percentThroughDay >= 0.50f && percentThroughDay < 0.75f) {
			redAdder = 1;
			greenAdder = 1;
		}
		
		// add adders back for sunset
		else if (percentThroughDay >= 0.75f && percentThroughDay < 0.85f) {
			redAdder = max(1, (exposeRed * ((percentThroughDay - 0.75f)/0.10f)));
			greenAdder = max(1, (exposeGreen * ((percentThroughDay - 0.75f)/0.10f)));
		}
		
		// begin reducing adders
		else if (percentThroughDay >= 0.85f) {
			redAdder = max(1, (exposeRed * (1-(percentThroughDay - 0.85f)/0.15f)));
			greenAdder = max(1, (exposeGreen * (1-(percentThroughDay - 0.85f)/0.15f)));
		}

		outputColor.r = outputColor.r * redAdder;
		outputColor.g = outputColor.g * greenAdder;
	}

	// Sepia
	//outputColor.r = (pixelColor.r * 0.393) + (pixelColor.g * 0.769) + (pixelColor.b * 0.189);
    //outputColor.g = (pixelColor.r * 0.349) + (pixelColor.g * 0.686) + (pixelColor.b * 0.168);    
    //outputColor.b = (pixelColor.r * 0.272) + (pixelColor.g * 0.534) + (pixelColor.b * 0.131);

	// Greyscale
	//float value = (pixelColor.r + pixelColor.g + pixelColor.b) / 3;
	//outputColor.r = value;
	//outputColor.g = value;
	//outputColor.b = value;

	return outputColor;
}

technique ambientLightDayNight
{
	pass P0
	{
		PixelShader = compile ps_2_0 MainPS();
	}
};