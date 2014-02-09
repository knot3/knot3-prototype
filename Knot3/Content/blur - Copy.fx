sampler s0 : register(s0);


Texture texture1;

sampler __s0 = sampler_state { texture = <texture1>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = wrap; AddressV = wrap; };


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

float4 PixelShaderTest2(float2 coord    : TEXCOORD0) : COLOR0
{
	float4 Color;
	Color = tex2D(s0, coord);
	Color += tex2D(s0, coord + (0.005));
	Color += tex2D(s0, coord - (0.005));
	Color = Color / 3;
	return Color;
}

float4x4 World;
float4x4 View;
float4x4 Projection;

VertexShaderOutput VertexShaderFunction(float4 position : POSITION, float2 coord : TEXCOORD0)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(position, World);
		float4 viewPosition = mul(worldPosition, View);
		output.position = mul(viewPosition, Projection);
	output.position = mul(position, Projection);

	output.color = float4(1,1,1,1);
	output.coord = coord;

	return output;
}

void SpriteVertexShader(inout float4 color    : COLOR0,
	inout float2 texCoord : TEXCOORD0,
	inout float4 position : POSITION0)
{
}

technique BlurTest1
{
 pass Pass1
 {
 // A post process shader only needs a pixel shader.
	 //VertexShader = compile vs_2_0 VertexShaderFunction();
	 VertexShader = compile vs_2_0 SpriteVertexShader();
	 PixelShader = compile ps_2_0 PixelShaderTest2();
 }
}


