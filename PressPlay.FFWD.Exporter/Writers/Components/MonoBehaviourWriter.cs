using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;
using System.Reflection;

namespace PressPlay.FFWD.Exporter.Writers.Components
{
    public class MonoBehaviourWriter : IComponentWriter
    {
        #region IComponentWriter Members
        public void Write(SceneWriter scene, object component)
        {
            MonoBehaviour beh = component as MonoBehaviour;
            if (beh == null)
            {
                throw new Exception(GetType() + " cannot export components of type " + component.GetType());
            }
            Type t = component.GetType();
            FieldInfo[] memInfo = t.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            for (int m = 0; m < memInfo.Length; m++)
            {
                scene.WriteElement(memInfo[m].Name, memInfo[m].GetValue(component));
            }
            scene.WriteScript(beh);
        }
        #endregion
    }
}
