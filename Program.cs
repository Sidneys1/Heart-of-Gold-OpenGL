using System;
using OpenGL;
// using Glfw3;

namespace SfmlTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            if (!Glfw3.Glfw.Init())
                Environment.Exit(-1);

            var window = Glfw3.Glfw.CreateWindow(1280, 720, "Hello World!");
            if (!window) {
                Glfw3.Glfw.Terminate();
                Environment.Exit(-1);
            }

            Glfw3.Glfw.MakeContextCurrent(window);

            Console.WriteLine($"Renderer: {Gl.GetString(StringName.Renderer)}");
            Console.WriteLine($"Version:  {Gl.GetString(StringName.Version)}");

            float[] points = {
                0.0f, 0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,
                -0.5f, -0.5f, 0.0f
            };
            uint buffer = Gl.GenBuffer();
            Gl.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            Gl.BufferData(BufferTarget.ArrayBuffer, 9 * sizeof(float), points, BufferUsage.StaticDraw);

            Gl.ClearColor(1.0f, 0.0f, 1.0f, 1.0f);

            while (!Glfw3.Glfw.WindowShouldClose(window)) {
                Gl.Clear(ClearBufferMask.ColorBufferBit);

                Gl.BindBuffer(BufferTarget.ArrayBuffer, buffer);
                Gl.VertexPointer(3, VertexPointerType.Float, sizeof(float), new IntPtr(0));
                Gl.DrawArrays(PrimitiveType.Triangles, 0, 3);

                Glfw3.Glfw.SwapBuffers(window);

                Glfw3.Glfw.PollEvents();
            }

            Console.WriteLine("Exiting...");

            Glfw3.Glfw.Terminate();
        }
    }
}
