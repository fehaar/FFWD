using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;
using System.Reflection;

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
            if (coll.sharedMaterial != null)
            {
                if (coll.sharedMaterial.name.Contains("(Instance)"))
                {
                    UnityEditor.EditorUtility.ResetToPrefabState(coll);
                }
                else
                {
                    scene.WriteElement("material", coll.sharedMaterial.name);
                }
            }
        }
        #endregion
    }
}
