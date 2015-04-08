float4x4 world;
float4x4 view;
float4x4 projection;

float4x4 worldInverseTranspose;

float3 direction;

#define maxLights 30
float4 lightsPositions[maxLights];
float4 lightsColors[maxLights];
float lightsAttenuations[maxLights];
float lightsFalloffs[maxLights];
int lightsPoint[maxLights];
int lightsDirectional[maxLights];
int lightsCount = 0;

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

float4 Pos = float4(0, 0, 0, 0);


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
	float4 WorldPos : TEXCOORD4;
};

VertexShaderOutput VS(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, world);
    float4 viewPosition = mul(worldPosition, view);
    output.Position = mul(viewPosition, projection);
	output.WorldPos = worldPosition;

	output.Normal = normalize(mul(input.Normal, worldInverseTranspose));
	output.Tangent = normalize(mul(input.Tangent, worldInverseTranspose));
	output.Binormal = normalize(mul(input.Binormal, worldInverseTranspose));

	output.TextureCoordinate = input.TextureCoordinate;

    return output;
}

float4 PS(VertexShaderOutput input) : COLOR0
{

	//Normal calc
	float3 normalMap = (tex2D(normalSampler, input.TextureCoordinate) - (0.5, 0.5, 0.5));
	float3 normal = normalIntensity * (input.Normal + (normalMap.x * input.Tangent + normalMap.y * input.Binormal));
	//Lights
	float4 totalLight = float4(0, 0, 0, 1);
	//Specular
	float4 totalSpecular = float4(0, 0, 0, 1);

	for (int i = 0; i < lightsCount; ++i)
	{
		//directional
		float4 lightDir = normalize((lightsPositions[i] - input.WorldPos) * lightsPoint[i] + lightsPositions[i] * lightsDirectional[i]); // same as dLight
		float dirLightAffect = saturate(dot(lightDir, normal)) * lightsAttenuations[i]; //same as diffuseIntensity
		//point
		float d = distance(lightsPositions[i], input.WorldPos);
		float att = 1 - pow(clamp(d / lightsAttenuations[i], 0, 1),lightsFalloffs[i]);

		totalLight += lightsColors[i] * ( lightsPoint[i] * att + dirLightAffect * lightsDirectional[i]);

		float3 r = normalize((2 * dot(lightDir, normal) * normal - lightDir));
		float3 v = normalize(mul(normalize(direction), world));
		totalSpecular += dot(r, v);
 	}
	totalSpecular = (tex2D(specularSampler, input.TextureCoordinate)) * specularIntensity
		* specularColor * totalSpecular * totalLight;

	//Emissive
	float4 emissive = (tex2D(emissiveSampler, input.TextureCoordinate).rgba * emissiveIntensity) * emissiveColor;
	emissive.a = 1.0f;

	//Texture color
	float4 textureColor = tex2D(diffuseSampler, input.TextureCoordinate);
	textureColor.a = 1;

	return saturate((textureColor * /*diffuseIntensity  * */ totalLight)/* + specular*/) + emissive;
}

technique PhongBlinn
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS();
    }
}
