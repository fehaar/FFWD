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

        private bool findClosest;
        private float distance;
        private int layerMask;
        private List<RaycastHit> _hits = new List<RaycastHit>();
        private RaycastHit _hit;
        private float nearest = float.PositiveInfinity;
        private bool didHit = false;

        public void SetValues(float distance, bool findClosest, int layerMask)
        {
            this.distance = distance;
            this.findClosest = findClosest;
            this.layerMask = layerMask;

            _hits.Clear();
            nearest = float.PositiveInfinity;
            didHit = false;;
        }

        internal float rayCastCallback(Fixture fixture, PressPlay.FFWD.Vector2 point, PressPlay.FFWD.Vector2 normal, float fraction)
        {
            float dist = distance * fraction;
            UnityObject uo = fixture.GetBody().GetUserData() as UnityObject;
            Collider coll = uo as Collider;

            if (coll == null && (uo is Rigidbody))
            {
                coll = (uo as Rigidbody).collider;
            }
            if ((coll != null) && (coll.gameObject != null) && (layerMask & (1 << coll.gameObject.layer)) > 0)
            {
                if (findClosest)
                {
                    if (dist < nearest)
                    {
                        nearest = dist;
                        _hit = new RaycastHit() { body = fixture.GetBody(), point = point, normal = normal, distance = dist, collider = coll };
                        didHit = true;
                    }
                    return fraction;
                }
                else
                {
                    _hits.Add(new RaycastHit() { body = fixture.GetBody(), point = point, normal = normal, distance = dist, collider = coll });
                    return 1;
                }
            }
            return 1;
        }   

        internal int HitCount
        {
            get
            {
                if (findClosest && didHit)
                {
                    return 1;
                }

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
            return _hit;
        }

        public bool pointCastCallback(Fixture fixture)
        {
            UnityObject uo = fixture.GetBody().GetUserData() as UnityObject;
            Collider coll = uo as Collider;
            if (coll == null && (uo is Rigidbody))
            {
                coll = (uo as Rigidbody).collider;
            }
            if ((coll != null) && (coll.gameObject != null) && (layerMask & (1 << coll.gameObject.layer)) > 0)
            {
                didHit = true;
                _hit = new RaycastHit() { body = fixture.GetBody(), collider = coll };
            }
            return true;
        }

    }
}
