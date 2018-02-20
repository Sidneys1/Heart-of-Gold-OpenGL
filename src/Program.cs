using System;
using OpenGL;
using Glfw3;

namespace SfmlTest
{
    internal class Program
    {
        private static uint LoadShaders(string vertexFilePath, string fragmentFilePath)
        {
            System.Diagnostics.Trace.TraceInformation("Beginning loading shaders '{0}' and '{1}'...", vertexFilePath, fragmentFilePath);
            uint vertexShaderId = Gl.CreateShader(ShaderType.VertexShader);
            uint fragmentShaderId = Gl.CreateShader(ShaderType.FragmentShader);

            string vertexShaderCode = System.IO.File.ReadAllText(vertexFilePath);
            string fragmentShaderCode = System.IO.File.ReadAllText(fragmentFilePath);

            Gl.ShaderSource(vertexShaderId, new[] { vertexShaderCode });
            Gl.CompileShader(vertexShaderId);
            Gl.GetShader(vertexShaderId, ShaderParameterName.CompileStatus, out _);
            Gl.GetShader(vertexShaderId, ShaderParameterName.InfoLogLength, out int logLength);
            if (logLength > 0)
            {
                var sb = new System.Text.StringBuilder(logLength + 1);
                Gl.GetShaderInfoLog(vertexShaderId, logLength, out logLength, sb);
                System.Diagnostics.Trace.TraceError("While compiling Vertex shader: '{0}'", sb.ToString());
            }

            Gl.ShaderSource(fragmentShaderId, new[] { fragmentShaderCode });
            Gl.CompileShader(fragmentShaderId);
            Gl.GetShader(fragmentShaderId, ShaderParameterName.CompileStatus, out _);
            Gl.GetShader(fragmentShaderId, ShaderParameterName.InfoLogLength, out logLength);
            if (logLength > 0)
            {
                var sb = new System.Text.StringBuilder(logLength + 1);
                Gl.GetShaderInfoLog(fragmentShaderId, logLength, out logLength, sb);
                System.Diagnostics.Trace.TraceError("While compiling Fragment shader: '{0}'", sb.ToString());
            }

            uint programId = Gl.CreateProgram();
            Gl.AttachShader(programId, vertexShaderId);
            Gl.AttachShader(programId, fragmentShaderId);
            Gl.LinkProgram(programId);
            Gl.GetProgram(programId, ProgramProperty.LinkStatus, out _);
            Gl.GetProgram(programId, ProgramProperty.InfoLogLength, out logLength);
            if (logLength > 0)
            {
                var sb = new System.Text.StringBuilder(logLength + 1);
                Gl.GetProgramInfoLog(programId, logLength, out logLength, sb);
                System.Diagnostics.Trace.TraceError("While compiling shader program: '{0}'", sb.ToString());
            }

            Gl.DetachShader(programId, vertexShaderId);
            Gl.DetachShader(programId, fragmentShaderId);

            Gl.DeleteShader(vertexShaderId);
            Gl.DeleteShader(fragmentShaderId);

            System.Diagnostics.Trace.TraceInformation("Finished compiling shaders.");

            return programId;
        }

