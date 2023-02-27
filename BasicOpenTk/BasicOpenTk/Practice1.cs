using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL;

namespace BasicOpenTk
{
    public class Practice1 : GameWindow
    {
        private int _vertexArrayObject;
        private int _vertexBufferObject;
        private int _shaderProgramObject;
        
        public Practice1() : base(GameWindowSettings.Default, new NativeWindowSettings
        {
            Title = "Practice gl",
            Size = new Vector2i(1280, 768),
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

           

            var vertexShaderCode = @"
            #version 330 core

            layout (location = 0) in vec3 aPostion;

            void main()
            {
                gl_Position = vec4(aPostion, 1.0f);
            }
            ";

            var fragmentShaderCode = @"
            #version 330 core

            out vec4 fragmentColor;

            void main(){
                fragmentColor = vec4(1, 0, 0, 1);
            }
            ";


            var vertexShaderObject = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderObject, vertexShaderCode);
            GL.CompileShader(vertexShaderObject);
            GetShaderInfoLog(vertexShaderObject);

            var fragmentShaderOjbect = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShaderOjbect, fragmentShaderCode);
            GL.CompileShader(fragmentShaderOjbect);
            GetShaderInfoLog(fragmentShaderOjbect);

            _shaderProgramObject = GL.CreateProgram();
            GL.AttachShader(_shaderProgramObject, vertexShaderObject);
            GL.AttachShader(_shaderProgramObject, fragmentShaderOjbect);

            GL.LinkProgram(_shaderProgramObject);

            GL.DetachShader(_shaderProgramObject, vertexShaderObject);
            GL.DetachShader(_shaderProgramObject, fragmentShaderOjbect);

            GL.DeleteShader(vertexShaderObject);
            GL.DeleteShader(fragmentShaderOjbect);

            //normalized device coordinates space
            //var vertices = new float[] //defined on ram need to move it to graphics ram will be moved via graphics buffer
            //{
            //    -0.5f, 0.5f, 0f, 1f, 0f, 0f, 1f,     //vertex0 position (2 floats) color (4 floats)
            //    0.5f, 0.5f, 0f, 0f, 1f, 0f, 1f,     //vertex1
            //    0.5f, -0.5f, 0f, 0f, 0f, 1f, 0f,     //vertex2
            //    -0.5f, -0.5f, 0f, 0f, 0f, 1f, 0f,     //vertex3
            //};

            //screen space or pixel coordinates
            //var vertices = new float[]
            //{
            //    x, y + h, 1f, 0f, 0f, 1f, // position (2 floats) color (4 floats)
            //    x + w, y + h, 0f, 1f, 0f, 1f,
            //    x + w, y, 0f, 0f, 1f, 0f,
            //    x, y, 0f, 0f, 1f, 0f,
            //};

            base.OnLoad();
        }

        protected override void OnUnload()
        {
            GL.BindVertexArray(0);
            GL.DeleteVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer,0);
            GL.DeleteBuffer(_vertexBufferObject);

            GL.UseProgram(0);
            GL.DeleteProgram(_shaderProgramObject);
            base.OnUnload();
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

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(Color4.Blue);

            Thread.Sleep(100);
            var random = new Random();
            var randomVertice1X = Convert.ToSingle(random.Next(-100,100)) / 100;
            var vertices = new float[]
            {
                randomVertice1X, 0.5f, 0f,
                0.5f, -0.5f, 0f,
                -0.5f, -0.5f, 0f
            };

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);


            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindVertexArray(0);



            GL.UseProgram(_shaderProgramObject);
            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }
    }
}
