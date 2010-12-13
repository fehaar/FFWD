using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD
{
    public struct RaycastHit
    {
        public Body body;
        public Vector3 point;
        public Vector3 normal;
        public float distance;
        public Transform transform;
        public Collider collider;
    }
}
