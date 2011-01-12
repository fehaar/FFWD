using System;
using System.Reflection;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;

namespace PressPlay.FFWD.Exporter.Writers.Components
{
    public class MonoBehaviourWriter : IFilteredComponentWriter, IOptionComponentWriter
    {
        public Filter filter { get; set; }

        public Filter.FilterType defaultFilterType
        {
            get
            {
                return Filter.FilterType.IncludeAll;
            }
        }

        #region IComponentWriter Members
        public virtual void Write(SceneWriter scene, object component)
        {
            MonoBehaviour beh = component as MonoBehaviour;
            if (beh == null)
            {
                throw new Exception(GetType() + " cannot export components of type " + component.GetType());
            }

            if (filter != null)
            {
                WriteFieldsForType(scene, beh, component.GetType());
            }
            scene.WriteScript(beh, !options.Contains("noOverwrite"));
        }
        #endregion

        private void WriteFieldsForType(SceneWriter scene, MonoBehaviour component, Type t)
        {
            if (t != typeof(MonoBehaviour))
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
                    scene.WriteElement(memInfo[m].Name, memInfo[m].GetValue(component), memInfo[m].FieldType);
                }
            }
        }

        protected virtual void WriteElement(SceneWriter scene, string name, object value)
        {
            scene.WriteElement(name, value);
        }

        #region IOptionComponentWriter Members
        public string options { get; set; }
        #endregion
    }
}
