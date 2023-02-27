using OpenTK.Graphics.OpenGL;

namespace BasicOpenTk
{
    public sealed class VertexBuffer : IDisposable
    {
        public static readonly int MAX_VERTEX_COUNT = 100_000;
        public static readonly int MIN_VERTEX_COUNT = 1;

        private bool _disposed;
        public readonly int VertexBufferObject;
        public readonly VertexInfo VertexInfo;
        public readonly int VertexCount;
        public readonly bool IsStatic;
        public VertexBuffer(VertexInfo vertexInfo, int vertexCount, bool isStatic = true)
        {
            _disposed = false;

            if (vertexCount < MIN_VERTEX_COUNT || vertexCount > MAX_VERTEX_COUNT)
                throw new ArgumentException(nameof(vertexCount));


            VertexInfo = vertexInfo;
            VertexCount = vertexCount;
            IsStatic = isStatic;

            var hint = BufferUsageHint.StaticDraw;
            if (!isStatic)
                hint = BufferUsageHint.StreamDraw;

            VertexBufferObject = GL.GenBuffer(); //generating the buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject); //binding the vertex buffer to the handle
            GL.BufferData(BufferTarget.ArrayBuffer, VertexCount * VertexInfo.SizeInBytes, IntPtr.Zero, hint);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //way to unbind the buffer
        }

        ~VertexBuffer()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (_disposed) return;
          
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VertexBufferObject);

            _disposed = true;
            GC.SuppressFinalize(this);
        }

        public void SetData<T>(T[] data, int count) where T : struct
        {
            if (typeof(T) != VertexInfo.Type)
                throw new ArgumentNullException("Generic type 'T' does not match the vertex type of the vertex buffer.");

            if(data is null)
                throw new ArgumentNullException(nameof(data));

            if (data.Length <= 0)
                throw new ArgumentOutOfRangeException(nameof(data));

            if(count <= 0 || count > VertexCount || count > data.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, count * VertexInfo.SizeInBytes, data);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
