using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace BasicOpenTk
{
    public class Game : GameWindow
    {
        private VertexBuffer _vertexBuffer;
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

            float x = 384f;
            float y = 400f;
            float w = 512f;
            float h = 256f;

            var vertices = new VertexPositionColor[]
            {
                new VertexPositionColor(new Vector2(x, y + h), new Color4(1f, 0f, 0f, 1f)),
                new VertexPositionColor(new Vector2(x + w, y + h), new Color4(0f, 1f, 0f, 1f)),
                new VertexPositionColor(new Vector2(x + w, y), new Color4(0f, 0f, 1f, 1f)),
                new VertexPositionColor(new Vector2(x, y), new Color4(1f, 1f, 0f, 1f)),
            };

            var indices = new int[]
            {
                0,1,2,0,2,3
            };

            _vertexBuffer= new VertexBuffer(VertexPositionColor.VertexInfo, vertices.Length);
            _vertexBuffer.SetData(vertices, vertices.Length);

            _indexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            var vertexSizeInBytes = _vertexBuffer.VertexInfo.SizeInBytes;

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer.VertexBufferObject);

            foreach (var attr in VertexPositionColor.VertexInfo.VertexAttributes)
            {
                GL.VertexAttribPointer(attr.Index, attr.ComponentCount, VertexAttribPointerType.Float, false, vertexSizeInBytes, attr.Offset);//stride is number of bytes between each vertex
                GL.EnableVertexAttribArray(attr.Index);  
            }
            GL.BindVertexArray(0);

            // shaders are written in glsl and out is used in vertex shader to pass value to pixel shaders and a is prefix for attribute, v for vertex 
            var vertexShaderCode =
                @"
                #version 330 core

                uniform vec2 ViewportSize;

                layout (location = 0) in vec2 aPosition;
                layout (location = 1) in vec4 aColor;

                out vec4 vColor;

                void main()
                {
                    float nx = aPosition.x / ViewportSize.x * 2.0f - 1.0f;
                    float ny = aPosition.y / ViewportSize.y * 2.0f - 1.0f;    
                    gl_Position = vec4(nx, ny, 0, 1.0f);
                    
                    vColor = aColor;
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

            GL.DetachShader(_shaderProgramObject, vertexShaderHanlde);
            GL.DetachShader(_shaderProgramObject, pixelShaderHandle);

            GL.DeleteShader(vertexShaderHanlde);
            GL.DeleteShader(pixelShaderHandle);

            var viewPort = new int[4];
            GL.GetInteger(GetPName.Viewport, viewPort);

            GL.UseProgram(_shaderProgramObject);
            var viewPortSizeUniformLocation = GL.GetUniformLocation(_shaderProgramObject, "ViewportSize");
            GL.Uniform2(viewPortSizeUniformLocation, viewPort[2], (float)viewPort[3]);
            GL.UseProgram(0);


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

           _vertexBuffer?.Dispose();

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
