sampler s0 : register(s0);


struct VertexShaderInput
{
	float4 color    : COLOR0;
	float2 coord    : TEXCOORD0;
	float4 position : POSITION0;
};

struct VertexShaderOutput
{
	float4 color    : COLOR0;
	float2 coord    : TEXCOORD0;
	float4 position : POSITION0;
};

float4 PixelShaderTest1(VertexShaderOutput input) : COLOR0
{
	float4 Color;
	Color = tex2D(s0, input.coord);
	Color += tex2D(s0, input.coord + (0.005));
	Color += tex2D(s0, input.coord - (0.005));
	Color = Color / 3;
	return Color;
}

technique BlurTest1
{
 pass Pass1
 {
 // A post process shader only needs a pixel shader.
	 PixelShader = compile ps_2_0 PixelShaderTest1();
 }
}


