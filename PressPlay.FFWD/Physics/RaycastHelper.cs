using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.Components;
using PressPlay.FFWD.Extensions;

namespace PressPlay.FFWD
{
    internal class RaycastHelper
    {
        public RaycastHelper(float distance, bool findClosest)
        {
            this.distance = distance;
            this.findClosest = findClosest;
        }

        private bool findClosest = false;
        private float distance;
        private List<RaycastHit> _hits = new List<RaycastHit>();


        internal float rayCastCallback(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
        {
            float dist = distance * fraction;
            if (findClosest)
            {
                _hits.Clear();
                _hits.Add(new RaycastHit() { body = fixture.GetBody(), point = point.To3d(), normal = normal.To3d(), distance = dist, collider = fixture.GetBody().GetUserData() as MeshCollider });
                return fraction;
            }
            else
            {
                _hits.Add(new RaycastHit() { body = fixture.GetBody(), point = point.To3d(), normal = normal.To3d(), distance = dist, collider = fixture.GetBody().GetUserData() as MeshCollider });
                return 1;
            }
        }   

        internal int HitCount
        {
            get
            {
                return _hits.Count;
            }
        }

        internal RaycastHit[] Hits
        {
            get
            {
                return _hits.ToArray();
            }
        }

        internal RaycastHit ClosestHit()
        {
            return _hits.LastOrDefault();
        }
    }
}
