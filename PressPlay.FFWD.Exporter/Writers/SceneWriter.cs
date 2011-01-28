using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using PressPlay.FFWD.Exporter.Interfaces;
using UnityEditor;
using UnityEngine;

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
        public List<string> componentsNotWritten = new List<string>();

        public void Write(string path)
        {
            path = PreparePath(path);

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

        public void WriteResource(string path, GameObject go)
        {
            path = PreparePath(path);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            using (writer = XmlWriter.Create(path, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("XnaContent");
                writer.WriteStartElement("Asset");
                writer.WriteAttributeString("Type", resolver.DefaultNamespace + ".Scene");
                writer.WriteStartElement("go");
                WriteGameObject(go);
                writer.WriteEndElement();
                WritePrefabs();
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        private string PreparePath(string path)
        {
            path = Path.Combine(ExportDir, path);
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                Debug.Log("Created directory " + Path.GetDirectoryName(path));
            }
            return path;
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
                writer.WriteStartElement("go");
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
                writer.WriteStartElement("p");
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
            writer.WriteStartElement("cs");
            Component[] comps = go.GetComponents(typeof(Component));
            for (int i = 0; i < comps.Length; i++)
            {
                try
                {
                    WriteComponent(comps[i], false);
                }
                catch (Exception ex)
                {
                    Debug.Log("Exception while writing Component of type " + comps[i].GetType());
                    throw;
                }
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
                writer.WriteStartElement("c");
                writer.WriteAttributeString("Type", "PressPlay.FFWD.Transform");
            }
            writer.WriteElementString("id", transform.GetInstanceID().ToString());
            if (isPrefab)
            {
                writer.WriteElementString("isPrefab", ToString(true));
            }
            if (!isPrefab && transform.childCount > 0)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    writer.WriteStartElement("go");
                    WriteGameObject(transform.GetChild(i).gameObject);
                    writer.WriteEndElement();
                }
            }
            if (pos != Vector3.zero)
            {
                writer.WriteElementString("p", ToString(pos));
            }
            if (transform.localScale != Vector3.one)
            {
                writer.WriteElementString("s", ToString(transform.localScale));
            }
            if (transform.localRotation != Quaternion.identity)
            {
                writer.WriteElementString("r", ToString(transform.localRotation));
            }
            if (!isPrefab)
            {
                writer.WriteEndElement();
            }
        }

        private bool WriteComponent(Component component, bool isPrefab)
        {
            if (resolver == null)
            {
                return false;
            }
            if (component == null)
            {
                return false;
            }
            if (component is Transform)
            {
                WriteTransform(component as Transform, isPrefab);
                return true;
            }
            if (resolver.SkipComponent(component))
            {
                return false;
            }

            System.Type type = component.GetType();
            IComponentWriter componentWriter = resolver.GetComponentWriter(type);
            if (componentWriter != null)
            {
                try
                {
                    writtenIds.Add(component.GetInstanceID());
                    if (!isPrefab)
                    {
                        writer.WriteStartElement("c");
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
                    return true;
                }
                catch
                {
                    Debug.Log("Exception when writing " + component.GetType() + " on " + component.name + " under " + component.transform.root.name, component);
                }
            }
            else
            {
                if (!componentsNotWritten.Contains(type.FullName))
                {
                    componentsNotWritten.Add(type.FullName);
                }
            }
            return false;
        }

        //internal void WriteTexture(Texture texture)
        //{
        //    writer.WriteElementString("texture", texture.name);
        //    assetHelper.ExportTexture(texture as Texture2D);
        //}

        internal void WriteScript(MonoBehaviour component, bool overwrite)
        {
            // TODO: Find Interfaces as well
            Type tp = component.GetType();
            while (tp != typeof(MonoBehaviour))
            {
                assetHelper.ExportScript(tp, false, overwrite);
                tp = tp.BaseType;
            }
        }

        internal void WriteScriptStub(MonoBehaviour component)
        {
            Type tp = component.GetType();
            while (tp != typeof(MonoBehaviour))
            {
                assetHelper.ExportScript(tp, true, false);
                tp = tp.BaseType;
            }
        }

        internal void WriteMesh(Mesh mesh, string name)
        {
            string asset = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(mesh.GetInstanceID()));
            writer.WriteStartElement(name);
            writer.WriteElementString("id", mesh.GetInstanceID().ToString());
            writer.WriteElementString("name", mesh.name);
            writer.WriteElementString("asset", asset);
            writer.WriteEndElement();
            assetHelper.ExportMesh(mesh);
        }

        internal void WriteElement(string name, object obj)
        {
            WriteElement(name, obj, typeof(MonoBehaviour));
        }

        internal void WriteElement(string name, object obj, Type elementType)
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
                if (obj is Bounds)
                {
                    // These components are always skipped for some reason. There must be some logic to it?
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
                if (obj is bool[])
                {
                    writer.WriteElementString(name, ToString(obj as bool[]));
                    return;
                }
                if (obj is int)
                {
                    writer.WriteElementString(name, obj.ToString());
                    return;
                }
                if (obj is int[])
                {
                    if (!obj.GetType().GetElementType().IsEnum)
                    {
                        writer.WriteElementString(name, ToString(obj as int[]));
                        return;
                    }
                }
                if (obj is Guid)
                {
                    writer.WriteElementString(name, ToString((Guid)obj));
                    return;
                }
                if (obj is Vector2)
                {
                    writer.WriteElementString(name, ToString((Vector2)obj));
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
                if (obj is Color)
                {
                    writer.WriteElementString(name, ToString((Color)obj));
                    return;
                }
                if (obj is Rect)
                {
                    writer.WriteElementString(name, ToString((Rect)obj));
                    return;
                }
                if (obj is LayerMask)
                {
                    writer.WriteElementString(name, ((LayerMask)obj).value.ToString());
                    return;
                }
                if (obj is IDictionary)
                {
                    writer.WriteStartElement(name);
                    foreach (DictionaryEntry item in (obj as IDictionary))
                    {
                        writer.WriteStartElement("Item");
                        WriteElement("Key", item.Key);
                        WriteElement("Value", item.Value);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
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
                    if (mat == null)
                    {
                        return;
                    }
                    writer.WriteStartElement(name);
                    writer.WriteElementString("id", mat.GetInstanceID().ToString());
                    writer.WriteElementString("name", mat.name);
                    writer.WriteElementString("shader", mat.shader.name);
                    writer.WriteElementString("renderQueue", mat.renderQueue.ToString());
                    if (mat.HasProperty("_Color"))
                    {
                        writer.WriteElementString("color", ToString(mat.color));
                    }
                    else
                    {
                        if (mat.HasProperty("_TintColor"))
                        {
                            writer.WriteElementString("color", ToString(mat.GetColor("_TintColor")));
                        }
                    }
                    if (mat.mainTexture != null)
                    {
                        writer.WriteElementString("mainTexture", mat.mainTexture.name);
                        writer.WriteElementString("mainTextureOffset", ToString(mat.mainTextureOffset));
                        writer.WriteElementString("mainTextureScale", ToString(mat.mainTextureScale));
                        try
                        {
                            assetHelper.ExportTexture(mat.mainTexture as Texture2D);
                        }
                        catch (UnityException ex)
                        {
                            Debug.Log("Error when exporting texture in Material " + mat.name, mat);
                            throw;
                        }
                    }
                    writer.WriteEndElement();
                    return;
                }
                if (obj is AudioClip)
                {
                    AudioClip audio = obj as AudioClip;
                    if (audio == null)
                    {
                        return;
                    }
                    writer.WriteElementString(name, audio.name);
                    assetHelper.ExportAudio(audio);
                    return;
                }
                if (obj is String)
                {
                    writer.WriteElementString(name, obj.ToString());
                    return;
                }
                if (obj is GameObject)
                {
                    GameObject go = (obj as GameObject);
                    writer.WriteStartElement(name);
                    if (obj == null || (go.GetInstanceID() == 0))
                    {
                        writer.WriteAttributeString("Null", ToString(true));
                    }
                    else
                    {
                        writer.WriteElementString("id", go.GetInstanceID().ToString());
                        writer.WriteElementString("isPrefab", ToString(true));
                        AddPrefab(go);
                    }
                    writer.WriteEndElement();
                    return;
                }
                if (obj is Component)
                {
                    Component theObject = (obj as Component);
                    writer.WriteStartElement(name);

                    if ((theObject != null) && (obj.GetType() != elementType))
                    {   
                        writer.WriteAttributeString("Type", resolver.ResolveTypeName(obj));
                    }
                   
                    if (theObject == null || !WriteComponent(theObject, true))
                    {
                        writer.WriteAttributeString("Null", ToString(true));
                        writer.WriteEndElement();
                        return;
                    }

                    AddPrefab(theObject);
                    writer.WriteEndElement();
                    return;
                }
                // Check if we have a Serializable array
                if (obj.GetType().IsArray)
                {
                    writer.WriteStartElement(name);
                    foreach (object mat in (Array)obj)
                    {
                        WriteElement("Item", mat);
                    }
                    writer.WriteEndElement();
                    return;
                }
                // Check if we have a List
                if (obj is IList)
                {
                    writer.WriteStartElement(name);
                    foreach (object item in (IList)obj)
                    {
                        WriteElement("Item", item);
                    }
                    writer.WriteEndElement();
                    return;
                }
                // Check if we have a Serializable class
                if (obj.GetType().GetCustomAttributes(typeof(SerializableAttribute), true).Length > 0 || (obj.GetType().IsValueType && !obj.GetType().IsEnum))
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
                Prefabs.Add((theObject as GameObject).transform.root.gameObject);
                return theObject.GetInstanceID().ToString();
            }
            else if (theObject is Component)
            {
                Prefabs.Add((theObject as Component).gameObject.transform.root.gameObject);
                return theObject.GetInstanceID().ToString();
            }
            return theObject.GetType().FullName;
        }

        #region ToString methods
        private string ToString(bool[] array)
        {
            StringBuilder sb = new StringBuilder();
            foreach (bool item in array)
            {
                sb.Append(ToString(item));
                sb.Append(" ");
            }
            return sb.ToString();
        }

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

        private string ToString(Rect rect)
        {
            return rect.x.ToString("0.#####", CultureInfo.InvariantCulture) + " " + rect.y.ToString("0.#####", CultureInfo.InvariantCulture) + " " + rect.width.ToString("0.#####", CultureInfo.InvariantCulture) + " " + rect.height.ToString("0.#####", CultureInfo.InvariantCulture);
        }

        private string ToString(Color c)
        {
            return ((int)(c.a * 255)).ToString("X2") + ((int)(c.r * 255)).ToString("X2") + ((int)(c.g * 255)).ToString("X2") + ((int)(c.b * 255)).ToString("X2");
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

        private string ToString(Guid g)
        {
            return g.ToString();
        }
        #endregion

    }
}
