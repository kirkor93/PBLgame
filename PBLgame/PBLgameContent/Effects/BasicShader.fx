﻿float4x4 world;
float4x4 view;
float4x4 projection;
float4x4 worldInverseTranspose;
float3 camDirection;

#define maxLights 9
float3 lightsPositions[maxLights];
float4 lightsColors[maxLights];
float lightsAttenuations[maxLights];
float lightsFalloffs[maxLights];
int lightsPoint[maxLights];
int lightsDirectional[maxLights];
//int lightsCount = 0;


#ifdef SKINNED
float4x3 Bones[72];
#endif

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
	float3 Normal   : NORMAL0;
	float3 Tangent  : TANGENT0;
	float3 Binormal : BINORMAL0;
#ifdef SKINNED
	int4   Indices  : BLENDINDICES0;
	float4 Weights  : BLENDWEIGHT0;
#endif
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
	float3 invNormal   : TEXCOORD1;
	float3 invTangent  : TEXCOORD2;
	float3 invBinormal : TEXCOORD3;
	float4 WorldPos : TEXCOORD4;
	float3 worldedNormal   : TEXCOORD5;
	float3 worldedTangent  : TEXCOORD6;
	float3 worldedBinormal : TEXCOORD7;
};

#ifdef SKINNED
void Skin(inout VertexShaderInput vin, uniform int boneCount)
{
	float4x3 skinning = 0;

	[unroll]
	for (int i = 0; i < boneCount; i++)
	{
		skinning += Bones[vin.Indices[i]] * vin.Weights[i];
	}

	vin.Position.xyz = mul(vin.Position, skinning);
	vin.Normal = mul(vin.Normal, (float3x3) skinning);
}
#endif

VertexShaderOutput VS(VertexShaderInput input)
{
	VertexShaderOutput output;

#ifdef SKINNED
	Skin(input, 4);
#endif

	float4 worldPosition = mul(input.Position, world);
	float4 viewPosition = mul(worldPosition, view);
	output.Position = mul(viewPosition, projection);
	output.WorldPos = worldPosition;

	output.invNormal   = normalize(mul(input.Normal,   worldInverseTranspose));
	output.invTangent  = normalize(mul(input.Tangent,  worldInverseTranspose));
	output.invBinormal = normalize(mul(input.Binormal, worldInverseTranspose));

	output.worldedNormal   = normalize(mul(input.Normal,   world));
	output.worldedTangent  = normalize(mul(input.Tangent,  world));
	output.worldedBinormal = normalize(mul(input.Binormal, world));

	output.TextureCoordinate = input.TextureCoordinate;

	return output;
}

float4 PS(VertexShaderOutput input) : COLOR0
{

	//Normal calc
	float3 normalMap = (tex2D(normalSampler, input.TextureCoordinate) - (0.5, 0.5, 0.5));

	float3 worldedNormal = normalIntensity * (input.worldedNormal + (normalMap.x * input.worldedTangent + normalMap.y * input.worldedBinormal));

	float3 invNormal = normalIntensity * (input.invNormal + (normalMap.x * input.invTangent + normalMap.y * input.invBinormal));

	//Lights
	float4 totalLight = float4(0, 0, 0, 1);
	//Specular
	float4 totalSpecular = float4(0, 0, 0, 1);

	[unroll]
	for (int i = 0; i < maxLights; ++i)
	{
		//directional light
		float3 directionalLight = (normalize(lightsPositions[i]) * lightsDirectional[i]) + (normalize(lightsPositions[i] - input.WorldPos) * lightsPoint[i]);
		float dirLight = saturate(dot(directionalLight, worldedNormal)) * lightsAttenuations[i];

		//point
		float d = distance(lightsPositions[i], input.WorldPos);
		float att = 1 - pow(clamp(d / lightsAttenuations[i], 0, 1), lightsFalloffs[i]);

		totalLight += lightsColors[i] * (dirLight * lightsDirectional[i] + att * lightsPoint[i]);

		float3 r = normalize((2 * dot(directionalLight, invNormal) * invNormal - directionalLight));
		float3 v = normalize(mul(normalize(camDirection), world));
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
	
	float4 ambient = 1.0f * (1, 1, 1, 1);
	
	return (textureColor * totalLight /*+ ambient*/ + totalSpecular) + emissive;
}

technique PhongBlinn
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VS();
		PixelShader = compile ps_3_0 PS();
	}
}
