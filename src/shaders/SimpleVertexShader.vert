#version 330 core

in vec2 texcoord;
in vec3 position;
in vec3 color;

uniform mat4 model;
//uniform mat4 view;
uniform mat4 projection;

out vec3 Color;
out vec2 Texcoord;

void main() {
    Texcoord = texcoord;
    Color = color;
	gl_Position = /* projection * model * */ vec4(position, 1.0);
}