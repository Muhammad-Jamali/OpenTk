using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace BasicOpenTk
{
    

    public class Game : GameWindow
    {
        private int _vertexBufferHandle;
        private int _shaderProgramHandle;
        private int _vertexArrayHandle;

        public Game()
            : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            CenterWindow(new Vector2i(1280, 768));

        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }

        protected override void OnLoad()
        {
            GL.ClearColor(new Color4(0.3f, 0.4f, 0.5f, 1f));

            float[] vertices = new float[] //defined on ram need to move it to graphics ram will be moved via graphics buffer
            {
                0.0f, 0.5f, 0f, //vertex0
                0.5f, -0.5f, 0f, //vertex1
                -0.5f, -0.5f, 0f //vertex2
            };

            _vertexBufferHandle = GL.GenBuffer(); //generating the buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferHandle); //binding the vertex buffer to the handle
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //way to unbind the buffer

            _vertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayHandle);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferHandle);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);

            // shaders are written in glsl
            string vertexShaderCode =
                @"
                        #version 330 core

                        layout (location = 0) in vec3 aPosition;

                        void main()
                        {
                            gl_Position=vec4(aPosition, 1.0f);
                        }
                        ";

            //aka fragment shader
            string pixelShaderCode =
                @"
                        #version 330 core

                        out vec4 pixelColor;

                        void main()
                        {
                            pixelColor=vec4(1.0f, 0.8f, 0.8f, 1.0f);
                        }
                        ";



            int vertexShaderHanlde = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHanlde, vertexShaderCode);
            GL.CompileShader(vertexShaderHanlde);

            GetShaderInfoLog(vertexShaderHanlde);
            
            int pixelShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(pixelShaderHandle, pixelShaderCode);
            GL.CompileShader(pixelShaderHandle);
            GetShaderInfoLog(pixelShaderHandle);

            _shaderProgramHandle = GL.CreateProgram();

            GL.AttachShader(_shaderProgramHandle, vertexShaderHanlde);
            GL.AttachShader(_shaderProgramHandle, pixelShaderHandle);

            GL.LinkProgram(_shaderProgramHandle);

            //var buffer = new int[1024];
            //GL.GetShaderInfoLog(vertexShaderHanlde, buffer.Length, );
            GL.DetachShader(_shaderProgramHandle, vertexShaderHanlde);
            GL.DetachShader(_shaderProgramHandle, pixelShaderHandle);

            GL.DeleteShader(vertexShaderHanlde);
            GL.DeleteShader(pixelShaderHandle);

            base.OnLoad();
        }

        private void GetShaderInfoLog(int shaderHandle)
        {
            GL.GetShader(shaderHandle, ShaderParameter.CompileStatus, out int compileStatus);

            if (compileStatus != 0)
                return;

            GL.GetShader(shaderHandle, ShaderParameter.InfoLogLength, out int infoLogLength);

            if (infoLogLength <= 0) return;

            int dummy;
            string shaderInfoLog;
            GL.GetShaderInfoLog(shaderHandle, infoLogLength, out dummy, out shaderInfoLog);
            Console.WriteLine(shaderInfoLog);
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(_vertexBufferHandle);

            GL.UseProgram(0);
            GL.DeleteProgram(_shaderProgramHandle);
            base.OnUnload();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(_shaderProgramHandle);

            GL.BindVertexArray(_vertexArrayHandle);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }

    }
}
