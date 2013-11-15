sampler2D input : register(s0); 

Texture xColoredTexture;

sampler ColoredTextureSampler = sampler_state {
	texture = <xColoredTexture> ;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter=LINEAR;
	AddressU = mirror;
	AddressV = mirror;
};

float4 PixelShaderTest1(in float2 coord: TEXCOORD) : COLOR0
{
    float4 Color;
    Color =  tex2D(ColoredTextureSampler, coord);
    Color += tex2D(ColoredTextureSampler, coord + (0.01));
    Color += tex2D(ColoredTextureSampler, coord - (0.01));
    Color = Color / 3;
    return Color;
}

float4 PixelShaderTest2(in float2 coord: TEXCOORD) : COLOR0
{
    float4 Color;
    Color =  tex2D(ColoredTextureSampler, coord);
//    Color += tex2D(ColoredTextureSampler, coord + (0.01));
//    Color = Color / 2;
    return Color;
}

float4x4 MatrixTransform;

void SpriteVertexShader(inout float4 color    : COLOR0,
	inout float2 texCoord : TEXCOORD0,
	inout float4 position : SV_Position)
{
	position = mul(position, MatrixTransform);
}

technique BlurTest1
{
 pass Pass1
 {
 // A post process shader only needs a pixel shader.
	 PixelShader = compile ps_3_0 PixelShaderTest1();
	 VertexShader = compile vs_3_0 SpriteVertexShader();
 }
}

technique BlurTest2
{
 pass Pass1
 {
 // A post process shader only needs a pixel shader.
	 PixelShader = compile ps_3_0 PixelShaderTest2();
	 VertexShader = compile vs_3_0 SpriteVertexShader();
 }
}


