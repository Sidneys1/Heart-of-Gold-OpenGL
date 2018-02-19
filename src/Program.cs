﻿using System;
using OpenGL;
using Glfw3;

namespace SfmlTest
{
    class Program
    {
        static uint LoadShaders(string vertex_file_path, string fragment_file_path)
        {
            System.Diagnostics.Trace.TraceInformation("Beginning loading shaders '{0}' and '{1}'...", vertex_file_path, fragment_file_path);
            uint VertexShaderID = Gl.CreateShader(ShaderType.VertexShader);
            uint FragmentShaderID = Gl.CreateShader(ShaderType.FragmentShader);

            string VertexShaderCode = System.IO.File.ReadAllText(vertex_file_path);
            string FragmentShaderCode = System.IO.File.ReadAllText(fragment_file_path);

            Gl.ShaderSource(VertexShaderID, new string[] { VertexShaderCode });
            Gl.CompileShader(VertexShaderID);
            int result, logLength;
            Gl.GetShader(VertexShaderID, ShaderParameterName.CompileStatus, out result);
            Gl.GetShader(VertexShaderID, ShaderParameterName.InfoLogLength, out logLength);
            if (logLength > 0)
            {
                var sb = new System.Text.StringBuilder(logLength + 1);
                Gl.GetShaderInfoLog(VertexShaderID, logLength, out logLength, sb);
                System.Diagnostics.Trace.TraceError("While compiling Vertex shader: '{0}'", sb.ToString());
            }

            Gl.ShaderSource(FragmentShaderID, new string[] { FragmentShaderCode });
            Gl.CompileShader(FragmentShaderID);
            Gl.GetShader(FragmentShaderID, ShaderParameterName.CompileStatus, out result);
            Gl.GetShader(FragmentShaderID, ShaderParameterName.InfoLogLength, out logLength);
            if (logLength > 0)
            {
                var sb = new System.Text.StringBuilder(logLength + 1);
                Gl.GetShaderInfoLog(FragmentShaderID, logLength, out logLength, sb);
                System.Diagnostics.Trace.TraceError("While compiling Fragment shader: '{0}'", sb.ToString());
            }

            uint ProgramID = Gl.CreateProgram();
            Gl.AttachShader(ProgramID, VertexShaderID);
            Gl.AttachShader(ProgramID, FragmentShaderID);
            Gl.LinkProgram(ProgramID);
            Gl.GetProgram(ProgramID, ProgramProperty.LinkStatus, out result);
            Gl.GetProgram(ProgramID, ProgramProperty.InfoLogLength, out logLength);
            if (logLength > 0)
            {
                var sb = new System.Text.StringBuilder(logLength + 1);
                Gl.GetProgramInfoLog(ProgramID, logLength, out logLength, sb);
                System.Diagnostics.Trace.TraceError("While compiling shader program: '{0}'", sb.ToString());
            }

            Gl.DetachShader(ProgramID, VertexShaderID);
            Gl.DetachShader(ProgramID, FragmentShaderID);

            Gl.DeleteShader(VertexShaderID);
            Gl.DeleteShader(FragmentShaderID);

            System.Diagnostics.Trace.TraceInformation("Finished compiling shaders.");

            return ProgramID;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            if (!Glfw3.Glfw.Init())
                Environment.Exit(-1);

            Glfw.WindowHint(Glfw.Hint.Resizable, false);
            Glfw.WindowHint(Glfw.Hint.Doublebuffer, true);
            Glfw.WindowHint(Glfw.Hint.Samples, 4);
            var window = Glfw.CreateWindow(1280, 720, "Hello World!");
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
                var data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, data.Width, data.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                image.UnlockBits(data);
                Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, Gl.NEAREST);
                Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, Gl.NEAREST);
                // Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.GenerateMipmap, Gl.NEAREST_MIPMAP_NEAREST);
                // Gl.GenerateMipmap(TextureTarget.Texture2d);
            }

            uint programID = LoadShaders(".\\src\\shaders\\SimpleVertexShader.vert", ".\\src\\shaders\\SimpleFragmentShader.frag");
            Gl.UseProgram(programID);

            uint posAttrib = (uint)Gl.GetAttribLocation(programID, "position");
            Gl.EnableVertexAttribArray(posAttrib);
            Gl.VertexAttribPointer(posAttrib, 3, VertexAttribType.Float, false, 8 * 4, IntPtr.Zero);
            uint colAttrib = (uint)Gl.GetAttribLocation(programID, "color");
            Gl.EnableVertexAttribArray(colAttrib);
            Gl.VertexAttribPointer(colAttrib, 3, VertexAttribType.Float, false, 8 * 4, new IntPtr(3 * 4));
            uint texAttrib = (uint)Gl.GetAttribLocation(programID, "texcoord");
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

            Gl.DeleteProgram(programID);
            Gl.DeleteBuffers(buffer);
            Gl.DeleteVertexArrays(vao);

            Glfw.Terminate();
        }
    }
}
