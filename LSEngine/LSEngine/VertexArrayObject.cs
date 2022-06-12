using Silk.NET.OpenGL;
using System;
namespace LSEngine
{
    public class VertexArrayObject<TVertexType, TIndexType> : IDisposable
        where TVertexType : unmanaged
        where TIndexType : unmanaged
    {
        private uint _id;
        private GL _gl;
        
        public uint VertexCount { get; set; }

        public VertexArrayObject(GL gl, BufferObject<TVertexType> vbo, BufferObject<TIndexType> ebo)
        {
            _gl = gl;
            _id = gl.GenVertexArray();
            Bind();
            vbo.Bind();
            ebo.Bind();
        }
        
        public unsafe void VertexAttribPointer(uint index, int count, VertexAttribPointerType type, uint vertSize, int offset)
        {
            _gl.VertexAttribPointer(index, count, type, false, vertSize * (uint)sizeof(TVertexType), (void*)(offset*sizeof(TVertexType)));
            _gl.EnableVertexAttribArray(index);
        }
        
        public void Dispose()
        {
            _gl.DeleteVertexArray(_id);
        }

        public void Bind()
        {
            _gl.BindVertexArray(_id);
        }
    } 
}