        private static void Main()
        {
            Console.WriteLine("Hello World!");

            if (!Glfw.Init())
                Environment.Exit(-1);

            Glfw.WindowHint(Glfw.Hint.Resizable, false);
            Glfw.WindowHint(Glfw.Hint.Doublebuffer, true);
            Glfw.WindowHint(Glfw.Hint.Samples, 4);
            Glfw.Window window = Glfw.CreateWindow(1280, 720, "Hello World!");
            if (!window)
            {
                Glfw.Terminate();
                Environment.Exit(-1);
            }

            Gl.Initialize();
            Glfw.MakeContextCurrent(window);

            Gl.Enable(EnableCap.Multisample);
            Gl.Enable(EnableCap.Texture2d);
            Gl.Enable(EnableCap.Blend);

            

            System.Diagnostics.Trace.TraceInformation($"Renderer: {Gl.GetString(StringName.Renderer)}");
            System.Diagnostics.Trace.TraceInformation($"Version:  {Gl.GetString(StringName.Version)}");

            uint vao= Gl.GenVertexArray();
            Gl.BindVertexArray(vao);

            // float[] points = {
            //     // Pos              color           tex
            //     -0.5f, 0.5f, 0f,    1f, 0f, 0f,     0f, 0f,
            //     0.5f, 0.5f, 0f,     0f, 1, 0f,      1f, 0f,
            //     0.5f, -0.5f, 0f,    0f, 0f, 1f,     1f, 1f,
            //     -0.5f, -0.5f, 0f,   1f, 1f, 1f,     0f, 1f
            // };
            float[] points = {
                // Pos              color           tex
                -0.5f, 0.5f, 0f,    1f, 1f, 1f,     0f, 0f,
                0.5f, 0.5f, 0f,     1f, 1f, 1f,     1f, 0f,
                0.5f, -0.5f, 0f,    1f, 1f, 1f,     1f, 1f,
                -0.5f, -0.5f, 0f,   1f, 1f, 1f,     0f, 1f
            };
            uint buffer = Gl.GenBuffer();
            Gl.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)points.Length * 4, points, BufferUsage.StaticDraw);

            uint[] elements = {
                0, 1, 2,
                2, 3, 0
            };
            uint ebo = Gl.GenBuffer();
            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            Gl.BufferData(BufferTarget.ElementArrayBuffer, (uint)elements.Length * 4, elements, BufferUsage.StaticDraw);

            uint texture = Gl.CreateTexture(TextureTarget.Texture2d);
            {
                Gl.BindTexture(TextureTarget.Texture2d, texture);
                var image = new System.Drawing.Bitmap(".\\resources\\textures\\dirt.png");
                System.Diagnostics.Trace.TraceInformation("Image is {0}x{1}", image.Width, image.Height);
                System.Drawing.Imaging.BitmapData data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, data.Width, data.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                image.UnlockBits(data);
                Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, Gl.NEAREST);
                Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, Gl.NEAREST);
                // Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.GenerateMipmap, Gl.NEAREST_MIPMAP_NEAREST);
                // Gl.GenerateMipmap(TextureTarget.Texture2d);
            }

            uint programId = LoadShaders(".\\src\\shaders\\SimpleVertexShader.vert", ".\\src\\shaders\\SimpleFragmentShader.frag");
            Gl.UseProgram(programId);

            var posAttrib = (uint)Gl.GetAttribLocation(programId, "position");
            Gl.EnableVertexAttribArray(posAttrib);
            Gl.VertexAttribPointer(posAttrib, 3, VertexAttribType.Float, false, 8 * 4, IntPtr.Zero);
            var colAttrib = (uint)Gl.GetAttribLocation(programId, "color");
            Gl.EnableVertexAttribArray(colAttrib);
            Gl.VertexAttribPointer(colAttrib, 3, VertexAttribType.Float, false, 8 * 4, new IntPtr(3 * 4));
            var texAttrib = (uint)Gl.GetAttribLocation(programId, "texcoord");
            Gl.EnableVertexAttribArray(texAttrib);
            Gl.VertexAttribPointer(texAttrib, 2, VertexAttribType.Float, false, 8 * 4, new IntPtr(6 * 4));


            while (!Glfw.WindowShouldClose(window))
            {
                Gl.ClearColor(1.0f, 0.0f, 1.0f, 1.0f);
                Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                Gl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero);
                // Gl.DrawArrays(PrimitiveType.Triangles, 0, 3);

                Glfw.SwapBuffers(window);

                Glfw.PollEvents();
                if (Glfw.GetKey(window, (int)Glfw.KeyCode.Escape))
                {
                    Glfw.SetWindowShouldClose(window, true);
                }
            }

            Console.WriteLine("Exiting...");

            Gl.DeleteProgram(programId);
            Gl.DeleteBuffers(buffer);
            Gl.DeleteVertexArrays(vao);

            Glfw.Terminate();
        }
    }
}
