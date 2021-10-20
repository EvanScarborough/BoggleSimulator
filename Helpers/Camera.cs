using System;
using Microsoft.Xna.Framework;

namespace Boggle.Helpers
{
    public class Camera
    {
        public Vector2 Position { get; set; }
        public float Zoom { get; set; } = 50;

        public Camera()
        {
        }
    }
}
