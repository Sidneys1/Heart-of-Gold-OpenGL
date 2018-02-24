using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenGL;

namespace SfmlTest {
    internal class ShaderProgram : IDisposable {
        public readonly uint ID;

        private Dictionary<string, int> _uniforms = new Dictionary<string, int>();
        private Dictionary<string, int> _attributes = new Dictionary<string, int>();

        public readonly IReadOnlyDictionary<string, int> Uniforms;
        public readonly IReadOnlyDictionary<string, int> Attributes;

        public ShaderProgram(Shader[] shaders, string[] uniformNames, string[] attributeNames) {
            Uniforms = _uniforms;
            Attributes = _attributes;

            ID = Gl.CreateProgram();
            foreach (var shader in shaders) {
                Gl.AttachShader(ID, shader.ID);
            }
            Gl.LinkProgram(ID);

            int linked;
            Gl.GetProgram(ID, ProgramProperty.LinkStatus, out linked);

            if (linked == 0) {
                const int maxLogLength = 1024;
                var infoLog = new System.Text.StringBuilder(maxLogLength);
                int infoLogLength;
                Gl.GetProgramInfoLog(ID, maxLogLength, out infoLogLength, infoLog);
                string message = $"Unable to link program: {infoLog}";
                Trace.TraceError(message);
                throw new InvalidOperationException(message);
            }

            foreach (var shader in shaders) {
                Gl.DetachShader(ID, shader.ID);
            }

            foreach (var uniformName in uniformNames) {
                int uniformId = Gl.GetUniformLocation(ID, uniformName);
                if (uniformId < 0) {
                    string message = $"Could not locate uniform {uniformName}";
                    Trace.TraceError(message);
                    throw new InvalidOperationException(message);
                }
                _uniforms.Add(uniformName, uniformId);
            }

            foreach (var attributeName in attributeNames) {
                int attributeId = Gl.GetAttribLocation(ID, attributeName);
                if (attributeId < 0) {
                    string message = $"Could not locate attribute '{attributeName}'";
                    Trace.TraceError(message);
                    throw new InvalidOperationException(message);
                }
                _attributes.Add(attributeName, attributeId);
            }
        }

        public void Use() {
            Gl.UseProgram(ID);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects).
                }
                Gl.DeleteProgram(ID);
                disposedValue = true;
            }
        }

        ~ShaderProgram() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}