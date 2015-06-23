
/// ========== SHADOWS GENERATING ========== ///

struct VertexShaderShadowsOutput
{
	float4 Position : POSITION0;
	float4 ScreenPos : TEXCOORD0;
	float3 WorldPos  : TEXCOORD1;
};

VertexShaderShadowsOutput VShadows(VertexShaderInput input)
{
	VertexShaderShadowsOutput output;

#ifdef SKINNED
	Skin(input, 4);
#endif

	float4 worldPosition = mul(input.Position, world);
	float4 viewPosition = mul(worldPosition, view);
	output.Position = mul(viewPosition, projection);
	output.ScreenPos = output.Position;
	output.WorldPos = worldPosition.xyz;

	return output;
}

// ---- Pointlight Shadows ---- //
float4 PShadows(VertexShaderShadowsOutput input) : COLOR0
{
	float depth = saturate(length(shadowLightPos - input.WorldPos) / shadowFarPlane);
	return float4(depth, 0, 0, 1);
}

// ---- Directional Shadows ---- //
float4 PShadowsDir(VertexShaderShadowsOutput input) : COLOR0
{
	float depth = saturate(input.ScreenPos.z / input.ScreenPos.w);
	return float4(depth, 0, 0, 1);
}

technique Shadows
{
	pass Pass1
	{
		AlphaBlendEnable = FALSE;
		VertexShader = compile vs_3_0 VShadows();
		PixelShader = compile ps_3_0 PShadows();
	}
}
technique ShadowsDir
{
	pass Pass1
	{
		AlphaBlendEnable = FALSE;
		VertexShader = compile vs_3_0 VShadows();
		PixelShader = compile ps_3_0 PShadowsDir();
	}
}
