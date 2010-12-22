using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEditor;
using System.IO;
using PressPlay.FFWD.Exporter.Interfaces;
using System.Globalization;
using System.Reflection;

namespace PressPlay.FFWD.Exporter.Writers
{
    public class SceneWriter
    {
        public SceneWriter(TypeResolver resolver, AssetHelper assets)
        {
            this.resolver = resolver;
            assetHelper = assets;
        }

        private TypeResolver resolver;
        private AssetHelper assetHelper;

        public string ExportDir { get; set; }
        public bool FlipYInTransforms { get; set; }

        private XmlWriter writer = null;

        private List<GameObject> Prefabs = new List<GameObject>();
        private List<int> writtenIds = new List<int>();

        public void Write(string path)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            using (writer = XmlWriter.Create(path, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("XnaContent");
                writer.WriteStartElement("Asset");
                writer.WriteAttributeString("Type", resolver.DefaultNamespace + ".Scene");
                WriteGOs();
                WritePrefabs();
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        private void WriteGOs()
        {
            UnityEngine.Object[] objs = GameObject.FindObjectsOfType(typeof(GameObject));

            for (int i = 0; i < objs.Length; i++)
            {
                GameObject go = objs[i] as GameObject;
                if (go.transform.parent != null)
                {
                    continue;
                }
                if (writtenIds.Contains(go.GetInstanceID()))
                {
                    continue;
                }
                writer.WriteStartElement("gameObject");
                WriteGameObject(go);
                writer.WriteEndElement();
            }
        }

        private void WritePrefabs()
        {
            for (int i = 0; i < Prefabs.Count; i++)
            {
                if (writtenIds.Contains(Prefabs[i].GetInstanceID()))
                {
                    continue;
                }
                writtenIds.Add(Prefabs[i].GetInstanceID());
                writer.WriteStartElement("prefab");
                WriteGameObject(Prefabs[i]);
                writer.WriteEndElement();
            }
        }

        private void WriteGameObject(GameObject go)
        {
            writtenIds.Add(go.GetInstanceID());
            writer.WriteElementString("id", go.GetInstanceID().ToString());
            writer.WriteElementString("name", go.name);            
            writer.WriteElementString("layer", ToString(go.layer));
            writer.WriteElementString("active", ToString(go.active));
            writer.WriteElementString("tag", go.tag);
            writer.WriteStartElement("components");
            Component[] comps = go.GetComponents(typeof(Component));
            for (int i = 0; i < comps.Length; i++)
            {
                WriteComponent(comps[i], false);
            }
            writer.WriteEndElement();
        }

        private void WriteTransform(Transform transform, bool isPrefab)
        {
            Vector3 pos = transform.localPosition;
            if (FlipYInTransforms)
            {
                pos.y = -pos.y;
            }
            if (!isPrefab)
            {
                writer.WriteStartElement("component");
                writer.WriteAttributeString("Type", "PressPlay.FFWD.Transform");
            }
            writer.WriteElementString("id", transform.GetInstanceID().ToString());
            if (isPrefab)
            {
                writer.WriteElementString("isPrefab", ToString(true));
            }
            writer.WriteElementString("localPosition", ToString(pos));
            writer.WriteElementString("localScale", ToString(transform.localScale));
            writer.WriteElementString("localRotation", ToString(transform.localRotation));
            if (!isPrefab && transform.childCount > 0)
            {
                writer.WriteStartElement("children");
                for (int i = 0; i < transform.childCount; i++)
                {
                    writer.WriteStartElement("child");
                    WriteGameObject(transform.GetChild(i).gameObject);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            if (!isPrefab)
            {
                writer.WriteEndElement();
            }
        }

        private void WriteComponent(Component component, bool isPrefab)
        {
            if (resolver == null)
            {
                return;
            }
            if (component == null)
            {
                return;
            }
            if (component is Transform)
            {
                WriteTransform(component as Transform, isPrefab);
                return;
            }
            if (resolver.SkipComponent(component))
            {
                return;
            }

            System.Type type = component.GetType();
            IComponentWriter componentWriter = resolver.GetComponentWriter(type);
            if (componentWriter != null)
            {
                writtenIds.Add(component.GetInstanceID());
                if (!isPrefab)
                {
                    writer.WriteStartElement("component");
                    writer.WriteAttributeString("Type", resolver.ResolveTypeName(component));
                }
                writer.WriteElementString("id", component.GetInstanceID().ToString());
                if (isPrefab)
                {
                    writer.WriteElementString("isPrefab", ToString(true));
                }
                componentWriter.Write(this, component);
                if (!isPrefab)
                {
                    writer.WriteEndElement();
                }
            }
        }

        internal void WriteTexture(Texture texture)
        {
            writer.WriteElementString("texture", texture.name);
            assetHelper.ExportTexture(texture as Texture2D);
        }

        internal void WriteScript(MonoBehaviour component, bool overwrite)
        {
            assetHelper.ExportScript(component, false, overwrite);
            // Check for base classes
            Type tp = component.GetType().BaseType;
            if (tp != typeof(MonoBehaviour))
            {
                WriteScript(component.gameObject.AddComponent(tp) as MonoBehaviour, overwrite);
            }
        }

        internal void WriteScriptStub(MonoBehaviour component)
        {
            assetHelper.ExportScript(component, true, false);
            // Check for base classes
            Type tp = component.GetType().BaseType;
            if (tp != typeof(MonoBehaviour))
            {
                WriteScriptStub(component.gameObject.AddComponent(tp) as MonoBehaviour);
            }
        }

        internal void WriteMesh(Mesh mesh, string name)
        {
            string asset = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(mesh.GetInstanceID()));
            writer.WriteStartElement(name);
            writer.WriteElementString("name", mesh.name);
            writer.WriteElementString("asset", asset);
            writer.WriteEndElement();
            assetHelper.ExportMesh(mesh);
        }

        internal void WriteElement(string name, object obj)
        {
            try
            {
                if (obj == null)
                {
                    writer.WriteStartElement(name);
                    writer.WriteAttributeString("Null", ToString(true));
                    writer.WriteEndElement();
                    return;
                }
                if (obj is float)
                {
                    writer.WriteElementString(name, ToString((float)obj));
                    return;
                }
                if (obj is Boolean)
                {
                    writer.WriteElementString(name, ToString((Boolean)obj));
                    return;
                }
                if (obj is int)
                {
                    writer.WriteElementString(name, ToString((int)obj));
                    return;
                }
                if (obj is int[])
                {
                    writer.WriteElementString(name, ToString(obj as int[]));
                    return;
                }
                if (obj is Vector3)
                {
                    writer.WriteElementString(name, ToString((Vector3)obj));
                    return;
                }
                if (obj is Vector3[])
                {
                    writer.WriteElementString(name, ToString(obj as Vector3[]));
                    return;
                }
                if (obj is LayerMask)
                {
                    writer.WriteElementString(name, ((LayerMask)obj).value.ToString());
                    return;
                }
                if (obj is Material[])
                {
                    Material[] objArr = obj as Material[];
                    writer.WriteStartElement(name);
                    foreach (Material mat in objArr)
                    {
                        WriteElement("material", mat);
                    }
                    writer.WriteEndElement();
                    return;
                }
                if (obj is Material)
                {
                    Material mat = obj as Material;
                    writer.WriteStartElement(name);
                    writer.WriteElementString("shader", mat.shader.name);
                    if (mat.HasProperty("_Color"))
                    {
                        writer.WriteElementString("color", ToString(mat.color));
                    }
                    if (mat.mainTexture != null)
                    {
                        writer.WriteElementString("mainTexture", mat.mainTexture.name);
                        writer.WriteElementString("mainTextureOffset", ToString(mat.mainTextureOffset));
                        writer.WriteElementString("mainTextureScale", ToString(mat.mainTextureScale));
                        assetHelper.ExportTexture(mat.mainTexture as Texture2D);
                    }
                    writer.WriteEndElement();
                    return;
                }
                if (obj is String)
                {
                    writer.WriteElementString(name, obj.ToString());
                    return;
                }
                if (obj is UnityEngine.Object)
                {
                    UnityEngine.Object theObject = (obj as UnityEngine.Object);
                    writer.WriteStartElement(name);
                    if (theObject == null)
                    {
                        writer.WriteAttributeString("Null", ToString(true));
                        writer.WriteEndElement();
                        return;
                    }
                    AddPrefab(theObject);
                    WriteComponent(theObject as Component, true);
                    writer.WriteEndElement();
                    return;
                }
                // Check if we have a Serializable class
                if (obj.GetType().GetCustomAttributes(typeof(SerializableAttribute), true).Length > 0)
                {
                    writer.WriteStartElement(name);
                    FieldInfo[] memInfo = obj.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
                    for (int m = 0; m < memInfo.Length; m++)
                    {
                        if (memInfo[m].GetCustomAttributes(typeof(HideInInspector), true).Length > 0)
                        {
                            continue;
                        }
                        WriteElement(memInfo[m].Name, memInfo[m].GetValue(obj));
                    }
                    writer.WriteEndElement();
                    return;
                }

                writer.WriteElementString(name, obj.ToString());
            }
            catch (Exception ex)
            {
                Debug.Log("Exception when writing " + name + " with value " + obj + ": " + ex.Message);
                throw;
            }
        }

        private string AddPrefab(UnityEngine.Object theObject)
        {
            if (theObject is GameObject)
            {
                Prefabs.Add(theObject as GameObject);
                return theObject.GetInstanceID().ToString();
            }
            else if (theObject is Component)
            {
                Prefabs.Add((theObject as Component).gameObject);
                return theObject.GetInstanceID().ToString();
//                return (theObject as Component).gameObject.GetInstanceID().ToString();
            }
            return theObject.GetType().FullName;
        }

        #region ToString methods
        private string ToString(int[] array)
        {
            StringBuilder sb = new StringBuilder();
            foreach (int item in array)
            {
                sb.Append(item);
                sb.Append(" ");
            }
            return sb.ToString();
        }

        private string ToString(Vector3[] array)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Vector3 item in array)
            {
                sb.Append(ToString(item));
                sb.Append(" ");
            }
            return sb.ToString();
        }

        private string ToString(Vector2 vector2)
        {
            return vector2.x.ToString("0.#####", CultureInfo.InvariantCulture) + " " + vector2.y.ToString("0.#####", CultureInfo.InvariantCulture);
        }

        private string ToString(Vector3 vector3)
        {
            return vector3.x.ToString("0.#####", CultureInfo.InvariantCulture) + " " + vector3.y.ToString("0.#####", CultureInfo.InvariantCulture) + " " + vector3.z.ToString("0.#####", CultureInfo.InvariantCulture);
        }

        private string ToString(Color c)
        {
            return ((int)(c.a * 255)).ToString("X") + ((int)(c.r * 255)).ToString("X") + ((int)(c.g * 255)).ToString("X") + ((int)(c.b * 255)).ToString("X");
        }

        private string ToString(Quaternion quaternion)
        {
            return quaternion.x.ToString("0.#####", CultureInfo.InvariantCulture) + " " + quaternion.y.ToString("0.#####", CultureInfo.InvariantCulture) + " " + quaternion.z.ToString("0.#####", CultureInfo.InvariantCulture) + " " + quaternion.w.ToString("0.#####", CultureInfo.InvariantCulture);
        }

        private string ToString(bool b)
        {
            return b.ToString().ToLower();
        }

        private string ToString(float f)
        {
            return f.ToString("0.#####", CultureInfo.InvariantCulture);
        }
        #endregion

    }
}
