using System;
using System.Diagnostics;
using OpenGL;

namespace SfmlTest {
    internal class Shader : IDisposable {
        public readonly uint ID;
        public readonly ShaderType Type;

        public Shader(string source, ShaderType type) {
            Type = type;
            ID = Gl.CreateShader(type);
            Gl.ShaderSource(ID, new[] { source });
            Gl.CompileShader(ID);
            int compiled;
            Gl.GetShader(ID, ShaderParameterName.CompileStatus, out compiled);
            if (compiled != 0) {
                Trace.TraceInformation("Compiled {0} successfully.", type);
                return;
            }

            const int logMaxLength = 1024;
            var infoLog = new System.Text.StringBuilder(logMaxLength);
            int infoLogLength;
            Gl.GetShaderInfoLog(ID, logMaxLength, out infoLogLength, infoLog);

            string message = $"Unable to compile shader: {infoLog}";
            Trace.TraceError(message);
            throw new InvalidOperationException(message);
        }

        public static Shader FromFile(string path, ShaderType type) {
            System.Diagnostics.Trace.TraceInformation("Loading {0} from file '{1}'.", type, path);
            string source;
            try {
                source = System.IO.File.ReadAllText(path);
            } catch (System.IO.FileNotFoundException ex) {
                Trace.TraceError("File did not exist. {0}", ex.Message);
                return null;
            } catch (System.IO.FileLoadException ex) {
                Trace.TraceError("Could not load contents of file. {0}", ex.Message);
                return null;
            }
            return new Shader(source, type);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects).
                }
                Gl.DeleteShader(ID);
                disposedValue = true;
            }
        }

        ~Shader() {
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