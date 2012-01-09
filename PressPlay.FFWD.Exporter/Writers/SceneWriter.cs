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

        private XmlWriter writer = null;

        private List<GameObject> Prefabs = new List<GameObject>();
        private List<int> writtenIds = new List<int>();
        public List<string> componentsNotWritten = new List<string>();

        private struct MeshWriterData
        {
            internal Mesh mesh;
            internal bool writeAsStatic;
        }

        private Dictionary<int, MeshWriterData> meshesToWrite = new Dictionary<int, MeshWriterData>();
        private Dictionary<int, AnimationClip> animationClipsToWrite = new Dictionary<int, AnimationClip>();

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
                WriteAssets();
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            WriteAnimations();
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
                Prefabs.Add(go);
                WritePrefabs();
                WriteAssets();
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            WriteAnimations();
        }

        public void WriteResource(string path, Material mat)
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
                writer.WriteAttributeString("Type", resolver.DefaultNamespace + ".Material");
                WriteElement(null, mat);
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

        private void WriteAssets()
        {
            foreach (var item in meshesToWrite.Values)
            {
                writer.WriteStartElement("asset");
                WriteMeshData(item);
                writer.WriteEndElement();
            }
        }

        private void WriteAnimations()
        {
            if (animationClipsToWrite.Count > 0)
            {
                Debug.Log("Write " + animationClipsToWrite.Count + " animations");
            }
            foreach (int key in animationClipsToWrite.Keys)
            {
                string path = PreparePath(String.Format("../Assets/{0}.xml", key));
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "  ";
                //Debug.Log("Write animation " + key, animationClipsToWrite[key]);
                using (writer = XmlWriter.Create(path, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("XnaContent");
                    writer.WriteStartElement("Asset");
                    writer.WriteAttributeString("Type", resolver.DefaultNamespace + ".AnimationClip");
                    WriteElement(null, animationClipsToWrite[key]);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
            }
        }

        private void WriteGameObject(GameObject go)
        {
            writtenIds.Add(go.GetInstanceID());
            writer.WriteElementString("id", go.GetInstanceID().ToString());
            writer.WriteElementString("name", go.name);            
            writer.WriteElementString("layer", ToString(go.layer));
            if (!go.active)
            {
                writer.WriteElementString("active", ToString(go.active));
            }
            writer.WriteElementString("tag", go.tag);
            if (go.isStatic)
            {
                writer.WriteElementString("static", ToString(go.isStatic));
            }
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
                    Debug.LogError("Exception while writing Component of type " + comps[i].GetType() + " : " + ex.Message);
                    throw;
                }
            }
            writer.WriteEndElement();
        }

        private void WriteTransform(Transform transform, bool isPrefab)
        {
            Vector3 pos = transform.localPosition;
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
                if (transform.localScale.x < 0 || transform.localScale.y < 0 || transform.localScale.z < 0)
                {
                    Debug.LogWarning("Transform with negative scale, this could cause trouble in FFWD", transform);
                }

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

            System.Type type = component.GetType();
            if (resolver.SkipComponent(component))
            {
                if (!componentsNotWritten.Contains(type.FullName))
                {
                    componentsNotWritten.Add(type.FullName);
                }
                return false;
            }
            IComponentWriter componentWriter = resolver.GetComponentWriter(type);

            if (componentWriter != null)
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
                try
                {
                    componentWriter.Write(this, component);
                }
                catch (Exception ex)
                {
                    Debug.LogError(String.Format("Exception when writing {0} on {1} under {2} using writer {3} :\n{4}",
                        component.GetType(), component.name, component.transform.root.name, componentWriter.GetType(), ex.Message), component);
                }
                if (!isPrefab)
                {
                    writer.WriteEndElement();
                }
                return true;
            }
            else
            {
                if (type == typeof(MeshCollider))
                {
                    Debug.Log("Unexported Mesh collider", component);
                }
                if (!componentsNotWritten.Contains(type.FullName))
                {
                    componentsNotWritten.Add(type.FullName);
                }
            }
            return false;
        }

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

        internal void WriteMesh(Mesh mesh, string name, bool staticMesh = false)
        {
            int id = mesh.GetInstanceID();
            writer.WriteStartElement(name);
            writer.WriteElementString("id", id.ToString());
            writer.WriteEndElement();

            if (!meshesToWrite.ContainsKey(id))
            {
                meshesToWrite[id] = new MeshWriterData() { mesh = mesh, writeAsStatic = staticMesh };
            }
            else
            {
                if (staticMesh && !meshesToWrite[id].writeAsStatic)
                {
                    MeshWriterData d = meshesToWrite[id];
                    d.writeAsStatic = true;
                    meshesToWrite[id] = d;
                }
            }
            if (!staticMesh)
            {
                assetHelper.ExportMesh(mesh);
            }
        }

        private void WriteMeshData(MeshWriterData data)
        {
            writer.WriteAttributeString("Type", "PressPlay.FFWD.Mesh");
            string asset = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(data.mesh.GetInstanceID()));
            writer.WriteElementString("id", data.mesh.GetInstanceID().ToString());
            writer.WriteElementString("name", data.mesh.name);
            writer.WriteElementString("asset", asset);
            if (data.writeAsStatic || String.IsNullOrEmpty(asset))
            {
                WriteElement("vertices", data.mesh.vertices);
                WriteElement("normals", data.mesh.normals);
                WriteElement("uv", TransformUV(data.mesh.uv));
                writer.WriteStartElement("triangleSets");
                for (int i = 0; i < data.mesh.subMeshCount; i++)
                {
                    WriteElement("Item", data.mesh.GetTriangles(i));
                }
                writer.WriteEndElement();
                if (data.mesh.boneWeights != null)
                {
                    writer.WriteStartElement("boneWeights");
                    for (int i = 0; i < data.mesh.boneWeights.Length; i++)
                    {
                        BoneWeight w = data.mesh.boneWeights[i];
                        writer.WriteElementString("Item", String.Format("{0} {1} {2} {3} {4} {5} {6} {7}", w.weight0, w.weight1, w.weight2, w.weight3, w.boneIndex0, w.boneIndex1, w.boneIndex2, w.boneIndex3));
                    }
                    writer.WriteEndElement();
                }
                if (data.mesh.bindposes != null)
                {
                    writer.WriteStartElement("bindPoses");
                    for (int i = 0; i < data.mesh.bindposes.Length; i++)
                    {
                        writer.WriteString(ToString(data.mesh.bindposes[i]) + " ");
                    }
                    writer.WriteEndElement();
                }
            }
            WriteElement("bounds", data.mesh.bounds);
        }

        private Vector2[] TransformUV(Vector2[] uv)
        {
            Vector2[] result = new Vector2[uv.Length];
            for (int i = 0; i < uv.Length; i++)
            {
                result[i] = new Vector2(uv[i].x, 1 - uv[i].y);
            }
            return result;
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
                    Bounds b = (Bounds)obj;
                    writer.WriteStartElement(name);
                    writer.WriteElementString("c", ToString(b.center));
                    writer.WriteElementString("e", ToString(b.extents));
                    writer.WriteEndElement();
                    return;
                }
                if (obj is String)
                {
                    writer.WriteElementString(name, obj.ToString());
                    return;
                }
                if (obj is float)
                {
                    writer.WriteElementString(name, ToString((float)obj));
                    return;
                }
                if (obj is Double)
                {
                    writer.WriteElementString(name, ToString((Double)obj));
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
                if (obj is Vector2[])
                {
                    writer.WriteElementString(name, ToString(obj as Vector2[]));
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
                if (obj is Matrix4x4)
                {
                    writer.WriteElementString(name, ToString((Matrix4x4)obj));
                    return;
                }
                if (obj is Color)
                {
                    writer.WriteElementString(name, ToString((Color)obj));
                    return;
                }
                if (obj is Color[])
                {
                    writer.WriteElementString(name, ToString(obj as Color[]));
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
                if (obj is AnimationClip)
                {
                    AnimationClip clip = obj as AnimationClip;
                    if (name != null)
                    {
                        writer.WriteStartElement(name);
                    }
                    WriteElement("id", clip.GetInstanceID());
                    WriteElement("length", clip.length);
                    writer.WriteElementString("name", clip.name);
                    WriteElement("wrapMode", clip.wrapMode);
                    WriteElement("curves", AnimationUtility.GetAllCurves(clip));
                    if (name != null)
                    {
                        writer.WriteEndElement();
                    }
                    return;
                }
                if (obj is AnimationClipCurveData)
                {
                    AnimationClipCurveData acd = obj as AnimationClipCurveData;
                    writer.WriteStartElement(name);
                    writer.WriteElementString("path", acd.path);
                    writer.WriteElementString("propertyName", acd.propertyName);
                    if (acd.type != null)
                    {
                        writer.WriteElementString("type", acd.type.FullName);
                    }
                    if (acd.target != null)
                    {
                        WriteElement("target", acd.target.GetInstanceID());
                    }
                    WriteElement("curve", acd.curve);
                    writer.WriteEndElement();
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
                    if (name != null)
                    {
                        writer.WriteStartElement(name);
                    }
                    writer.WriteElementString("id", mat.GetInstanceID().ToString());
                    writer.WriteElementString("name", mat.name);
                    writer.WriteElementString("shader", mat.shader.name);
                    writer.WriteElementString("renderQueue", mat.renderQueue.ToString());

                    if (mat.HasProperty("_Color"))
                    {
                        writer.WriteElementString("color", ToString(mat.color));
                    }
                    else if (mat.HasProperty("_TintColor"))
                    {
                        writer.WriteElementString("color", ToString(mat.GetColor("_TintColor")));
                    }
                    else if ((obj as Material).name == "Default-Particle")
                    {
                        writer.WriteElementString("color", "FFFFFFFF");
                    }
                    
                    if (mat.mainTexture != null)
                    {
                        WriteElement("mainTexture", mat.mainTexture, typeof(Texture2D));
                        writer.WriteElementString("mainTextureOffset", ToString(mat.mainTextureOffset));
                        writer.WriteElementString("mainTextureScale", ToString(mat.mainTextureScale));
                        if (mat.mainTexture.wrapMode == TextureWrapMode.Repeat)
                        {
                            writer.WriteElementString("wrapRepeat", ToString(true));
                        }
                        try
                        {
                            assetHelper.ExportTexture(mat.mainTexture as Texture2D);
                        }
                        catch (UnityException ex)
                        {
                            Debug.Log("Error when exporting texture in Material " + mat.name + ", " + ex.Message, mat);
                            throw;
                        }
                    }
                    if (name != null)
                    {
                        writer.WriteEndElement();
                    }
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
                
                if (obj is Texture2D)
                {
                    writer.WriteStartElement(name);
                    if ((obj as Texture2D) == null)
                    {
                        writer.WriteAttributeString("Null", ToString(true));
                    }
                    else
                    {
                        writer.WriteElementString("id", (obj as Texture2D).GetInstanceID().ToString());
                        if ((obj as Texture2D).name == "Default-Particle")
                        {
                            writer.WriteElementString("name", "Default-Particle");
                        }
                        else
                        {
                            writer.WriteElementString("name", Path.ChangeExtension(assetHelper.GetAssetName(obj as Texture2D), "").TrimEnd('.'));
                            assetHelper.ExportTexture(obj as Texture2D);
                        }
                    }
                    writer.WriteEndElement();
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
                if (obj is AnimationCurve)
                {
                    writer.WriteStartElement(name);
                    writer.WriteAttributeString("Type", resolver.ResolveTypeName(obj));
                    if (obj != null)
                    {
                        Components.AnimationCurveWriter acw = new Components.AnimationCurveWriter();
                        acw.Write(this, obj);
                    }
                    else
                    {
                        writer.WriteAttributeString("Null", ToString(true));
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
                    WriteMembers(obj);
                    writer.WriteEndElement();
                    return;
                }

                writer.WriteElementString(name, obj.ToString());
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception when writing " + name + " with value " + obj + ": " + ex.Message + " (" + ex.GetType() + ")", obj as UnityEngine.Object);
                throw;
            }
        }

        private void WriteMembers(object obj)
        {
            FieldInfo[] memInfo = obj.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            for (int m = 0; m < memInfo.Length; m++)
            {
                if (memInfo[m].GetCustomAttributes(typeof(HideInInspector), true).Length > 0)
                {
                    continue;
                }
                WriteElement(memInfo[m].Name, memInfo[m].GetValue(obj));
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

        internal void AddAnimationClip(AnimationClip animationClip)
        {
            if (!animationClipsToWrite.ContainsKey(animationClip.GetInstanceID()))
            {
                animationClipsToWrite.Add(animationClip.GetInstanceID(), animationClip);
            }
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

        private string ToString(Vector2[] array)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Vector2 item in array)
            {
                sb.Append(ToString(item));
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

        private string ToString(Color[] array)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Color item in array)
            {
                sb.Append(ToString(item));
                sb.Append(" ");
            }
            return sb.ToString();
        }

        private string ToString(Matrix4x4 matrix4x4)
        {
            StringBuilder sb = new StringBuilder(35 * 4);
            for (int i = 0; i < 16; i++)
			{
                sb.Append(matrix4x4[i]);
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

        private string ToString(Double d)
        {
            return d.ToString("0.#####", CultureInfo.InvariantCulture);
        }

        private string ToString(Guid g)
        {
            return g.ToString();
        }
        #endregion

    }
}
