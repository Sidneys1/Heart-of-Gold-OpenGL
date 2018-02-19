#version 330 core

in vec3 position;
in vec3 color;

out vec3 Color;

void main() {
    Color = color;
    gl_Position.xyz = position;
    gl_Position.w = 1.0;
}