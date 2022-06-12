using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;

namespace LSEngine
{
    internal class Texture : IDisposable
    {
        private uint _id;
        private GL _gl;
        public int texSlot;

        public unsafe Texture(GL gl, string path, GLEnum clamp = GLEnum.ClampToEdge)
        {
            _gl = gl;
            _id = gl.GenTexture();
            Bind();
            using (var img = Image.Load<Rgba32>(path))
            {
                _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, (uint)img.Width, (uint)img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);

                img.ProcessPixelRows(a =>
                {
                    for (int y = 0; y < a.Height; y++)
                    {
                        fixed (void* data = a.GetRowSpan(y))
                        {
                            _gl.TexSubImage2D(TextureTarget.Texture2D, 0, 0, y, (uint)a.Width, 1, PixelFormat.Rgba, PixelType.UnsignedByte, data);
                        }
                    }
                });
            }
            SetParameters(clamp);
        }

        private void SetParameters(GLEnum clamp)
        {
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)clamp);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)clamp);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
            _gl.GenerateMipmap(TextureTarget.Texture2D);
        }

        public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
        {
            texSlot = textureSlot - TextureUnit.Texture0;
            _gl.ActiveTexture(textureSlot);
            _gl.BindTexture(TextureTarget.Texture2D, _id);  
        }
        public void Dispose()
        {
            _gl.DeleteTexture(_id);
        }        
    }


    
}
