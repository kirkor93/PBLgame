#define RADIUS 4
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

float4 PSGaussianBlur(float2 texCoord : TEXCOORD) : COLOR0
{
	float4 color = float4(0, 0, 0, 0);

	for (int i = 0; i < KERNEL_SIZE; i++)
		color += tex2D(colorMap, texCoord + offsets[i]) * weights[i];

	return color;
}

technique GaussianBlur
{
	pass
	{
		PixelShader = compile ps_2_0 PSGaussianBlur();
	}
}
