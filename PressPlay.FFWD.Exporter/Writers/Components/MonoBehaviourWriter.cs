using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;
using System.Reflection;

namespace PressPlay.FFWD.Exporter.Writers.Components
{
    public class MonoBehaviourWriter : IComponentWriter, IOptionComponentWriter
    {
        public List<string> memberFilter = new List<string>();
        public bool filterIsExclude = true;
        public bool exportScript = true;

        #region IComponentWriter Members
        public virtual void Write(SceneWriter scene, object component)
        {
            MonoBehaviour beh = component as MonoBehaviour;
            if (beh == null)
            {
                throw new Exception(GetType() + " cannot export components of type " + component.GetType());
            }
            Type t = component.GetType();
            FieldInfo[] memInfo = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (int m = 0; m < memInfo.Length; m++)
            {
                if (filterIsExclude && memberFilter.Contains(memInfo[m].Name))
                {
                    continue;
                }
                if (!filterIsExclude && !memberFilter.Contains(memInfo[m].Name))
                {
                    continue;
                }
                WriteElement(scene, memInfo[m].Name, memInfo[m].GetValue(component));
            }
            if (exportScript)
            {
                scene.WriteScript(beh, !options.Contains("noOverwrite"));
            }
        }
        #endregion

        protected virtual void WriteElement(SceneWriter scene, string name, object value)
        {
            scene.WriteElement(name, value);
        }

        #region IOptionComponentWriter Members
        public string options { get; set; }
        #endregion
    }
}
