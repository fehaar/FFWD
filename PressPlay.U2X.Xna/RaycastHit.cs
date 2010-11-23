using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
using Microsoft.Xna.Framework;

namespace PressPlay.U2X.Xna
{
    public class RaycastHit
    {
        public Body body;
        public Vector2 point;
        public Vector2 normal;
        public float distance;
    }
}
