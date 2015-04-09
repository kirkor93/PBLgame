float4x4 World;
float4x4 View;
float4x4 Projection;

//float Time;
//float Lifespan;
float2 Size;
//float3 Wind;
float3 Up;
float3 Side;
//float FadeInTime;

texture ParticleTexture;
sampler2D texSampler = sampler_state
{
	Texture = (ParticleTexture);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
	//float3 Direction : TEXCOORD1;
	//float Speed: TEXCOORD2;
	//float StartTime : TEXCOORD3;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
	//float2 RelativeTime : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	float3 pos = input.Position;

	float2 offset Size * float2((input.UV.x - 0.5f)* 2.0f), -(input.UV.y - 0.5f) * 2.0f);

	pos += offset.X * Side + offset.Y * Up;

	float4 worldPosition = mul(pos, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.UV = input.UV;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(texSampler, input.UV);

	return color;
}

technique Bilboard
{
    pass Pass1
    {

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
