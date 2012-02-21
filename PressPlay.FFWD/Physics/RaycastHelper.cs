using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.Components;
using PressPlay.FFWD;

namespace PressPlay.FFWD
{
    /// <summary>
    /// This is a helper class that reduces the amount of garbage that is created when doing raycasts and making delegates.
    /// </summary>
    internal static class RaycastHelper
    {
        private static bool _findClosest;
        private static float _distance;
        private static int _layerMask;
        private static RaycastHit[] _hits = new RaycastHit[ApplicationSettings.DefaultCapacities.RaycastHits];
        private static int _hitCount;
        private static RaycastHit _hit;
        private static float _nearest = float.PositiveInfinity;
        private static bool _didHit = false;

        public static void SetValues(float distance, bool findClosest, int layerMask)
        {
            _distance = distance;
            _findClosest = findClosest;
            _layerMask = layerMask;
            _hitCount = 0;
            _nearest = float.PositiveInfinity;
            _didHit = false;;
        }

        internal static float rayCastCallback(Fixture fixture, Microsoft.Xna.Framework.Vector2 point, Microsoft.Xna.Framework.Vector2 normal, float fraction)
        {
            float dist = _distance * fraction;
            Component uo = fixture.Body.UserData;
            Collider coll = uo as Collider;

            if (coll == null && (uo is Rigidbody))
            {
                coll = (uo as Rigidbody).collider;
            }
            if ((coll != null) && (coll.gameObject != null) && (coll.gameObject.active) && (_layerMask & (1 << coll.gameObject.layer)) > 0)
            {
                if (_findClosest)
                {
                    if (dist < _nearest)
                    {
                        _nearest = dist;
                        _hit.body = fixture.Body;
                        _hit.point = VectorConverter.Convert(point, coll.to2dMode);
                        _hit.normal = VectorConverter.Convert(normal, coll.to2dMode);
                        _hit.distance = dist;
                        _hit.collider = coll;
                        _hit.transform = coll.transform;
                        _didHit = true;
                    }
                    return fraction;
                }
                else
                {
                    _hits[_hitCount].body = fixture.Body;
                    _hits[_hitCount].collider = coll;
                    _hits[_hitCount].distance = dist;
                    _hits[_hitCount].normal = VectorConverter.Convert(normal, coll.to2dMode);
                    _hits[_hitCount].point = VectorConverter.Convert(point, coll.to2dMode);
                    _hits[_hitCount].transform = coll.transform;
                    _hitCount++;
                    return 1;
                }
            }
            return -1;
        }   

        internal static int HitCount
        {
            get
            {
                if (_findClosest && _didHit)
                {
                    return 1;
                }

                return _hitCount;
            }
        }

        internal static RaycastHit[] Hits
        {
            get
            {
                return _hits.Take(_hitCount).ToArray();
            }
        }

        internal static RaycastHit[] HitsByDistance
        {
            get
            {
                return _hits.Take(_hitCount).OrderByDescending(h => h.distance).ToArray();
            }
        }

        internal static RaycastHit ClosestHit()
        {
            return _hit;
        }
    }
}
