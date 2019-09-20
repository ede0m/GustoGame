#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;
float xWaveLength;
float xWaveHeight;

texture bumpMap;
sampler2D bumpSampler = sampler_state
{
	Texture = <bumpMap>;
};

texture water;
sampler2D waterSampler = sampler_state
{
	Texture = <water>;
};
// MAG,MIN,MIRRR SETTINGS? SEE RIEMERS

/*struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TextureCords : TEXCOORD;
	float4 Color : COLOR0;
};*/

/*struct VertexShaderOutput
{
	float4 Pos : SV_POSITION;
	float2 BumpMapSamplingPos : TEXCOORD2;
	float4 Color : COLOR0;
};*/

/*VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.BumpMapSamplingPos = input.TextureCords/xWaveLength;
	output.Pos = mul(input.Position, WorldViewProjection);
	output.Color = input.Color;

	return output;
}*/

float4 MainPS(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : COLOR
{
	float4 bumpColor = tex2D(bumpSampler, texCoord.xy);
	//get offset 
    float2 perturbation = xWaveHeight * (bumpColor.rg - 0.5f)*2.0f;

    //apply offset to coordinates in original texture
    float2 currentCoords = texCoord.xy;
    float2 perturbatedTexCoords = currentCoords + perturbation;

    //return the perturbed values
    float4 color = tex2D(waterSampler, perturbatedTexCoords);
    return color;
}

technique oceanRipple
{
	pass P0
	{
		//VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};