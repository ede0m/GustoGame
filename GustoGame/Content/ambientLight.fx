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

float ambient = 1.0f;
float percentThroughDay = 0.0f;

float4 MainPS(VertexShaderOutput input) : COLOR
{

	float4 pixelColor = tex2D(s0, input.TextureCoordinates);
	float4 outputColor = pixelColor;

	// lighting intensity is gradient of pixel position
	float Intensity = 1 + (1  - input.TextureCoordinates.y) * 1.05;
	outputColor.r = outputColor.r / ambient * Intensity;
	outputColor.g = outputColor.g / ambient * Intensity;
	outputColor.b = outputColor.b / ambient * Intensity;

	// sun set/rise blending  
	float gval = (1 - input.TextureCoordinates.y); // replace 1 with .39 to lock to 39 percent of screen (this is how it was before)
	float exposeRed = (1 + gval * 8); // overexpose red
	float exposeGreen = (1 + gval * 2); // some extra green
	float exposeBlue = (1 + gval * 4); // some extra blue 
	
	float quarterDayPercent = (percentThroughDay/0.25f);
	float redAdder = max(1, (exposeRed * quarterDayPercent)); // be at full exposure at 25% of day gone
	float greenAdder = max(1, (exposeGreen * quarterDayPercent)); // be at full exposure at 25% of day gone
	float blueAdder = max(1, (exposeBlue * quarterDayPercent)); // be at full exposure at 25% of day gone

	// begin reducing adders
	if (percentThroughDay >= 0.25f ) {
		float gradientVal1 = (1-(percentThroughDay - 0.25f)/0.25f);
		redAdder = max(1, (exposeRed * gradientVal1));
		greenAdder = max(1, (exposeGreen * gradientVal1));
		blueAdder = max(1, (exposeGreen * gradientVal1));
	}
		
	//mid day
	if (percentThroughDay >= 0.50f) {
		redAdder = 1;
		greenAdder = 1;
		blueAdder = 1;
	}
		
	// add adders back for sunset
	if (percentThroughDay >= 0.75f) {
		float gradientVal2 = ((percentThroughDay - 0.75f)/0.10f);
		redAdder = max(1, (exposeRed * gradientVal2));
		greenAdder = max(1, (exposeGreen * gradientVal2));
		blueAdder = max(1, (exposeBlue * gradientVal2));
	}
		
	// begin reducing adders
	if (percentThroughDay >= 0.85f) {
			
		float gradientVal3 = (1-(percentThroughDay - 0.85f)/0.15f);
		redAdder = max(1, (exposeRed * gradientVal3));
		greenAdder = max(1, (exposeGreen * gradientVal3));
		blueAdder = max(1, (exposeBlue * gradientVal3));
	}

	outputColor.r = outputColor.r * redAdder;
	outputColor.g = outputColor.g * greenAdder;
	outputColor.b = outputColor.b * blueAdder;

	// first check if we are in a lightMask light
	float4 lightMaskColor = tex2D(lightSampler, input.TextureCoordinates);
	if (lightMaskColor.r != 0.0f || lightMaskColor.g != 0.0f || lightMaskColor.b != 0.0f) 
	{
		// we are in the light so don't apply ambient light
		return pixelColor * (lightMaskColor + outputColor); // have to offset by outputColor here because the lightMask is pure black
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

	return outputColor * pixelColor; // must multiply by pixelColor here to offset the lightMask bounds. TODO: could try to restore original color by removing this multiplaction and factoring in more of an offset on ln 91
}

technique ambientLightDayNight
{
	pass P0
	{
		PixelShader = compile ps_3_0 MainPS();
	}
};