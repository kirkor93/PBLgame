float4x4 world;
float4x4 view;
float4x4 projection;

float4x4 worldInverseTranspose;

float3 direction;

//#define maxLights 10
//float3 lightDirections[maxLights];
//float4 lightColors[maxLights];
//float lightIntensities[maxLights];
//float lightsCount;

float3 DiffuseLightDirection = float3(1, 1, 0);
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

float normalIntensity;
texture normalMap;
sampler2D normalSampler = sampler_state{
	Texture = (normalMap);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

float shininess = 200;
float4 specularColor = float4(1, 1, 1, 1);

float specularIntensity = 1;
texture specularTexture;
sampler2D specularSampler = sampler_state{
	Texture = (specularTexture);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

float emissiveIntensity;
float4 emissiveColor = float4(1, 1, 1, 1);
texture emissiveTexture;
sampler2D emissiveSampler = sampler_state{
	Texture = (emissiveTexture);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};


struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
	float3 Normal : NORMAL0;
	float3 Tangent : TANGENT0;
	float3 Binormal : BINORMAL0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TextureCoordinate :TEXCOORD0;
	float3 Normal :TEXCOORD1;
	float3 Tangent : TEXCOORD2;
	float3 Binormal : TEXCOORD3;
};

VertexShaderOutput VS(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, world);
    float4 viewPosition = mul(worldPosition, view);
    output.Position = mul(viewPosition, projection);

	output.Normal = normalize(mul(input.Normal, worldInverseTranspose));
	output.Tangent = normalize(mul(input.Tangent, worldInverseTranspose));
	output.Binormal = normalize(mul(input.Binormal, worldInverseTranspose));

	output.TextureCoordinate = input.TextureCoordinate;

    return output;
}

float4 PS(VertexShaderOutput input) : COLOR0
{
	float3 dLight = normalize(DiffuseLightDirection);

	//Normal calc
	float3 normalMap = (tex2D(normalSampler, input.TextureCoordinate) - (0.5, 0.5, 0.5));
	float3 normal = normalIntensity * (input.Normal + (normalMap.x * input.Tangent + normalMap.y * input.Binormal));
	//Diffuse light with normals 
	float diffuseIntensity = dot(dLight, normalize(normal));
	//Specular
	float3 r = normalize(2 * dot(dLight, normal) * normal - dLight);
	float3 v = normalize(mul(normalize(direction), world));
	float dotProduct = dot(r, v);

	float4 specular = (tex2D(specularSampler, input.TextureCoordinate) *  specularIntensity 
						* specularColor * max(pow(dotProduct, shininess), 0) * diffuseIntensity);

	float4 emissive = (tex2D(emissiveSampler, input.TextureCoordinate) * emissiveIntensity) * emissiveColor;

	//Texture color
	float4 textureColor = tex2D(diffuseSampler, input.TextureCoordinate);
	textureColor.a = 1;

	return saturate(textureColor * (diffuseIntensity) + specular) + emissive;
}

technique PhongBlinn
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VS();
        PixelShader = compile ps_2_0  PS();
    }
}
