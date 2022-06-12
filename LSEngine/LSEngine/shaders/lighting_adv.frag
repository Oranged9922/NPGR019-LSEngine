#version 330 core

// Holds information about a light
struct Light {
	vec3 position;
	vec3 color;
	float ambientIntensity;
	float diffuseIntensity;

	int type;
	vec3 direction;
	float coneAngle;

	float linearAttenuation;
	float quadraticAttenuation;
	//float radius;
};

in vec3 fNormal;
in vec3 vPos;
in vec2 fTexCoords;
out vec4 outputColor;


// Texture info
uniform sampler2D mainTexture;
uniform bool hasSpecularMap;
uniform sampler2D mapSpecular;

uniform mat4 view;

// Material info
uniform vec3 materialAmbient;
uniform vec3 materialDiffuse;
uniform vec3 materialSpecular;
uniform float materialSpecExponent;

// Array of lights
uniform Light lights[5];


void main()
{
	outputColor = vec4(0.0, 0.0, 0.0, 1.0);
	
	// Texture info
	vec2 flipped_texcoord = vec2(fTexCoords.x, 1.0 - fTexCoords.y);
	vec4 texel = texture2D(mainTexture, flipped_texcoord.xy);
	
	if(texel.a < 0.5) discard;
	
	vec3 n = normalize(fNormal);
	
	// Loop through all lights, adding the lighitng contribution to the final color
	for(int i = 0; i < 5; i++)
	{
		// skip lights with no effect
		if(lights[i].color == vec3(0,0,0)) continue;
		
		vec3 lightVec = normalize(lights[i].position - vPos);
		vec4 lightColor = vec4(0,0,0,1);

		// check spotlight angle
		bool inCone = false;
		if(lights[i].type == 1 && degrees(acos(dot(lightVec,lights[i].direction))) < lights[i].coneAngle)
		{
			inCone = true;
		}

		// Directional lighting
		if(lights[i].type == 2)
		{
			lightVec = lights[i].direction;
		}

		// Colors
		vec4 lightAmbient = lights[i].ambientIntensity * vec4(lights[i].color, 1.0);
		vec4 lightDiffuse = lights[i].diffuseIntensity * vec4(lights[i].color, 1.0);

		// ambient lighting
		lightColor = lightColor + texel * lightAmbient * vec4(materialAmbient,0.0);

		// diffuse lighting
		float diffuseFactor = max(dot(n, lightVec), 0.0);

		// spotlight, limit to cone
		if(lights[i].type != 1 || inCone)
		{
			lightColor = lightColor + (texel * lightDiffuse * vec4(materialDiffuse,0.0)) * diffuseFactor;
		}

		// specular lighting
		vec3 reflectionVector = normalize(reflect(-lightVec, n));
		vec3 viewVector = normalize(vec3(inverse(view) * vec4(0,0,0,1)) - vPos);
		float materialSpecularReflection = max(dot(fNormal, lightVec),0.0) * pow(max(dot(reflectionVector, viewVector), 0.0), materialSpecExponent);
		
		// if has specular map
		if(hasSpecularMap)
		{
			materialSpecularReflection = materialSpecularReflection * texture2D(mapSpecular, flipped_texcoord.xy).r;
		}

		// spotlight, specular reflections limited to angle
		if(lights[i].type != 1 || inCone)
		{
			lightColor = lightColor + vec4(materialSpecular * lights[i].color, 0.0) * materialSpecularReflection;
		}

		// Attenuation
		float distanceFactor = distance(lights[i].position, vPos);
		float attenuation = 1.0 / (1.0 + (distanceFactor * lights[i].linearAttenuation) + (distanceFactor * distanceFactor * lights[i].quadraticAttenuation));
		
		outputColor = outputColor + lightColor * attenuation;
	}
}