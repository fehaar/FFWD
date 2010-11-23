using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
using Microsoft.Xna.Framework;

namespace PressPlay.U2X.Xna
{
    internal class RaycastHelper
    {
        public RaycastHelper(float distance)
        {
            this.distance = distance;
        }

        private float distance;
        private List<RaycastHit> _hits = new List<RaycastHit>();

        internal float rayCastCallback(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
        {
            _hits.Add(new RaycastHit() { body = fixture._body, point = point, normal = normal, distance = distance * fraction });
            return 0;
        }

        internal int HitCount
        {
            get
            {
                return _hits.Count;
            }
        }

        internal RaycastHit ClosestHit()
        {
            return _hits.ElementAtOrDefault(0);
        }
    }
}
