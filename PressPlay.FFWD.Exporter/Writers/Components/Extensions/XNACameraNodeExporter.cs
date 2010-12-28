using System;
using System.Collections.Generic;
using System.Reflection;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;

namespace PressPlay.FFWD.Exporter.Writers.Components.Extensions
{
    public class XNACameraNodeExporter : IComponentWriter
    {
        #region IComponentWriter Members
        public void Write(SceneWriter scene, object component)
        {
            MonoBehaviour script = component as MonoBehaviour;
            if (script == null)
            {
                throw new Exception(GetType() + " cannot export components of type " + component.GetType());
            }
            // Gather all nodes under this and export the data as a dictionary
            Dictionary<int, int> nodeRefs = new Dictionary<int, int>();
            Type findType = component.GetType().GetField("component").FieldType;
            string propertyName = component.GetType().GetField("propertyName").GetValue(component).ToString();
            Component[] nodes = script.GetComponentsInChildren(findType);
            foreach (Component node in nodes)
            {
                FieldInfo field = node.GetType().GetField(propertyName);
                UnityEngine.Object obj = (field.GetValue(node) as UnityEngine.Object);
                if (obj != null)
	            {
                    nodeRefs.Add(node.GetInstanceID(), obj.GetInstanceID());
	            }
            }
            scene.WriteElement("nodes", nodeRefs);
        }
        #endregion
    }
}
