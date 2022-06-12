using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Maths;
using Silk.NET.Input;
using Silk.NET.OpenGL.Extensions.ImGui;
using System.Numerics;
using System.Linq;
using static LSEngine.Light;

namespace LSEngine
{
    internal class Program
    {
        private static IWindow window;
        private static ImGuiController guiController;
        private static bool IsPaused = false;
        private static GL gl;
        private static IKeyboard primaryKeyboard;

        private static int _windowWidth = 1280;
        private static int _windowHeight = 720;


        private static List<Objects> objects;
        private static List<Light> lights;
        private static List<Material> materials;

        private static Shader LightingShader;
        private static Shader LampShader;
        private static Shader AdvShader;

        private static Vector3 LampPos = new Vector3(1.2f, 1.0f, 2.0f);


        private static Camera camera;
        
        private static Vector2 LastMousePos;
        
        private static DateTime StartTime;
        private static DateTime PrevFrame;


      

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
            foreach (var item in objects)
            {
                item.Dispose();
            }
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
            gl.ClearColor(0.2f,0.2f,0.4f, 1);
            #region a
            //foreach (var obj in objects)
            //{

            //    obj.vao.Bind();
            //    LightingShader.Use();

            //    // bind diffuse map and set to tex0
            //    DiffuseMap.Bind(TextureUnit.Texture0);
            //    // bind specular map and set to tex1
            //    SpecularMap.Bind(TextureUnit.Texture1);

            //    // setup coordinate system
            //    //LightingShader.SetUniform("uModel", Matrix4x4.Identity);
            //    LightingShader.SetUniform("uModel", obj.ModelMatrix);
            //    LightingShader.SetUniform("uView", camera.GetViewMatrix());
            //    LightingShader.SetUniform("uProjection", camera.GetProjectionMatrix());
            //    LightingShader.SetUniform("viewPos", camera.Position);
            //    LightingShader.SetUniform("material.diffuse", 0);
            //    LightingShader.SetUniform("material.specular", 1);
            //    LightingShader.SetUniform("material.shininess", 32.0f);

            //    foreach (var light in lights)
            //    {
            //        LightingShader.Use();
            //        LightingShader.SetUniform("light.ambient", light.AmbientColor);
            //        LightingShader.SetUniform("light.diffuse", light.DiffuseColor); // darkened
            //        LightingShader.SetUniform("light.specular", new Vector3(1.0f, 1.0f, 1.0f));
            //        LightingShader.SetUniform("light.position", light.Position);
            //        gl.DrawArrays(PrimitiveType.Triangles, 0, obj.VertexCount);

            //        LampShader.Use();
            //        LampShader.SetUniform("uModel", light.ModelMatrix);
            //        LampShader.SetUniform("uView", camera.GetViewMatrix());
            //        LampShader.SetUniform("uProjection", camera.GetProjectionMatrix());
            //        light.vao.Bind();
            //        gl.DrawArrays(PrimitiveType.Triangles, 0, light.VertexCount);
            //    }
            //}
            #endregion


            #region Using Advanced lighting shader
            AdvShader.Use();
            foreach (var obj in objects)
            {
                // vPosition, vNormal and vTexCoords are in VAOCube
                obj.vao.Bind();

                if (obj.Material.DiffuseMap != null)
                {
                    obj.Material.DiffuseMap.Bind(TextureUnit.Texture0);
                }
                if (obj.Material.SpecularMap != null)
                {
                    obj.Material.SpecularMap.Bind(TextureUnit.Texture1);
                }
                AdvShader.SetUniform("uModel", obj.ModelMatrix);
                AdvShader.SetUniform("uView", camera.GetViewMatrix());
                AdvShader.SetUniform("uProjection", camera.GetProjectionMatrix());
                
                //AdvShader.SetUniform("vPos", camera.Position);
                AdvShader.SetUniform("mainTexture", obj.Material.DiffuseMap.texSlot);
                AdvShader.SetUniform("hasSpecularMap", obj.Material.SpecularMap == null ? 0 : 1);
                AdvShader.SetUniform("mapSpecular", obj.Material.SpecularMap == null ? 1 : materials[0].SpecularMap.texSlot);

                AdvShader.SetUniform("view", camera.GetViewMatrix());

                AdvShader.SetUniform("materialAmbient", obj.Material.AmbientCoefficient);
                AdvShader.SetUniform("materialDiffuse", obj.Material.DiffuseCoefficient);
                AdvShader.SetUniform("materialSpecular", obj.Material.SpecularCoefficient);
                AdvShader.SetUniform("materialSpecExponent", obj.Material.SpecularExponent);

                // Lights
                for (int j = 0; j < lights.Count; j++)
                {
                    AdvShader.SetUniform($"lights[{j}].position",                   lights[j].Position);
                    AdvShader.SetUniform($"lights[{j}].color",                      lights[j].Color);
                    AdvShader.SetUniform($"lights[{j}].ambientIntensity",           lights[j].AmbientIntensity);
                    AdvShader.SetUniform($"lights[{j}].diffuseIntensity",           lights[j].DiffuseIntensity);
                    AdvShader.SetUniform($"lights[{j}].type",                  (int)lights[j].Type);
                    AdvShader.SetUniform($"lights[{j}].direction",                  lights[j].Direction);
                    AdvShader.SetUniform($"lights[{j}].coneAngle",                  lights[j].ConeAngle);
                    AdvShader.SetUniform($"lights[{j}].linearAttenuation",          lights[j].LinearAttenuation);
                    AdvShader.SetUniform($"lights[{j}].quadraticAttenuation",       lights[j].QuadraticAttenuation);
                    //AdvShader.SetUniform($"lights[{i}].radius", light.Radius);                    
                }
                gl.DrawArrays(PrimitiveType.Triangles, 0, obj.VertexCount);
            }

