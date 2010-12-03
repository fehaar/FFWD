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
    public class ManualScriptWriter : IFilteredComponentWriter
    {
        public Filter filter { get; set; }

        #region IComponentWriter Members
        public void Write(SceneWriter scene, object component)
        {
            MonoBehaviour beh = component as MonoBehaviour;
            if (beh == null)
            {
                throw new Exception(GetType() + " cannot export components of type " + component.GetType());
            }

            if (filter != null)
            {
                Type t = component.GetType();
                FieldInfo[] memInfo = t.GetFields(BindingFlags.Public | BindingFlags.Instance);                
                for (int m = 0; m < memInfo.Length; m++)
                {
                    if (memInfo[m].GetCustomAttributes(typeof(HideInInspector), true).Length > 0)
                    {
                        continue;
                    }
                    if (filter.Includes(memInfo[m].Name))
                    {
                        scene.WriteElement(memInfo[m].Name, memInfo[m].GetValue(component));
                    }
                }                
            }
            scene.WriteScriptStub(beh);

        }
        #endregion
    }
}
