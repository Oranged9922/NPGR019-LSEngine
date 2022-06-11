using Silk.NET.OpenGL;
using System.Numerics;

namespace LSEngine
{
    internal class Shader : IDisposable
    {
        private uint _id;
        private GL _gl;

        public Shader(GL gl, string vertexShaderPath, string fragmentShaderPath)
        {
            _gl = gl;
            uint vertexShader = LoadShader(vertexShaderPath, ShaderType.VertexShader);
            uint fragmentShader = LoadShader(fragmentShaderPath, ShaderType.FragmentShader);
            _id = gl.CreateProgram();
            gl.AttachShader(_id, vertexShader);
            gl.AttachShader(_id, fragmentShader);
            gl.LinkProgram(_id);
            // check for errors
            _gl.GetProgram(_id, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                throw new Exception($"Error linking shader program: {_gl.GetProgramInfoLog(_id)}");
            }
            _gl.DetachShader(_id, vertexShader);
            _gl.DetachShader(_id, fragmentShader);
            _gl.DeleteShader(vertexShader);
            _gl.DeleteShader(fragmentShader);
        }

        public void Use()
        {
            _gl.UseProgram(_id);
        }

        public void SetUniform(string name, int value)
        {
            int location = _gl.GetUniformLocation(_id, name);
            if (location == -1)
            {
                throw new Exception($"Uniform {name} not found");
            }
            _gl.Uniform1(location, value);
        }

        public void SetUniform(string name, float value)
        {
            int location = _gl.GetUniformLocation(_id, name);
            if (location == -1)
            {
                throw new Exception($"Uniform {name} not found");
            }
            _gl.Uniform1(location, value);
        }

        public void SetUniform(string name, Vector3 value)
        {
            int location = _gl.GetUniformLocation(_id, name);
            if (location == -1)
            {
                throw new Exception($"Uniform {name} not found");
            }
            _gl.Uniform3(location, value.X, value.Y, value.Z);
        }
        // set matrix4x4
        public unsafe void SetUniform(string name, Matrix4x4 value)
        {
            int location = _gl.GetUniformLocation(_id, name);
            if (location == -1)
            {
                throw new Exception($"Uniform {name} not found");
            }
            _gl.UniformMatrix4(location, 1, false, (float*)&value);
        }

        public void Dispose()
        {
            _gl.DeleteProgram(_id);
        }

        private uint LoadShader(string path, ShaderType type)
        {
            uint id = _gl.CreateShader(type);
            _gl.ShaderSource(id, File.ReadAllText(path));
            _gl.CompileShader(id);
            string infoLog = _gl.GetShaderInfoLog(id);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
            }
            return id;
        }
    }
}