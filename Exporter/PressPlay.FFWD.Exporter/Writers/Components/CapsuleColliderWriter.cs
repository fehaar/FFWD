using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PressPlay.FFWD.Exporter.Writers.Components
{
    public class CapsuleColliderWriter : ColliderWriter
    {
        public override void Write(SceneWriter scene, object collider)
        {
            base.Write(scene, collider);
            CapsuleCollider coll = collider as CapsuleCollider;
            if (coll == null)
            {
                throw new Exception(GetType() + " cannot export components of type " + collider.GetType());
            }
            scene.WriteElement("center", coll.center);
            scene.WriteElement("radius", coll.radius);
            scene.WriteElement("height", coll.height);
            scene.WriteElement("direction", coll.direction);
        }
    }
}
