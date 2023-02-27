using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace BasicOpenTk
{
    

    public class Game : GameWindow
    {
        private int _vertexBufferObject;
        private int _shaderProgramObject;
        private int _vertexArrayObject;
        private int _indexBufferObject;

        public Game(int width = 1280, int height = 768, string title = "Game1")
            : base(
                  GameWindowSettings.Default,
                  new NativeWindowSettings()
                  {
                      Title = title,
                      Size = new Vector2i(width, height),
                      WindowBorder = WindowBorder.Fixed,
                      StartVisible = false,
                      StartFocused = true,
                      API = ContextAPI.OpenGL,
                      Profile = ContextProfile.Core,
                      APIVersion = new Version(3, 3)
                  })
        {
            CenterWindow();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }

        protected override void OnLoad()
        {
            IsVisible = true;

            GL.ClearColor(new Color4(0.3f, 0.4f, 0.5f, 1f));

            var vertices = new float[] //defined on ram need to move it to graphics ram will be moved via graphics buffer
           {
                 -0.5f, 0.5f, 0f, 1f, 0f, 0f, 1f,     //vertex0 position (3 floats) color (4 floats)
                  0.5f, 0.5f, 0f, 0f, 1f, 0f, 1f,     //vertex1
                  0.5f, -0.5f, 0f, 0f, 0f, 1f, 0f,     //vertex2
                  -0.5f, -0.5f, 0f, 0f, 0f, 1f, 0f,     //vertex3
           };

            var indices = new int[]
            {
                0,1,2,0,2,3
            };

            _vertexBufferObject = GL.GenBuffer(); //generating the buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject); //binding the vertex buffer to the handle
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //way to unbind the buffer

            _indexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float)); //stride is number of bytes between each vertex
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.BindVertexArray(0);

            // shaders are written in glsl and out is used in vertex shader to pass value to pixel shaders and a is prefix for attribute, v for vertex 
            var vertexShaderCode =
                @"
                #version 330 core

                layout (location = 0) in vec3 aPosition;
                layout (location = 1) in vec4 aColor;

                out vec4 vColor;

                void main()
                {
                    vColor = aColor;
                    gl_Position = vec4(aPosition, 1.0f);
                }
                ";

            //aka fragment shader
            var pixelShaderCode =
                @"
                #version 330 core

                in vec4 vColor;

                out vec4 pixelColor;

                void main()
                {
                    pixelColor = vColor;
                }
                ";



            var vertexShaderHanlde = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHanlde, vertexShaderCode);
            GL.CompileShader(vertexShaderHanlde);

            GetShaderInfoLog(vertexShaderHanlde);
            
            var pixelShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(pixelShaderHandle, pixelShaderCode);
            GL.CompileShader(pixelShaderHandle);
            GetShaderInfoLog(pixelShaderHandle);

            _shaderProgramObject = GL.CreateProgram();

            GL.AttachShader(_shaderProgramObject, vertexShaderHanlde);
            GL.AttachShader(_shaderProgramObject, pixelShaderHandle);

            GL.LinkProgram(_shaderProgramObject);

            //var buffer = new int[1024];
            //GL.GetShaderInfoLog(vertexShaderHanlde, buffer.Length, );
            GL.DetachShader(_shaderProgramObject, vertexShaderHanlde);
            GL.DetachShader(_shaderProgramObject, pixelShaderHandle);

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
            GL.BindVertexArray(0);
            GL.DeleteVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DeleteBuffer(_indexBufferObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(_vertexBufferObject);

            GL.UseProgram(0);
            GL.DeleteProgram(_shaderProgramObject);
            base.OnUnload();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(_shaderProgramObject);
            GL.BindVertexArray(_vertexArrayObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBufferObject);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);

            //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }

    }
}
