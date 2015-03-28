float4x4 world;
float4x4 view;
float4x4 projection;

float4x4 worldInverseTranspose;

//#define maxLights 10
//float3 lightDirections[maxLights];
//float4 lightColors[maxLights];
//float lightIntensities[maxLights];
//float lightsCount;

float3 DiffuseLightDirection = float3(1, 0, 0);
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 1.0;


texture diffuseTexture;
sampler2D diffuseSampler = sampler_state{
	Texture = (diffuseTexture);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

int useBump;
texture normalMap;
sampler2D bumpSampler = sampler_state{
	Texture = (normalMap);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

//int useSpecular;
//texture specularTexture;
//sampler2D specularSampler = sampler_state{
//	Texture = (specularTexture);
//	MinFilter = Linear;
//	MagFilter = Linear;
//	AddressU = Clamp;
//	AddressV = Clamp;
//};


struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
	float3 Normal : NORMAL0;
	//float3 Tangent : TANGENT0;
	//float3 Binormal : BINORMAL0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TextureCoordinate :TEXCOORD0;
	float3 Normal :TEXCOORD1;
	//float3 Tangent : TEXCOORD2;
	//float3 Binormal : TEXCOORD3;
};

VertexShaderOutput VS_PhongBlinn(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, world);
    float4 viewPosition = mul(worldPosition, view);
    output.Position = mul(viewPosition, projection);

	output.Normal = normalize(mul(input.Normal, worldInverseTranspose));
	//output.Tangent = normalize(mul(input.Tangent, worldInverseTranspose));
	//output.Binormal = normalize(mul(input.Binormal, worldInverseTranspose));

	output.TextureCoordinate = input.TextureCoordinate;

    return output;
}

float4 PS_PhongBlinn(VertexShaderOutput input) : COLOR0
{
	//float3 bump = useBump * (tex2D(bumpSampler, input.TextureCoordinate) - (0.5, 0.5, 0.5));
	//float3 bumpNormal = input.Normal + (bump.x * input.Tangent + bump.y * input.Binormal);

	//float diffuseIntensity = dot(normalize(DiffuseLightDirection), normalize(bumpNormal));
	float diffuseIntensity = normalize(DiffuseLightDirection);
	float4 textureColor = tex2D(diffuseSampler, input.TextureCoordinate);
	textureColor.a = 1;

	return saturate(textureColor * (diffuseIntensity));
}

technique PhongBlinn
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VS_PhongBlinn();
        PixelShader = compile ps_2_0  PS_PhongBlinn();
    }
}
