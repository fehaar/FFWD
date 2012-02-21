using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;

namespace PressPlay.FFWD.Exporter.Writers.Components
{
    public class BoxColliderWriter : ColliderWriter
    {
        public override void Write(SceneWriter scene, object collider)
        {
            base.Write(scene, collider);
            BoxCollider coll = collider as BoxCollider;
            if (coll == null)
            {
                throw new Exception(GetType() + " cannot export components of type " + collider.GetType());
            }
            scene.WriteElement("center", coll.center);
            scene.WriteElement("size", coll.size);
        }

    }
}
