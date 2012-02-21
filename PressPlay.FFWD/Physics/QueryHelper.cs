using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD
{
    internal static class QueryHelper
    {
        private static Collider[] colliders = new Collider[ApplicationSettings.DefaultCapacities.QueryHelper];
        private static int colliderCount = 0;
        internal static LayerMask layermask;
        internal static bool breakOnFirst;

        private static RaycastHit _hit;
        //private static List<RaycastHit> hits = new List<RaycastHit>(ApplicationSettings.QueryHelperDefaultCapacity);

        internal static bool QueryCallback(Fixture fixture)
        {
            Component uo = fixture.Body.UserData;
            Collider coll = uo as Collider;

            if (coll == null && (uo is Rigidbody))
            {
                coll = (uo as Rigidbody).collider;
            }

            if ((bool)coll && fixture.Body.Enabled && coll.gameObject.active && (layermask & (1 << coll.gameObject.layer)) > 0)
            {
                colliders[colliderCount++] = coll;
                _hit = new RaycastHit() { body = fixture.Body, collider = coll, transform = coll.transform };
                if (breakOnFirst)
                {
                    return false;
                }
            }
            
            return true;
        }

        internal static RaycastHit ClosestHit()
        {
            return _hit;
        }

        internal static Collider[] GetQueryResult()
        {
            Collider[] cols = colliders.Take(colliderCount).ToArray();
            colliderCount = 0;
            layermask = Physics.kDefaultRaycastLayers;
            breakOnFirst = false;
            return cols;
        }
    }
}
