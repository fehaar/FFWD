using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.Components;
using PressPlay.FFWD;

namespace PressPlay.FFWD
{
    internal class RaycastHelper
    {
        public RaycastHelper(float distance, bool findClosest, int layerMask)
        {
            this.distance = distance;
            this.findClosest = findClosest;
            this.layerMask = layerMask;
        }

        private bool findClosest = false;
        private float distance;
        private int layerMask;
        private List<RaycastHit> _hits = new List<RaycastHit>();

        internal float rayCastCallback(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
        {
            float dist = distance * fraction;
            UnityObject uo = fixture.GetBody().GetUserData() as UnityObject;
            Collider coll = uo as Collider;
            if (coll == null && (uo is Rigidbody))
            {
                coll = (uo as Rigidbody).collider;
            }
            if ((coll == null) || (layerMask & coll.gameObject.layer) > 0)
            {
                if (findClosest)
                {
                    _hits.Clear();
                    _hits.Add(new RaycastHit() { body = fixture.GetBody(), point = point.To3d(), normal = normal.To3d(), distance = dist, collider = coll });
                    return fraction;
                }
                else
                {
                    _hits.Add(new RaycastHit() { body = fixture.GetBody(), point = point.To3d(), normal = normal.To3d(), distance = dist, collider = coll });
                    return 1;
                }
            }
            return 1;
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

        public bool pointCastCallback(Fixture fixture)
        {
            UnityObject uo = fixture.GetBody().GetUserData() as UnityObject;
            Collider coll = uo as Collider;
            if (coll == null && (uo is Rigidbody))
            {
                coll = (uo as Rigidbody).collider;
            }
            if ((coll == null) || (layerMask & coll.gameObject.layer) > 0)
            {
                _hits.Add(new RaycastHit() { body = fixture.GetBody(), collider = coll });
                return !findClosest;
            }
            return false;
        }

    }
}
