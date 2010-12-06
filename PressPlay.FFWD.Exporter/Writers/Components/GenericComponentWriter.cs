using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;
using System.Reflection;

namespace PressPlay.FFWD.Exporter.Writers.Components
{
    /// <summary>
    /// This writer is used to export data for components that need no special handling
    /// </summary>
    public class GenericComponentWriter : IFilteredComponentWriter
    {
        public Filter filter { get; set; }

        #region IComponentWriter Members
        public void Write(SceneWriter scene, object component)
        {
            Component beh = component as Component;
            if (beh == null)
            {
                throw new Exception(GetType() + " cannot export components of type " + component.GetType());
            }

            if (filter != null)
            {
                WriteFieldsForType(scene, beh, component.GetType());
            }
        }

        private void WriteFieldsForType(SceneWriter scene, Component component, Type t)
        {
            if (t != typeof(Component))
            {
                WriteFieldsForType(scene, component, t.BaseType);
            }
            FieldInfo[] memInfo = t.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
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
        #endregion
    }
}
