float4x4 World;
float4x4 View;
float4x4 Projection;

float3 CamPos;
float3 AllowedRotDir;

//float2 Size;
//float3 Up;
//float3 Side;

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
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	float3 pos = input.Position;

	//float2 offset = Size * float2(((input.UV.x - 0.5f) * 2.0f), (-(input.UV.y - 0.5f) * 2.0f));
	//pos += offset.x * Side + offset.y * Up;


	float3 center = mul(pos, World);
		float3 eyeVector = center - CamPos;

		float3 upVector = AllowedRotDir;
		upVector = normalize(upVector);
	float3 sideVector = cross(eyeVector, upVector);
		sideVector = normalize(sideVector);

	float3 finalPosition = center;
	finalPosition += (input.UV.x - 0.5f)*sideVector;
	finalPosition += (1.5f - input.UV.y*1.5f)*upVector;

	float4 finalPosition4 = float4(finalPosition, 1);

	float4x4 preViewProjection = mul(View, Projection);
	output.Position = mul(finalPosition4, preViewProjection);

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
		AlphaBlendEnable = TRUE;
		DestBlend = INVSRCALPHA;
		SrcBlend = SRCALPHA;
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
