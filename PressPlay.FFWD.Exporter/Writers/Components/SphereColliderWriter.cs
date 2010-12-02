using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PressPlay.FFWD.Exporter.Writers.Components
{
    public class SphereColliderWriter : ColliderWriter
    {
        public override void Write(SceneWriter scene, object collider)
        {
            base.Write(scene, collider);
            SphereCollider coll = collider as SphereCollider;
            if (coll == null)
            {
                throw new Exception(GetType() + " cannot export components of type " + collider.GetType());
            }
            scene.WriteElement("center", coll.center);
            scene.WriteElement("radius", coll.radius);
        }
    }
}
