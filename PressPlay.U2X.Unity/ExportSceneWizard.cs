using UnityEngine;
using UnityEditor;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using PressPlay.U2X;

public class ExportSceneWizard : ScriptableWizard {
    
    [MenuItem("Press Play/U2X/Export Scene")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Export Scene to XNA", typeof(ExportSceneWizard), "Execute");
    }

    public string exportDir = @"C:\Projects\PressPlay\Tentacles\XNA\PressPlay.Tentacles.XmlContent\Scenes";
    public string textureDir = @"C:\Projects\PressPlay\Tentacles\XNA\PressPlay.Tentacles.Win\PressPlay.Tentacles.WinContent\Textures";
    public string configSource = @"C:\Projects\PressPlay\Tentacles\Unity\Assets\Editor\U2X\PressPlay.U2X.dll.config";

    private TypeResolver resolver;
    private List<string> exportedTextures = new List<string>();

    public void OnWizardCreate()  
    {
        resolver = TypeResolver.ReadConfiguration(configSource);
        if (resolver == null)
        {
            Debug.LogWarning("We have no TypeResolver so we will not export any components");
        }

        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.IndentChars = "  ";
        using (XmlWriter writer = XmlWriter.Create(Path.Combine(exportDir, Path.ChangeExtension(Path.GetFileName(EditorApplication.currentScene), "xml")), settings))
        {
            WriteScene(writer);
        }
    }

    private void WriteScene(XmlWriter writer)
    {
        writer.WriteStartDocument();
        writer.WriteStartElement("XnaContent");
        writer.WriteStartElement("Asset");
        writer.WriteAttributeString("Type", resolver.DefaultNamespace + ".Scene");
        WriteGOs(writer);
        writer.WriteEndElement();
        writer.WriteEndElement();
    }

    private void WriteGOs(XmlWriter writer)
    {
        UnityEngine.Object[] objs = FindObjectsOfType(typeof(GameObject));

        for (int i = 0; i < objs.Length; i++)
        {
            GameObject go = objs[i] as GameObject;
            if (go.transform.parent != null)
            {
                continue;
            }            
            writer.WriteStartElement("gameObject");
            writer.WriteAttributeString("ID", "#go" + go.GetInstanceID());
            WriteGameObject(writer, go);
            writer.WriteEndElement();
        }
    }

    private void WriteGameObject(XmlWriter writer, GameObject go)
    {
        writer.WriteElementString("id", go.GetInstanceID().ToString());
        writer.WriteElementString("name", go.name);
        writer.WriteStartElement("transform");
        WriteTransform(writer, go.transform);
        writer.WriteEndElement();
        Object prefab = EditorUtility.GetPrefabParent(go);
        if (prefab != null)
        {
            writer.WriteElementString("prefab", prefab.name);            
        }
        else
        {
            writer.WriteStartElement("prefab");
            writer.WriteAttributeString("Null", ToString(true));
            writer.WriteEndElement();
        }
        writer.WriteStartElement("components");
        Component[] comps = go.GetComponents(typeof(Component));
        for (int i = 0; i < comps.Length; i++)
        {
            WriteComponent(writer, comps[i]);
        }
        writer.WriteEndElement();
    }

    private void WriteTransform(XmlWriter writer, Transform transform)
    {
        writer.WriteElementString("localPosition", ToString(transform.localPosition));
        writer.WriteElementString("localScale", ToString(transform.localScale));
        writer.WriteElementString("localRotation", ToString(transform.localRotation));
        if (transform.childCount > 0)
        {
            writer.WriteStartElement("children");
            for (int i = 0; i < transform.childCount; i++)
            {
                writer.WriteStartElement("child");
                WriteGameObject(writer, transform.GetChild(i).gameObject);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
        
    }

    private void WriteComponent(XmlWriter writer, Component component)
    {
        if (resolver == null)
        {
            return;
        }
        if (resolver.SkipComponent(component))
        {
            return;
        }

        writer.WriteStartElement("component");
        writer.WriteAttributeString("Type", resolver.ResolveTypeName(component));

        System.Type type = component.GetType();
        if (type == typeof(MeshRenderer))
        {
            MeshRenderer mr = component as MeshRenderer;
            if (mr.sharedMaterials.Length > 0 && mr.sharedMaterials[0].mainTexture != null)
            {
                writer.WriteElementString("Texture", mr.sharedMaterials[0].mainTexture.name);
                ExportTexture(mr.sharedMaterials[0].mainTexture as Texture2D);
            }
            MeshFilter filter = component.GetComponent<MeshFilter>();
            if (filter.sharedMesh != null)
            {
                writer.WriteElementString("Mesh", filter.sharedMesh.name);
            }
        }

        writer.WriteEndElement();
    }

    void ExportTexture(Texture2D tex)
    {
        if (tex == null) return;
        if (exportedTextures.Contains(tex.name)) return;

        string path = Path.Combine(textureDir, tex.name + ".png");
        try
        {
            Color[] texPixels = tex.GetPixels();
            Texture2D tex2 = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
            tex2.SetPixels(texPixels);
            byte[] texBytes = tex2.EncodeToPNG();
            FileStream writeStream;
            writeStream = new FileStream(path, FileMode.Create);
            BinaryWriter writeBinay = new BinaryWriter(writeStream);
            for (int i = 0; i < texBytes.Length; i++) writeBinay.Write(texBytes[i]);
            writeBinay.Close();
            exportedTextures.Add(tex.name);
        }
        catch (UnityException ue)
        {
            Debug.Log(ue.ToString());
        }
    }

    #region ToString methods
    private string ToString(Vector3 vector3)
    {
        return vector3.x.ToString("0.#####") + " " + vector3.y.ToString("0.#####") + " " + vector3.z.ToString("0.#####");
    }

    private string ToString(Quaternion quaternion)
    {
        return quaternion.x.ToString("0.#####") + " " + quaternion.y.ToString("0.#####") + " " + quaternion.z.ToString("0.#####") + " " + quaternion.w.ToString("0.#####");
    }

    private string ToString(bool b)
    {
        return b.ToString().ToLower();
    }
    #endregion
}

