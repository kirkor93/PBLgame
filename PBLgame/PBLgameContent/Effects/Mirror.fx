
/// =========== REFLECTIVE MAPPING ============ ///

float4x4 reflectedView;
texture reflectionMap;
sampler2D reflectionSampler = sampler_state {
	Texture = (reflectionMap);
	MinFilter = Anisotropic;
	MagFilter = Anisotropic;
};

struct VertexShaderReflectOutput
{
	float4 Position : POSITION0;
	float4 ReflectionPos : TEXCOORD0;
};

VertexShaderReflectOutput VSReflect(VertexShaderInput input)
{
	VertexShaderReflectOutput output;

	float4 worldPosition = mul(input.Position, world);
		float4 viewPosition = mul(worldPosition, view);
		output.Position = mul(viewPosition, projection);

	float4 reflect = mul(input.Position, world);
		reflect = mul(reflect, reflectedView);
	reflect = mul(reflect, projection);
	output.ReflectionPos = reflect;

	return output;
}

float4 PSReflect(VertexShaderReflectOutput input) : COLOR0
{
	float2 UV = postProjToScreen(input.ReflectionPos);
	float3 reflection = tex2D(reflectionSampler, UV);
	return float4(reflection, alphaValue);
}


technique Reflection
{
	pass Pass1
	{
		AlphaBlendEnable = TRUE;
		DestBlend = INVSRCALPHA;
		SrcBlend = SRCALPHA;
		VertexShader = compile vs_3_0 VSReflect();
		PixelShader = compile ps_3_0 PSReflect();
	}
}
