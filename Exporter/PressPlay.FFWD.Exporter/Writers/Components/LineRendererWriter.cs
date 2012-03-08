using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEngine;
using System.Reflection;

namespace PressPlay.FFWD.Exporter.Writers.Components
{
    internal class LineRendererWriter : IComponentWriter
    {
        #region IComponentWriter Members

        public void Write(SceneWriter writer, object component)
        {
            LineRenderer pr = component as LineRenderer;
            if (pr == null)
            {
                throw new Exception(GetType() + " cannot export components of type " + component.GetType());
            }
            writer.WriteElement("enabled", pr.enabled);
            writer.WriteElement("sharedMaterials", pr.sharedMaterials);

            writer.WriteElement("useWorldSpace", pr.useWorldSpace);

            Component c = pr.gameObject.GetComponent("XNALineRendererExport");
            if (c != null)
            {
                writeValue<object>(writer, c, "startWidth");
                writeValue<object>(writer, c, "endWidth");
                writeValue<object>(writer, c, "startColor");
                writeValue<object>(writer, c, "endColor");
                
                writeValue<object>(writer, c, "positions");
            }
        }
        #endregion

        private void writeValue<T>(SceneWriter writer, Component c, string fieldName)
        {
            Type t = c.GetType();
            FieldInfo f = t.GetField(fieldName);
            object value = f.GetValue(c);
            
            writer.WriteElement(fieldName, value);
        }
    }
}
