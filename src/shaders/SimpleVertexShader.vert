#version 330 core

in vec2 texcoord;
in vec3 position;
in vec3 color;

out vec3 Color;
out vec2 Texcoord;

void main() {
    Texcoord = texcoord;
    Color = color;
    gl_Position.xyz = position;
    gl_Position.w = 1.0;
}