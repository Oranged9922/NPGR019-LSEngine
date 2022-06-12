#version 460

in vec3 vPosition;
in vec3 vNormal;
in vec2 vTexCoords;

uniform mat4 uProjection;
uniform mat4 uModel;
uniform mat4 uView;


out vec3 fNormal;
out vec3 vPos;
out vec2 fTexCoords;

void
main()
{
    gl_Position = uProjection * uView * uModel * vec4(vPosition, 1.0);
    vPos = vec3(uModel * vec4(vPosition, 1.0));
    
    fNormal = mat3(transpose(inverse(uModel))) * vNormal;

    fTexCoords = vTexCoords;
}