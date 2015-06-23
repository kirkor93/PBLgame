
/// ========== GLOW ========== ///

struct VertexShaderGlowOutput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
	float3 WorldPos  : TEXCOORD1;
};

VertexShaderGlowOutput VSGlow(VertexShaderInput input)
{
	VertexShaderGlowOutput output;

#ifdef SKINNED
	Skin(input, 4);
#endif

	float4 worldPosition = mul(input.Position, world);
	float4 viewPosition = mul(worldPosition, view);
	output.Position = mul(viewPosition, projection);
	output.WorldPos = worldPosition.xyz;
	output.TextureCoordinate = input.TextureCoordinate;

	return output;
}

float4 PSGlow(VertexShaderGlowOutput input) : COLOR0
{
	float4 emissive = (tex2D(emissiveSampler, input.TextureCoordinate).rgba * emissiveIntensity) * emissiveColor;
	emissive.a = 1.0f;
	return emissive;
}

technique Glow
{
	pass Pass1
	{
		AlphaBlendEnable = FALSE;
		VertexShader = compile vs_3_0 VSGlow();
		PixelShader = compile ps_3_0 PSGlow();
	}
}
