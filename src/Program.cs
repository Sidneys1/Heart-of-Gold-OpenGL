using System;
using System.Diagnostics;
using OpenGL;
using Glfw3;

namespace SfmlTest {
    internal class Program {
        private static void Main() {
            if (!Glfw.Init())
                Environment.Exit(-1);

            Glfw.WindowHint(Glfw.Hint.Resizable, false);
            Glfw.WindowHint(Glfw.Hint.Doublebuffer, true);
            Glfw.WindowHint(Glfw.Hint.Samples, 4);
            Glfw.Window window = Glfw.CreateWindow(1280, 720, "Hello World!");

            if (!window) {
                Glfw.Terminate();
                Environment.Exit(-1);
            }

            Gl.Initialize();
            Glfw.MakeContextCurrent(window);

            Gl.Enable(EnableCap.Multisample);
            Gl.Enable(EnableCap.Texture2d);
            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            Gl.Enable(EnableCap.Blend);

            System.Diagnostics.Trace.TraceInformation($"Renderer: {Gl.GetString(StringName.Renderer)}");
            System.Diagnostics.Trace.TraceInformation($"Version:  {Gl.GetString(StringName.Version)}");

            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);

            float[] points = {
                // Pos              color           tex
                -0.5f, 0.5f, 0f,    1f, 1f, 1f,     0f, 0f,
                0.5f, 0.5f, 0f,     1f, 1f, 1f,     1f, 0f,
                0.5f, -0.5f, 0f,    1f, 1f, 1f,     1f, 1f,
                -0.5f, -0.5f, 0f,   1f, 1f, 1f,     0f, 1f,

                0f, 0f, 0f,    1f, 1f, 1f,     0f, 0f,
                1f, 0f, 0f,     1f, 1f, 1f,     1f, 0f,
                1f, -1f, 0f,    1f, 1f, 1f,     1f, 1f,
                0f, -1f, 0f,   1f, 1f, 1f,     0f, 1f
            };
            uint buffer = Gl.GenBuffer();
            Gl.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)points.Length * 4, points, BufferUsage.StaticDraw);

            uint[] elements = {
                0, 1, 2,
                2, 3, 0,

                4, 5, 6,
                6, 7, 4
            };
            uint ebo = Gl.GenBuffer();
            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            Gl.BufferData(BufferTarget.ElementArrayBuffer, (uint)elements.Length * 4, elements, BufferUsage.StaticDraw);

            uint texture = Gl.CreateTexture(TextureTarget.Texture2d);
            {
                Gl.BindTexture(TextureTarget.Texture2d, texture);
                using (var image = new System.Drawing.Bitmap(".\\resources\\textures\\dirt.png")) {
                    System.Drawing.Imaging.BitmapData data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, data.Width, data.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                    image.UnlockBits(data);
                }
                Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, Gl.NEAREST);
                Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, Gl.NEAREST);
            }

            ShaderProgram program;
            using (var vertexShader = Shader.FromFile(".\\src\\shaders\\SimpleVertexShader.vert", ShaderType.VertexShader))
            using (var fragmentShader = Shader.FromFile(".\\src\\shaders\\SimpleFragmentShader.frag", ShaderType.FragmentShader)) {
                program = new ShaderProgram(
                    new[] { vertexShader, fragmentShader },
                    new string[] { "model", "projection" },
                    new[] { "position", "color", "texcoord" }
                );
            }
            program.Use();

            var posAttrib = (uint)program.Attributes["position"];
            Gl.EnableVertexAttribArray(posAttrib);
            Gl.VertexAttribPointer(posAttrib, 3, VertexAttribType.Float, false, 8 * 4, IntPtr.Zero);
            var colAttrib = (uint)program.Attributes["color"];
            Gl.EnableVertexAttribArray(colAttrib);
            Gl.VertexAttribPointer(colAttrib, 3, VertexAttribType.Float, false, 8 * 4, new IntPtr(3 * 4));
            var texAttrib = (uint)program.Attributes["texcoord"];
            Gl.EnableVertexAttribArray(texAttrib);
            Gl.VertexAttribPointer(texAttrib, 2, VertexAttribType.Float, false, 8 * 4, new IntPtr(6 * 4));

            // var modelView = Matrix4x4f.Translated(0f, 0f, 0f);
            // var projection = Matrix4x4d.Ortho2D(-1f, 1f, -1f, 1f);
            // var modelAttrib = Gl.GetAttribLocation(programId, "model");

            //var viewAttrib = Gl.GetAttribLocation(programId, "view");
            //Gl.UniformMatrix4f(viewAttrib, 1, false, ref view);

            // var projectionAttrib = Gl.GetAttribLocation(programId, "projection");
            // Gl.UniformMatrix4f(projectionAttrib, 1, false, projection);

            while (!Glfw.WindowShouldClose(window)) {
                Gl.ClearColor(1.0f, 0.0f, 1.0f, 1.0f);
                Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                // Gl.UniformMatrix4f(modelAttrib, 1, false, projection * modelView);
                Gl.DrawElements(PrimitiveType.Triangles, elements.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);

                Glfw.SwapBuffers(window);

                Glfw.PollEvents();
                if (Glfw.GetKey(window, (int)Glfw.KeyCode.Escape)) {
                    Glfw.SetWindowShouldClose(window, true);
                }
            }

            Console.WriteLine("Exiting...");

            Gl.DeleteBuffers(buffer);
            Gl.DeleteVertexArrays(vao);

            Glfw.Terminate();
        }
    }
}
