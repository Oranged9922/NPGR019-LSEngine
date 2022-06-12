using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LSEngine
{
    internal class Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Up { get; private set; }
        public Vector3 Front { get; set; }

        public float AspectRatio { get; set; }
        public float Fov = 45f;
        public float Yaw { get; set; } = -90f;
        public float Pitch { get; set; } = 0f;
        public float Near { get; set; } = 0.1f;
        public float Far { get; set; } = 1000f;

        public Camera(Vector3 pos, Vector3 front, Vector3 up, float aspRat)
        {
            Position = pos;
            Front = front;
            Up = up;
            AspectRatio = aspRat;
        }

        public void ModifyFOV(float fov)
        {
            Fov = Math.Clamp(Fov-fov, 1.0f, 120f);
        }

        public void ModifyDirection(float xOffset, float yOffset)
        {
            Yaw += xOffset;
            Pitch -= yOffset;

            Pitch = Math.Clamp(Pitch, -89.9f, 89.9f);

            var camDir = Vector3.Zero;
            camDir.X = (float)Math.Cos(Math.PI/180f * Yaw) * (float)Math.Cos(Math.PI / 180f * Pitch);
            camDir.Y = (float)Math.Sin(Math.PI / 180f * Pitch);
            camDir.Z = (float)Math.Sin(Math.PI / 180f * Yaw) * (float)Math.Cos(Math.PI / 180f * Pitch);

            Front = Vector3.Normalize(camDir);
        }

        // Get View Matrix
        public Matrix4x4 GetViewMatrix()
        {
            return Matrix4x4.CreateLookAt(Position, Position + Front, Up);
        }

        // Get Projection Matrix
        public Matrix4x4 GetProjectionMatrix()
        {
            return Matrix4x4.CreatePerspectiveFieldOfView((float)(Math.PI / 180f * Fov), AspectRatio,Near,Far);
        }
    }
}
