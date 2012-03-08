using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;

namespace PressPlay.FFWD.Exporter.Writers.Components
{
    public class RigidbodyWriter : IComponentWriter
    {
        #region IComponentWriter Members
        public void Write(SceneWriter scene, object component)
        {
            Rigidbody body = component as Rigidbody;
            if (body == null)
            {
                throw new Exception(GetType() + " cannot export components of type " + component.GetType());
            }
            scene.WriteElement("mass", body.mass);
            scene.WriteElement("drag", body.drag);
            scene.WriteElement("angularDrag", body.angularDrag);
            if (body.freezeRotation)
            {
                scene.WriteElement("freezeRotation", body.freezeRotation);
            }
            if (body.isKinematic)
            {
                scene.WriteElement("isKinematic", body.isKinematic);
            }
        }
        #endregion
    }
}
