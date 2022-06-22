#version 460

layout (location = 0) in vec3 vPosition;
layout (location = 1) in vec3 vNormal;
layout (location = 2) in vec2 vTexCoords;
layout (location = 3) in vec3 vOffset;

uniform mat4 uProjection;
uniform mat4 uModel;
uniform mat4 uView;

out VS_OUT {
    vec3 Normal;
    vec3 FragPos;
    vec2 TexCoords;
} vs_out;

void
main()
{	
    
	vs_out.FragPos = vec3(uModel * vec4(vPosition + vOffset, 1.0));
	
    vs_out.Normal = mat3(transpose(inverse(uModel))) * vNormal;

    vs_out.TexCoords = vTexCoords;
    gl_Position = uProjection * uView * uModel * vec4(vs_out.FragPos, 1.0);
	
}