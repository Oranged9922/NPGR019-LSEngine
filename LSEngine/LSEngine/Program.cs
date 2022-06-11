using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Maths;
using Silk.NET.Input;
using Silk.NET.OpenGL.Extensions.ImGui;
using System.Numerics;
using System.Linq;

namespace LSEngine
{
    internal class Program
    {
        private static IWindow window;
        private static ImGuiController guiController;

        private static GL gl;
        private static IKeyboard primaryKeyboard;

        private static int _windowWidth = 1280;
        private static int _windowHeight = 720;

        private static BufferObject<float> VBO;
        private static BufferObject<uint> EBO;
        private static VertexArrayObject<float,uint> VAOCube;
        private static Shader LightingShader;
        private static Shader LampShader;

        private static Vector3 LampPos = new Vector3(1.2f, 1.0f, 2.0f);

        private static Texture DiffuseMap;
        private static Texture SpecularMap;

        private static Camera camera;
        
        private static Vector2 LastMousePos;
        
        private static DateTime StartTime;
        private static DateTime PrevFrame;


        private static readonly float[] Vertices =
       {
            //X    Y      Z       Normals             U     V
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 1.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 0.0f, 1.0f,

            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 0.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 0.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 0.0f, 1.0f,

            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f, 0.0f, 1.0f,
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f, 1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f, 1.0f, 0.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f, 1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f, 0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f, 0.0f, 1.0f,

             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f, 0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f, 1.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f, 1.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f, 1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f, 0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f, 0.0f, 1.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, 0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, 1.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f, 1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f, 1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f, 0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, 0.0f, 1.0f,

            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, 0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, 1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f, 1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f, 1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f, 0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, 0.0f, 1.0f
        };

        private static readonly uint[] Indices =
        {
            0, 1, 3,
            1, 2, 3
        };


        static void Main(string[] args)
        {
            window = CreateWindow();

            window.Run();
        }

        private static IWindow CreateWindow()
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(_windowWidth, _windowHeight);
            options.Title = "LSEngine";
            window = Window.Create(options);

            window.Load += OnLoad;
            window.Update += OnUpdate;
            window.Render += OnRender;
            window.Closing += OnClose;
            window.FramebufferResize += OnResize;

            return window;
        }

        private static void OnResize(Vector2D<int> Size)
        {
            gl.Viewport(Size);
            camera.AspectRatio = (float)Size.X / (float)Size.Y;
        }

        private static void OnClose()
        {
            VBO.Dispose();
            EBO.Dispose();
            VAOCube.Dispose();
            LightingShader.Dispose();
            LampShader.Dispose();
            
        }

        private static void OnRender(double delta)
        {
            // imgui controller
            guiController.Update((float)delta);
            
            //enable depth test
            gl.Enable(EnableCap.DepthTest);
            // clear color buffer bit and depth buffer bit
            gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            gl.ClearColor(0.1f, 0.1f, 0.2f, 1);
            VAOCube.Bind();
            LightingShader.Use();

            // bind diffuse map and set to tex0
            DiffuseMap.Bind(TextureUnit.Texture0);
            // bind specular map and set to tex1
            SpecularMap.Bind(TextureUnit.Texture1);

            // setup coordinate system
            LightingShader.SetUniform("uModel", Matrix4x4.Identity);
            LightingShader.SetUniform("uView", camera.GetViewMatrix());
            LightingShader.SetUniform("uProjection", camera.GetProjectionMatrix());
            LightingShader.SetUniform("viewPos", camera.Position);
            LightingShader.SetUniform("material.diffuse", 0);
            LightingShader.SetUniform("material.specular", 1);
            LightingShader.SetUniform("material.shininess", 32.0f);

            var diffuseColor = new Vector3(0.5f);
            var ambientColor = diffuseColor * new Vector3(0.2f);

            LightingShader.SetUniform("light.ambient", ambientColor);
            LightingShader.SetUniform("light.diffuse", diffuseColor); // darkened
            LightingShader.SetUniform("light.specular", new Vector3(1.0f, 1.0f, 1.0f));
            LightingShader.SetUniform("light.position", LampPos);

            //We're drawing with just vertices and no indicies, and it takes 36 verticies to have a six-sided textured cube
            gl.DrawArrays(PrimitiveType.Triangles, 0, 36);

            LampShader.Use();

            //The Lamp cube is going to be a scaled down version of the normal cubes verticies moved to a different screen location
            var lampMatrix = Matrix4x4.Identity;
            lampMatrix *= Matrix4x4.CreateScale(0.2f);
            lampMatrix *= Matrix4x4.CreateTranslation(LampPos);

            LampShader.SetUniform("uModel", lampMatrix);
            LampShader.SetUniform("uView", camera.GetViewMatrix());
            LampShader.SetUniform("uProjection", camera.GetProjectionMatrix());
            
            gl.DrawArrays(PrimitiveType.Triangles, 0, 36);

            // imgui stuff
            guiController.Render();

        }

