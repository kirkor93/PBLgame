float4x4 world;
float4x4 view;
float4x4 projection;
float3 cameraPosition;

#define ALL_LIGHTS 6
#define DIR_LIGHTS 2
#define POINT_LIGHTS (ALL_LIGHTS - DIR_LIGHTS)
#define lightsIntensities lightsAttenuations
#define lightsDirs		  lightsPositions

float3 lightsPositions	[ALL_LIGHTS];
float4 lightsColors		[ALL_LIGHTS];
float lightsAttenuations[ALL_LIGHTS];	// or Intensity
float lightsFalloffs	[ALL_LIGHTS];

float alphaValue = 1.0;

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

/// ==== SHADOWS DATA ==== ///

float shadowFarPlanes[ALL_LIGHTS];
float4x4 shadowViewMatrices[DIR_LIGHTS];
float4x4 shadowProjMatrices[DIR_LIGHTS];
bool hasShadows[ALL_LIGHTS];
float3 shadowLightPos;
float shadowFarPlane;

#define GenerateShadowSampler2D(x) sampler2D shadowSampler ## x = sampler_state { \
	Texture = (shadowMap ## x);	\
	MinFilter = Point;	\
	MagFilter = Point;	\
	MipFilter = Point;	\
}

#define GenerateShadowSamplerCUBE(x) samplerCUBE shadowSampler ## x = sampler_state { \
	Texture = (shadowMap ## x);	\
	MinFilter = Point;	\
	MagFilter = Point;	\
	MipFilter = Point;	\
}

texture2D shadowMap0, shadowMap1;
textureCUBE shadowMap2, shadowMap3, shadowMap4, shadowMap5;

GenerateShadowSampler2D(0);
GenerateShadowSampler2D(1);
GenerateShadowSamplerCUBE(2);
GenerateShadowSamplerCUBE(3);
GenerateShadowSamplerCUBE(4);
GenerateShadowSamplerCUBE(5);


#define lightCase(x) if (i == x) return tex2D(shadowSampler ## x, UV).r
float texDirDepth(int i, float2 UV)
{
	if (UV.x < 0 || UV.x > 1 ||
		UV.y < 0 || UV.y > 1) return 1;

	// switch not working properly, but compiler will solve those at compile-time
	lightCase(0);
	else lightCase(1);
	else return 1;

}
#undef lightCase

#define lightCase(x) if (i == x) return texCUBE(shadowSampler ## x, dir).r
float texPointDepth(int i, float3 dir)
{
	// switch not working properly, but compiler will solve those at compile-time
	lightCase(2);
	else lightCase(3);
	else lightCase(4);
	else lightCase(5);
	else return 1;

}
#undef lightCase


float2 postProjToScreen(float4 position)
{
	float2 screenPos = position.xy / position.w;
	return 0.5f * (float2(screenPos.x, -screenPos.y) + 1);
}

// 0.5f / float2(viewportWidth, viewportHeight)
float2 bias = float2(0.008f, 0.015f);

float shadowMult = 0.0f;
float shadowBias = 0.015f;

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
	float4 WorldPos        : TEXCOORD1;
	float3 worldedNormal   : TEXCOORD2;
	float3 worldedTangent  : TEXCOORD3;
	float3 worldedBinormal : TEXCOORD4;
	float3 viewDirection   : TEXCOORD5;
	float4 shadowScreenPos[DIR_LIGHTS] : TEXCOORD6;
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

	output.worldedNormal   = normalize(mul(input.Normal,   world));
	output.worldedTangent  = normalize(mul(input.Tangent,  world));
	output.worldedBinormal = normalize(mul(input.Binormal, world));

	output.TextureCoordinate = input.TextureCoordinate;
	output.viewDirection = normalize(cameraPosition.xyz - worldPosition.xyz);

	[unroll]
	for (int i = 0; i < DIR_LIGHTS; i++)
	{
		output.shadowScreenPos[i] = mul(worldPosition, mul(shadowViewMatrices[i], shadowProjMatrices[i]));
	}

	return output;
}


float4 CalcResultColor(float shadow, float intensity, float4 lightColor)
{
	return shadow * lightColor * intensity;
}

float4 CalcDiffuse(float3 lightDir, float3 normal)
{
	return saturate(dot(lightDir, normal));
}

