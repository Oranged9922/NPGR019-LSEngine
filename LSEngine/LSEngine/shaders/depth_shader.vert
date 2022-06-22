#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 3) in vec3 vOffset;

uniform mat4 lightSpaceMatrix;
uniform mat4 model;

void main()
{
    gl_Position = lightSpaceMatrix * model * vec4(aPos + vOffset, 1.0);
}  