//using Silk.NET.OpenGL;
//using System.Numerics;

//namespace LSEngine
//{
//    public abstract class Objects : IDisposable
//    {
//        public Type Type;
//        public VertexArrayObject<float, uint> vao;
//        public uint VertexCount { get => vao.VertexCount; }
//        public Vector3 position;
//        public abstract Vector3 Position { get; set; }
//        public float Scale = 1.0f;
//        public float Yaw = 0.0f;
//        public float Pitch = 0.0f;
//        public float Roll = 0.0f;
//        public Vector3 YawPitchRoll { set { Yaw = value.X; Pitch = value.Y; Roll = value.Z; } }

//        /// <summary>
//        /// kinda works, but doesn't really work
//        /// </summary>
//        public Matrix4x4 ModelMatrix { get => Matrix4x4.CreateScale(this.Scale) * Matrix4x4.CreateTranslation(this.Position); }
//        internal Material Material { get; set; }

//        public void Dispose()
//        {
//        }

//        public Action<double> Update { get; set; } = (deltaTime) => { };

//    }
    
//    static class UnitCube
//    {
//        static VertexArrayObject<float, uint> VAOCube = null;
//        private static readonly float[] Vertices =
//    {
//        //X    Y      Z       Normals             U     V
//        -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 0.0f, 1.0f,
//            0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 1.0f, 1.0f,
//            0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 1.0f, 0.0f,
//            0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 1.0f, 0.0f,
//        -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 0.0f, 0.0f,
//        -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, 0.0f, 1.0f,

//        -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 0.0f, 1.0f,
//            0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 1.0f, 1.0f,
//            0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 1.0f, 0.0f,
//            0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 1.0f, 0.0f,
//        -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 0.0f, 0.0f,
//        -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f, 0.0f, 1.0f,

//        -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f, 0.0f, 1.0f,
//        -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f, 1.0f, 1.0f,
//        -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f, 1.0f, 0.0f,
//        -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f, 1.0f, 0.0f,
//        -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f, 0.0f, 0.0f,
//        -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f, 0.0f, 1.0f,

//            0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f, 0.0f, 1.0f,
//            0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f, 1.0f, 1.0f,
//            0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f, 1.0f, 0.0f,
//            0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f, 1.0f, 0.0f,
//            0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f, 0.0f, 0.0f,
//            0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f, 0.0f, 1.0f,

//        -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, 0.0f, 1.0f,
//            0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, 1.0f, 1.0f,
//            0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f, 1.0f, 0.0f,
//            0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f, 1.0f, 0.0f,
//        -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f, 0.0f, 0.0f,
//        -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, 0.0f, 1.0f,

//        -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, 0.0f, 1.0f,
//            0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, 1.0f, 1.0f,
//            0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f, 1.0f, 0.0f,
//            0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f, 1.0f, 0.0f,
//        -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f, 0.0f, 0.0f,
//        -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, 0.0f, 1.0f
//    };

//        private static readonly uint[] Indices =
//        {
//        0, 1, 3,
//        1, 2, 3
//    };

//        public static VertexArrayObject<float, uint> GetVertexArrayObject(GL gl)
//        {
//            return VAOCube ?? CreateVAOCube(gl);
//        }

//        private static VertexArrayObject<float, uint> CreateVAOCube(GL gl)
//        {

//            var EBO = new BufferObject<uint>(gl, Indices, BufferTargetARB.ElementArrayBuffer);
//            var VBO = new BufferObject<float>(gl, Vertices, BufferTargetARB.ArrayBuffer);
//            VAOCube = new VertexArrayObject<float, uint>(gl, VBO, EBO);
//            VAOCube.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, 8, 0);
//            VAOCube.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, 8, 3);
//            VAOCube.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, 8, 6);
//            VAOCube.VertexCount = 36;
//            return VAOCube;
//        }
//    }

//    static class UnitQuad
//    {
//        private static VertexArrayObject<float, uint> VAOQuad;
//        private static readonly float[] Vertices =
//        {
//          //  X   Y   Z   nX  nY  nZ  U   V
//           -0.5f, 0.0f,  0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f,
//           0.5f, 0.0f, -0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f,
//          -0.5f, 0.0f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f,
         

//           -0.5f, 0.0f,  0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f,
//           0.5f, 0.0f,  0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f,
//           0.5f, 0.0f, -0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f,
//        };

//        private static readonly uint[] Indices =
//        {
//            0, 1, 2,
//            0, 3, 1
//        };

//        public static VertexArrayObject<float, uint> GetVertexArrayObject(GL gl)
//        {
//            return VAOQuad ?? CreateVAOQuad(gl);
//        }

