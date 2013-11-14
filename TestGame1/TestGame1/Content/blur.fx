sampler2D input : register(s0); 

sampler ColoredTextureSampler = sampler_state { texture = <xColoredTexture> ;    magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

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

technique BlurTest1
{
 pass Pass1
 {
 // A post process shader only needs a pixel shader.
 PixelShader = compile ps_3_0 PixelShaderTest1();
 }
}

technique BlurTest2
{
 pass Pass1
 {
 // A post process shader only needs a pixel shader.
 PixelShader = compile ps_3_0 PixelShaderTest2();
 }
}


