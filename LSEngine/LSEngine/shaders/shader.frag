#version 330 core

uniform vec3 color;
out vec4 FragColor;
void main()
{
      FragColor = vec4(color.rgb,1.0f);
}