            // draw lights as cubes
            foreach (var light in lights)
            {
                LampShader.Use();
                LampShader.SetUniform("uModel", light.ModelMatrix);
                LampShader.SetUniform("uView", camera.GetViewMatrix());
                LampShader.SetUniform("uProjection", camera.GetProjectionMatrix());
                LampShader.SetUniform("color", light.Color);
                light.vao.Bind();
                gl.DrawArrays(PrimitiveType.Triangles, 0, light.VertexCount);
            }
            #endregion
            #region imgui stuff
            ImGuiNET.ImGui.Begin("Details");            
            ImGuiNET.ImGui.Text(" ");
            ImGuiNET.ImGui.Text($"Paused: {IsPaused} | Escape to toggle");
            ImGuiNET.ImGui.Text($"FPS: {(int)(1 / delta)}");
            ImGuiNET.ImGui.Text($"FOV: {camera.Fov} | Change with mouse wheel or by slider");
            // fov slider
            ImGuiNET.ImGui.SliderFloat("FOV", ref camera.Fov, 1.0f, 120.0f);
            // display resolution
            ImGuiNET.ImGui.Text("Resolution: " + _windowWidth + "x" + _windowHeight);
            ImGuiNET.ImGui.Text("Aspect Ratio: " + camera.AspectRatio);
            // camera position
            ImGuiNET.ImGui.Text("Camera Position: " + camera.Position);
            ImGuiNET.ImGui.Text($"Fullscreen: {window.WindowState == WindowState.Fullscreen} | Press F to toggle");
            ImGuiNET.ImGui.End();
            
            ImGuiNET.ImGui.Begin("Lighting");
            ImGuiNET.ImGui.Text(" ");
            int i = 0;
            foreach (var light in lights)
            {
                ImGuiNET.ImGui.Text($"Type {light.Type}");
                // position
                ImGuiNET.ImGui.DragFloat($"Light{i} X position", ref lights[i].Position.X, 0.1f, -10.0f, 10.0f);
                ImGuiNET.ImGui.DragFloat($"Light{i} Y position", ref lights[i].Position.Y, 0.1f, -10.0f, 10.0f);
                ImGuiNET.ImGui.DragFloat($"Light{i} Z position", ref lights[i].Position.Z, 0.1f, -10.0f, 10.0f);
                // direction
                ImGuiNET.ImGui.DragFloat($"Light{i} X direction", ref lights[i].direction.X, 0.1f, -1.0f, 1.0f);
                ImGuiNET.ImGui.DragFloat($"Light{i} Y direction", ref lights[i].direction.Y, 0.1f, -1.0f, 1.0f);
                ImGuiNET.ImGui.DragFloat($"Light{i} Z direction", ref lights[i].direction.Z, 0.1f, -1.0f, 1.0f);
                // type
                // color
                ImGuiNET.ImGui.DragFloat($"Light{i} R", ref lights[i].Color.X, 0.01f, 0.0f, 1.0f);
                ImGuiNET.ImGui.DragFloat($"Light{i} G", ref lights[i].Color.Y, 0.01f, 0.0f, 1.0f);
                ImGuiNET.ImGui.DragFloat($"Light{i} B", ref lights[i].Color.Z, 0.01f, 0.0f, 1.0f);
                
                // set light properties and fields
                ImGuiNET.ImGui.DragFloat($"Light{i} Ambient Intensity", ref lights[i].AmbientIntensity, 0.01f, 0.0f, 1.0f);
                ImGuiNET.ImGui.DragFloat($"Light{i} Diffuse Intensity", ref lights[i].DiffuseIntensity, 0.01f, 0.0f, 1.0f);
                // cone angle
                ImGuiNET.ImGui.DragFloat($"Light{i} Cone Angle", ref lights[i].ConeAngle, 0.01f, 0.0f, 360.0f);
                // attenuation
                ImGuiNET.ImGui.DragFloat($"Light{i} Linear Attenuation", ref lights[i].LinearAttenuation, 0.01f, 0.0f, 1.0f);
                ImGuiNET.ImGui.DragFloat($"Light{i} Quadratic Attenuation", ref lights[i].QuadraticAttenuation, 0.01f, 0.0f, 1.0f);
                i++;
            }            
            ImGuiNET.ImGui.End();

