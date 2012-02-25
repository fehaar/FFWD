using System;
using System.Linq;
using System.Reflection;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;
using System.Collections.Generic;

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
            if (!options.Contains("noExport"))
            {
                scene.WriteScript(beh, !options.Contains("noOverwrite"));
            }
        }
        #endregion

        private void WriteFieldsForType(SceneWriter scene, MonoBehaviour component, Type t)
        {
            if (t != typeof(Behaviour))
            {
                WriteFieldsForType(scene, component, t.BaseType);
            }
            if (t == typeof(Behaviour))
            {
                if (!component.enabled)
                {
                    scene.WriteElement("enabled", component.enabled);
                }
                return;
            }
            scene.WriteMembers(component, t, filter);
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
