using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public static class VectorExtensions
    {
        public static Microsoft.Xna.Framework.Vector2 To2d(this Microsoft.Xna.Framework.Vector3 vector)
        {
            switch (ApplicationSettings.to2dMode)
            {
                case ApplicationSettings.To2dMode.DropX:
                    return new Microsoft.Xna.Framework.Vector2(vector.Y, vector.Z);
                case ApplicationSettings.To2dMode.DropY:
                    return new Microsoft.Xna.Framework.Vector2(vector.X, vector.Z);
                case ApplicationSettings.To2dMode.DropZ:
                    return new Microsoft.Xna.Framework.Vector2(vector.X, vector.Y);
                default:
                    throw new Exception("Unknown enum " + ApplicationSettings.to2dMode);
            }
        }

        public static Microsoft.Xna.Framework.Vector3 To3d(this Microsoft.Xna.Framework.Vector2 vector)
        {
            switch (ApplicationSettings.to2dMode)
            {
                case ApplicationSettings.To2dMode.DropX:
                    return new Microsoft.Xna.Framework.Vector3(0, vector.X, vector.Y);
                case ApplicationSettings.To2dMode.DropY:
                    return new Microsoft.Xna.Framework.Vector3(vector.X, 0, vector.Y);
                case ApplicationSettings.To2dMode.DropZ:
                    return new Microsoft.Xna.Framework.Vector3(vector.X, vector.Y, 0);
                default:
                    throw new Exception("Unknown enum " + ApplicationSettings.to2dMode);
            }
        }

        public static Microsoft.Xna.Framework.Vector3 UnityNormalize(this Microsoft.Xna.Framework.Vector3 vector)
        {
            if (vector == Microsoft.Xna.Framework.Vector3.Zero)
            {
                return Microsoft.Xna.Framework.Vector3.Zero;
            }
            return Microsoft.Xna.Framework.Vector3.Normalize(vector);
        }
    }
}