            ImGuiNET.ImGui.Begin("Objects");
            ImGuiNET.ImGui.Text(" ");
            i = 0;
            foreach (var obj in objects)
            {
                ImGuiNET.ImGui.DragFloat($"Object{i} X position", ref objects[i].Position.X, 0.1f, -10.0f, 10.0f);
                ImGuiNET.ImGui.DragFloat($"Object{i} Y position", ref objects[i].Position.Y, 0.1f, -10.0f, 10.0f);
                ImGuiNET.ImGui.DragFloat($"Object{i} Z position", ref objects[i].Position.Z, 0.1f, -10.0f, 10.0f);
                i++;
            }
            ImGuiNET.ImGui.End();

            ImGuiNET.ImGui.Begin("Material Settings");
            ImGuiNET.ImGui.Text(" ");
            foreach (var mat in materials)
            {
                ImGuiNET.ImGui.Text($"Material {mat.Name}");
                ImGuiNET.ImGui.DragFloat($"Material{mat.Name} Ambient Intensity R", ref mat.AmbientCoefficient.X, 0.1f, 0.0f, 1.0f);
                ImGuiNET.ImGui.DragFloat($"Material{mat.Name} Ambient Intensity G", ref mat.AmbientCoefficient.Y, 0.1f, 0.0f, 1.0f);
                ImGuiNET.ImGui.DragFloat($"Material{mat.Name} Ambient Intensity B", ref mat.AmbientCoefficient.Z, 0.1f, 0.0f, 1.0f);
                ImGuiNET.ImGui.DragFloat($"Material{mat.Name} Diffuse Intensity R", ref mat.DiffuseCoefficient.X, 0.1f, 0.0f, 1.0f);
                ImGuiNET.ImGui.DragFloat($"Material{mat.Name} Diffuse Intensity G", ref mat.DiffuseCoefficient.Y, 0.1f, 0.0f, 1.0f);
                ImGuiNET.ImGui.DragFloat($"Material{mat.Name} Diffuse Intensity B", ref mat.DiffuseCoefficient.Z, 0.1f, 0.0f, 1.0f);
                ImGuiNET.ImGui.DragFloat($"Material{mat.Name} Specular Intensity R", ref mat.SpecularCoefficient.X, 0.1f, 0.0f, 1.0f);
                ImGuiNET.ImGui.DragFloat($"Material{mat.Name} Specular Intensity G", ref mat.SpecularCoefficient.Y, 0.1f, 0.0f, 1.0f);
                ImGuiNET.ImGui.DragFloat($"Material{mat.Name} Specular Intensity B", ref mat.SpecularCoefficient.Z, 0.1f, 0.0f, 1.0f);
                ImGuiNET.ImGui.DragFloat($"Material{mat.Name} Specular Exponent", ref mat.SpecularExponent, 0.1f, 0.0f, 100.0f);
            }
            ImGuiNET.ImGui.End();
            guiController.Render();
            #endregion
        }

