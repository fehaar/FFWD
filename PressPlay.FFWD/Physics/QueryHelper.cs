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
        private static List<Collider> colliders = new List<Collider>(20);
        public static LayerMask layermask;

        internal static bool QueryCallback(Fixture fixture)
        {
            Component uo = fixture.Body.UserData;
            Collider coll = uo as Collider;

            if (coll == null && (uo is Rigidbody))
            {
                coll = (uo as Rigidbody).collider;
            }

            if ((bool)coll && (layermask & (1 << coll.gameObject.layer)) > 0)
            {
                colliders.Add(coll);
            }
            
            return true;
        }

        internal static Collider[] GetQueryResult()
        {
            Collider[] cols = colliders.ToArray();
            colliders.Clear();
            return cols;
        }
    }
}
