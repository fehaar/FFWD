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
    }
}