        private static void OnUpdate(double dTime)
        {
            int fps = (int)(1 / dTime);
            window.Title = $"LSEngine - {fps} FPS, {window.Size.X}x{window.Size.Y}, FOV {camera.Fov}";
            float movespeed;

            if (!IsPaused)
            {
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
            objects = new();
            lights = new();
            materials = new();
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
            ImGuiNET.ImGui.SetWindowSize(new(400, 200));

           

            camera = new Camera(Vector3.UnitZ * 6, Vector3.UnitZ * -1, Vector3.UnitY,(float)_windowWidth/(float)_windowHeight);
            gl.Viewport(0, 0, (uint)_windowWidth, (uint)_windowHeight);

            
            LightingShader = new Shader(gl, "shaders/shader.vert", "shaders/lighting.frag");
            LampShader = new Shader(gl, "shaders/shader.vert", "shaders/shader.frag");
            AdvShader = new Shader(gl, "shaders/lighting_adv.vert", "shaders/lighting_adv.frag");
            materials.Add(new(
                name : "bricks", 
                diffuseMap : new Texture(gl, "textures/brickwall/spnza_bricks_a_diff.png"), 
                specularMap : new Texture(gl, "textures/brickwall/spnza_bricks_a_spec.png")
                ));
            materials[0].AmbientCoefficient = new(1.0f, 1.0f, 1.0f);
            materials[0].DiffuseCoefficient = new(1.0f, 1.0f, 1.0f);
            materials[0].SpecularCoefficient = new(1.0f, 1.0f, 1.0f);
            materials[0].SpecularExponent = 3.2f;

            materials.Add(new(
                name: "minecraftGrass",
                diffuseMap: new Texture(gl, "textures/grassblock/grass.png")
                ));
            materials[1].AmbientCoefficient = new(1.0f, 1.0f, 1.0f);
            materials[1].DiffuseCoefficient = new(1.0f, 1.0f, 1.0f);
            materials[1].SpecularCoefficient = new(1.0f, 1.0f, 1.0f);
            materials[1].SpecularExponent = 3.2f;


            // VAOCube
            //
            objects.Add(Cube.GetCube(gl));
            objects[0].Position = new(-0.7f, 0.2f, -1.2f);
            objects[0].Material = materials[0];

            objects.Add(Cube.GetCube(gl));
            objects[1].Position = new(-1.4f, -0.2f, -1.9f);
            objects[1].Material = materials[0];

            objects.Add(Quad.GetQuad(gl));
            objects[2].Position = new(0.0f, -5.0f, 0.0f);
            objects[2].Scale = 100.0f;
            objects[2].Material = materials[1];

            // Lights
            //spotlight
            lights.Add(Light.GetLight(gl, Light.LightType.SpotLightCube));
            lights[0].Position = new(0.7f, 0.0f, -1.2f);
            lights[0].Direction = new(1.0f, 0.0f, 0.0f);
            lights[0].Color = new(0.0f, 0.0f, 1.0f);
            lights[0].AmbientIntensity = 0.7f;
            lights[0].DiffuseIntensity = 1.0f;
            lights[0].ConeAngle = 20.0f;
            lights[0].LinearAttenuation = 0.10f;
            lights[0].QuadraticAttenuation = 0.02f;
            // directional
            lights.Add(Light.GetLight(gl, Light.LightType.DirectionalCube));
            lights[1].Position = new(10.0f, 10.0f, 10.0f);
            lights[1].Direction = new(1.0f, 1.0f, 1.0f);
            lights[1].Color = new(1.0f, 1.0f, 1.0f);
            lights[1].AmbientIntensity = 0.21f;
            lights[1].DiffuseIntensity = 1.0f;
            // point
            lights.Add(Light.GetLight(gl, Light.LightType.PointLightCube));
            lights[2].Position = new(-1.0f, 0.9f, -0.5f);
            lights[2].Color = new(1.0f, 0.0f, 0.0f);
            lights[2].AmbientIntensity = 0.02f;
            lights[2].DiffuseIntensity = 1.0f;
            lights[2].LinearAttenuation = 1.0f;
            lights[2].QuadraticAttenuation = 1.0f;


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
                float xoffset = lookSens * (position.X - LastMousePos.X);
                float yoffset = lookSens * (position.Y - LastMousePos.Y);
                LastMousePos = position;
                if (!IsPaused)
                {
                    mouse.Cursor.CursorMode = CursorMode.Raw;
                    camera.ModifyDirection(xoffset, yoffset);
                }
                else
                {
                    mouse.Cursor.CursorMode = CursorMode.Normal;
                }
            }
        }

        private static void KeyDown(IKeyboard keyboard, Key key, int arg3)
        {
            // if escape, close window
            if (key == Key.Escape)
            {
                IsPaused = !IsPaused;
            }
        }
    }
}