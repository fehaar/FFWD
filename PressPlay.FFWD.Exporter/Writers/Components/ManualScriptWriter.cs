using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;
using System.Reflection;

namespace PressPlay.FFWD.Exporter.Writers.Components
{
    /// <summary>
    /// This writer is used to export data for scripts that will be created and handled manually in XNA.
    /// </summary>
    public class ManualScriptWriter : IComponentWriter
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
            FieldInfo[] memInfo = t.GetFields(BindingFlags.Public | BindingFlags.Instance);

            scene.WriteScriptStub(beh);

            //for (int m = 0; m < memInfo.Length; m++)
            //{
            //    if (filterIsExclude && memberFilter.Contains(memInfo[m].Name))
            //    {
            //        continue;
            //    }
            //    if (!filterIsExclude && !memberFilter.Contains(memInfo[m].Name))
            //    {
            //        continue;
            //    }
            //    WriteElement(scene, memInfo[m].Name, memInfo[m].GetValue(component));
            //}
        }
        #endregion
    }
}