//        private static VertexArrayObject<float, uint> CreateVAOQuad(GL gl)
//        {
//            var EBO = new BufferObject<uint>(gl, Indices, BufferTargetARB.ElementArrayBuffer);
//            var VBO = new BufferObject<float>(gl, Vertices, BufferTargetARB.ArrayBuffer);
//            VAOQuad = new VertexArrayObject<float, uint>(gl, VBO, EBO);
//            VAOQuad.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, 8, 0);
//            VAOQuad.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, 8, 3);
//            VAOQuad.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, 8, 6);
//            VAOQuad.VertexCount = 6;
//            return VAOQuad;
//        }
//    }


//    public class Cube : Objects
//    {
//        public Type Type = typeof(Cube);
//        public Vector3 position;
//        public override Vector3 Position { get => position; set => position = new Vector3(value.X - 0.5f, value.Y - 0.5f, value.Z - 0.5f); }
//        public static Cube GetCube(GL gl)
//        {
//            Cube c = new Cube();
//            c.vao = UnitCube.GetVertexArrayObject(gl);
//            return c;
//        }
//    }
//    public class Quad : Objects
//    {
//        private Vector3 position = new Vector3(0, 0, 0);
//        public override Vector3 Position { get => position; set => position = value; }
//        public Type Type = typeof(Quad);
//        public static Quad GetQuad(GL gl)
//        {
//            Quad q = new Quad();
//            q.vao = UnitQuad.GetVertexArrayObject(gl);
//            return q;
//        }
//    }

//        public class Light : Objects
//    {
//        public Vector3 position;
//        public override Vector3 Position { get => position; set => position = new Vector3(value.X - 0.5f, value.Y - 0.5f, value.Z - 0.5f); }

//        public Type Type = typeof(Light);
//        public Vector3 DiffuseColor = new(0.5f);
//        public Vector3 AmbientColor = new(0.1f);
//        public float DiffuseIntensity;
//        public float AmbientIntensity;
//        public LightType LightTypeEnum;
//        public Vector3 direction;
//        public Vector3 Direction { get => direction; set => direction = value; }
//        public float ConeAngle;
//        public float LinearAttenuation;
//        public float QuadraticAttenuation;
//        public float orthoWidth = 4.0f, orthoHeight = 4.0f, orthoNear = 0.1f, orthoFar = 1000.0f;
//        public Vector3 Color = new(1.0f);

//        public Light()
//        {
//            Scale = 0.1f;
//        }

//        public enum LightType
//        {
//            PointLightCube,
//            SpotLightCube,
//            DirectionalCube
//        }

//        internal static Light GetLight(GL gl, LightType lightType) => lightType switch
//        {
//            LightType.PointLightCube => GetPointLightCube(gl),
//            LightType.SpotLightCube => GetSpotLightCube(gl),
//            LightType.DirectionalCube => GetDirectionalLightCube(gl)
//        };

//        private static Light GetDirectionalLightCube(GL gl)
//        {
//            Light l = new Light();
//            l.Direction = -Vector3.UnitY;
//            l.LightTypeEnum = LightType.DirectionalCube;
//            l.vao = UnitCube.GetVertexArrayObject(gl);
//            return l;
//        }

//        private static Light GetSpotLightCube(GL gl)
//        {
//            Light l = new Light();
//            l.Direction = Vector3.UnitX;
//            l.LightTypeEnum = LightType.SpotLightCube;
//            l.ConeAngle = 15.0f;
//            l.vao = UnitCube.GetVertexArrayObject(gl);
//            return l;
//        }

//        private static Light GetPointLightCube(GL gl)
//        {
//            Light l = new Light();
//            l.LightTypeEnum = LightType.PointLightCube;
//            l.vao = UnitCube.GetVertexArrayObject(gl);
//            return l;
//        }

//        internal void vaoBind()
//        {
//            vao.Bind();
//        }

//        // perhaps swap matrices
//        internal Matrix4x4 GetLightSpaceMatrix(Vector2 ScreenSize, float near = 0.1f, float far = 1000.0f) => GetLightView() * GetLightProjection(ScreenSize, near, far);
//        private Matrix4x4 GetLightView() => Matrix4x4.CreateLookAt(Position, Position + Direction, Vector3.UnitY);

//        private Matrix4x4 GetLightProjection(Vector2 ScreenSize, float near, float far) =>Matrix4x4.CreateOrthographicOffCenter(-ScreenSize.X/2f, ScreenSize.X/2f, -ScreenSize.Y/2f, ScreenSize.Y/2, near, far);
//    }
//}

