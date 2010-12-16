using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD
{
    public static class PlaneExtensions
    {
        public static bool Raycast(this Plane plane, Ray ray, out float enter)
        {
            float? dist = ray.Intersects(plane);
            if (dist.HasValue)
            {
                enter = dist.Value;
                return true;
            }
            enter = 0.0f;
            return false;
        }
    }
}
