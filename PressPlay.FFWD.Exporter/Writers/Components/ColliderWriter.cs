using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;

namespace PressPlay.FFWD.Exporter.Writers.Components
{
    public class ColliderWriter : IComponentWriter
    {
        #region IComponentWriter Members
        public virtual void Write(SceneWriter scene, object component)
        {
            Collider coll = component as Collider;
            if (coll == null)
            {
                throw new Exception(GetType() + " cannot export components of type " + component.GetType());
            }
            scene.WriteElement("isTrigger", coll.isTrigger);
            scene.WriteElement("material", coll.material.name.Replace(" (Instance)", ""));
        }
        #endregion
    }
}