float4 CalcSpecular(float3 lightDir, float3 normal, float3 v)
{
#ifdef PHONG
	// Phong
	float3 r = normalize((2 * dot(lightDir, normal) * normal - lightDir));
	return pow(saturate(dot(r, v)), shininess);
#else
	// Blinn-Phong
	float3 h = normalize(lightDir + v);
	return pow(saturate(dot(normal, h)), shininess);
#endif
}

float4 PS(VertexShaderOutput input) : COLOR0
{
	//Normal calc
	float3 normalMap = (tex2D(normalSampler, input.TextureCoordinate) - (0.5, 0.5, 0.5));
	float3 worldedNormal = normalize(normalIntensity * (input.worldedNormal + (normalMap.x * input.worldedTangent + normalMap.y * input.worldedBinormal)));

	//Lights
	float4 totalLight = float4(0, 0, 0, 1);
	//Specular
	float4 totalSpecular = float4(0, 0, 0, 1);

	[unroll]
	// Directional lights
	for (int i = 0; i < DIR_LIGHTS; i++)
	{
		float shadow = 1;
		if (hasShadows[i])
		{
			float2 shadowCoords = postProjToScreen(input.shadowScreenPos[i]) /*+ bias*/;
			float mapDepth = texDirDepth(i, shadowCoords);
			float realDepth = input.shadowScreenPos[i].z / input.shadowScreenPos[i].w;
			if (realDepth < 1 && realDepth - shadowBias > mapDepth) shadow = shadowMult;
			//return float4(realDepth, mapDepth, mapDepth, 1);
		}
		
		// Light direction
		float3 lightDir = normalize(lightsDirs[i]);

		// Diffuse shading
		float4 resultColor = CalcResultColor(shadow, lightsIntensities[i], lightsColors[i]);

		totalLight += resultColor * CalcDiffuse(lightDir, worldedNormal);
		totalSpecular += resultColor * CalcSpecular(lightDir, worldedNormal, input.viewDirection);
	}

	const int FIRST_POINT_INDEX = DIR_LIGHTS;

	[unroll]
	// Point lights
	for (int i = FIRST_POINT_INDEX; i < ALL_LIGHTS; i++)
	{
		float shadow = 1;
		if (hasShadows[i])
		{	
			float3 shadowDir = (lightsPositions[i].xyz - input.WorldPos);
			float mapDepth = texPointDepth(i, normalize(shadowDir));
			float realDepth = saturate( length(shadowDir) / shadowFarPlanes[i] );
			if (realDepth < 1 && realDepth - shadowBias > mapDepth) shadow = shadowMult;
			//return float4(realDepth, mapDepth, mapDepth, 1);
		}

		// Light direction
		float3 lightDir = normalize(lightsPositions[i] - input.WorldPos);
		
		// Diffuse shading
		float d = distance(lightsPositions[i], input.WorldPos);
		float4 resultColor = CalcResultColor(shadow, (1 - pow(saturate(d / lightsAttenuations[i]), lightsFalloffs[i])), lightsColors[i]);
		
		totalLight += resultColor * CalcDiffuse(lightDir, worldedNormal);
		totalSpecular += resultColor * CalcSpecular(lightDir, worldedNormal, input.viewDirection);
	}

	totalSpecular = tex2D(specularSampler, input.TextureCoordinate) * specularIntensity * specularColor * totalSpecular;

	//Emissive
	float4 emissive = (tex2D(emissiveSampler, input.TextureCoordinate).rgba * emissiveIntensity) * emissiveColor;
	emissive.a = 1.0f;

	//Texture color
	float4 textureColor = tex2D(diffuseSampler, input.TextureCoordinate);
	textureColor.a = 1;
	
	float4 ambient = 0.1f * (1, 1, 1, 0);
	totalLight += ambient;
	float4 color = (textureColor * totalLight + totalSpecular) + emissive;
	color.a = alphaValue;

	return color;
}

/// ========== SHADOWS GENERATING ========== ///

struct VertexShaderShadowsOutput
{
	float4 Position : POSITION0;
	float4 ScreenPos : TEXCOORD0;
	float4 WorldPos  : TEXCOORD1;
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
	output.WorldPos = worldPosition;

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


technique PhongBlinn
{
	pass Pass1
	{
		AlphaBlendEnable = TRUE;
		DestBlend = INVSRCALPHA;
		SrcBlend = SRCALPHA;
		VertexShader = compile vs_3_0 VS();
		PixelShader = compile ps_3_0 PS();
	}
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
