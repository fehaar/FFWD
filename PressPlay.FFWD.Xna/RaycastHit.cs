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
        public Vector2 point;
        public Vector2 normal;
        public float distance;
        public Transform transform;
        public MeshCollider collider;
    }
}
