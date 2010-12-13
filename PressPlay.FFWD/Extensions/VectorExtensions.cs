using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Extensions
{
    public static class VectorExtensions
    {
        public enum To2dMode { DropX, DropY, DropZ };
        public static To2dMode to2dMode = To2dMode.DropY;

        public static Vector2 To2d(this Vector3 vector)
        {
            switch (to2dMode)
            {
                case To2dMode.DropX:
                    return new Vector2(vector.Y, vector.Z);
                case To2dMode.DropY:
                    return new Vector2(vector.X, vector.Z);
                case To2dMode.DropZ:
                    return new Vector2(vector.X, vector.Y);
                default:
                    throw new Exception("Unknown enum " + to2dMode);
            }
        }

        public static Vector3 To3d(this Vector2 vector)
        {
            switch (to2dMode)
            {
                case To2dMode.DropX:
                    return new Vector3(0, vector.X, vector.Y);
                case To2dMode.DropY:
                    return new Vector3(vector.X, 0, vector.Y);
                case To2dMode.DropZ:
                    return new Vector3(vector.X, vector.Y, 0);
                default:
                    throw new Exception("Unknown enum " + to2dMode);
            }
        }
    }
}