        private static void OnUpdate(double dTime)
        {
            int fps = (int)(1 / dTime);
            window.Title = $"LSEngine - {fps} FPS, {window.Size.X}x{window.Size.Y}, FOV {camera.Fov}";
            float movespeed;
            if (primaryKeyboard.IsKeyPressed(Key.ControlLeft))
            {
                movespeed = 10f * (float)dTime;
            }
            else
            {
                movespeed = 2.5f * (float)dTime;
            }                
            //move
            if (primaryKeyboard.IsKeyPressed(Key.W))
            {
                camera.Position += camera.Front * movespeed;
            }
            if (primaryKeyboard.IsKeyPressed(Key.S))
            {
                camera.Position -= camera.Front * movespeed;
            }
            if (primaryKeyboard.IsKeyPressed(Key.A))
            {
                camera.Position -= Vector3.Normalize(Vector3.Cross(camera.Front, camera.Up)) * movespeed;
            }
            if (primaryKeyboard.IsKeyPressed(Key.D))
            {
                camera.Position += Vector3.Normalize(Vector3.Cross(camera.Front, camera.Up)) * movespeed;
            }

            // up/down
            if (primaryKeyboard.IsKeyPressed(Key.Space))
            {
                camera.Position += camera.Up * movespeed;
            }
            if (primaryKeyboard.IsKeyPressed(Key.ShiftLeft))
            {
                camera.Position -= camera.Up * movespeed;
            }   
            if (primaryKeyboard.IsKeyPressed(Key.F))
            {
                window.WindowState = (window.WindowState == WindowState.Fullscreen) ? WindowState.Normal : WindowState.Fullscreen;
                _windowHeight = window.Size.Y;
                _windowWidth = window.Size.X;
                camera.AspectRatio = (float)_windowWidth / (float)_windowHeight;
                gl.Viewport(0, 0, (uint)_windowWidth, (uint)_windowHeight);
                Thread.Sleep(200);
            }
        }

        private static void OnLoad()
        {
            StartTime = DateTime.Now;
            PrevFrame = StartTime;
            // Input (keyboard/mice)
            IInputContext input = window.CreateInput();
            primaryKeyboard = input.Keyboards.FirstOrDefault();
            if (primaryKeyboard != null)
            {
                primaryKeyboard.KeyDown += KeyDown;
            }
            for (int i = 0; i < input.Mice.Count; i++)
            {
                input.Mice[i].Cursor.CursorMode = CursorMode.Raw;
                input.Mice[i].MouseMove += OnMouseMove;
                input.Mice[i].Scroll += OnMouseWheel;
            }
            //

            


            gl = GL.GetApi(window);


            guiController = new ImGuiController(gl, window, input);

            // VAOCube
            EBO = new BufferObject<uint>(gl,Indices, BufferTargetARB.ElementArrayBuffer);
            VBO = new BufferObject<float>(gl, Vertices, BufferTargetARB.ArrayBuffer);
            VAOCube = new VertexArrayObject<float,uint>(gl,VBO,EBO);
            VAOCube.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, 8, 0);
            VAOCube.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, 8, 3);
            VAOCube.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, 8, 6);
            //

            camera = new Camera(Vector3.UnitZ * 6, Vector3.UnitZ * -1, Vector3.UnitY,(float)_windowWidth/(float)_windowHeight);
            gl.Viewport(0, 0, (uint)_windowWidth, (uint)_windowHeight);
            LightingShader = new Shader(gl, "shaders/shader.vert", "shaders/lighting.frag");
            LampShader = new Shader(gl, "shaders/shader.vert", "shaders/shader.frag");

            DiffuseMap = new Texture(gl, "textures/brickwall/spnza_bricks_a_diff.png");
            SpecularMap = new Texture(gl, "textures/brickwall/spnza_bricks_a_spec.png");
        }

        private static void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel)
        {
            camera.ModifyFOV(mouse.ScrollWheels.FirstOrDefault().X);
        }

        private static void OnMouseMove(IMouse mouse, Vector2 position)
        {
            const float lookSens = 0.1f;
            if (LastMousePos == default) LastMousePos = position;
            else
            {
                float xoffset = lookSens* (position.X - LastMousePos.X);
                float yoffset = lookSens* (position.Y - LastMousePos.Y);
                LastMousePos = position;
                camera.ModifyDirection(xoffset, yoffset);
            }
        }

        private static void KeyDown(IKeyboard keyboard, Key key, int arg3)
        {
            // if escape, close window
            if (key == Key.Escape)
            {
                window.Close();
            }
        }
    }
}