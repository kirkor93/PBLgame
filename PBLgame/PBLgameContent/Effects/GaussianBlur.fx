
/// Radius of blur. Change it equally in GaussianBlur class:
#define RADIUS 10
#define KERNEL_SIZE (RADIUS * 2 + 1)

float weights[KERNEL_SIZE];
float2 offsets[KERNEL_SIZE];
texture colorMapTexture;

sampler2D colorMap = sampler_state
{
	Texture = (colorMapTexture);
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;
};

float4x4 MatrixTransform;

void SpriteVertexShader(inout float4 color : COLOR0,
	inout float2 texCoord : TEXCOORD0,
	inout float4 position : SV_Position)
{
	position = mul(position, MatrixTransform);
}

float4 PSGaussianBlur(float2 texCoord : TEXCOORD) : COLOR0
{
	float4 color = float4(0, 0, 0, 0);

	for (int i = 0; i < KERNEL_SIZE; i++)
		color += float4(tex2D(colorMap, texCoord + offsets[i]).rgb, 1.0f) * weights[i];

	return color;
}

technique GaussianBlur
{
	pass
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_3_0 PSGaussianBlur();
	}
}
