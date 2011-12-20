using System.IO;
using PressPlay.FFWD.Exporter;
using PressPlay.FFWD.Exporter.Writers;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Xml.Serialization;
using System.Text;
using System.Linq;

public class ExportSceneWizard : ScriptableWizard
{
    [Serializable]
    public class SceneGroup
    {
        public string name;
        public List<string> scenes;
    }

    [Serializable]
    public class ExportConfig
    {
        public string scriptNamespace = "";
        public AssetHelper xnaAssets = new AssetHelper();
        public string unityScriptDir = "Scripts";
        public string exportDir = "";
        public string configSource = @"Editor\FFWD\PressPlay.FFWD.Exporter.dll.config";
        public bool showComponentsNotWritten = true;
        public List<SceneGroup> groups;
        public List<string> allowedSkippedComponents = new List<string>();

        public List<string> excludedScripts = new List<string>();
        public List<string> excludedPaths = new List<string>();

        private static XmlSerializer xmlSer = new XmlSerializer(typeof(ExportConfig));
        public static ExportConfig Load()
        {
            ExportConfig config;
            try
            {
                using (StreamReader sr = new StreamReader(Path.Combine(Application.dataPath, @"Editor\FFWD\ExportSceneWizard.config")))
                {
                    config = (ExportConfig)xmlSer.Deserialize(sr);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Could not read configuration data. " + ex.Message);
                config = new ExportConfig();
                config.Save();
            }
            return config;
        }

        public void Save()
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(Application.dataPath, @"Editor\FFWD\ExportSceneWizard.config")))
            {
                xmlSer.Serialize(sw, this);
            }
        }
    }

    public class ExportCommands
    {
        public ExportCommands()
        {
            config = ExportConfig.Load();
            if (EditorPrefs.HasKey("FFWD XNA dir " + PlayerSettings.productName))
            {
                xnaBaseDir = EditorPrefs.GetString("FFWD XNA dir " + PlayerSettings.productName);
            }
            if (EditorPrefs.HasKey("FFWD active group"))
            {
                activeGroup = EditorPrefs.GetInt("FFWD active group");
            }

            string configPath = Path.Combine(Application.dataPath, config.configSource);
            if (File.Exists(configPath))
            {
                resolver = TypeResolver.ReadConfiguration(configPath);
            }
            if (resolver == null)
            {
                Debug.LogWarning("We have no TypeResolver so we will not export any components");
            }

            assets = new AssetHelper();
            assets.TextureDir = Path.Combine(xnaBaseDir, config.xnaAssets.TextureDir);
            assets.ScriptDir = Path.Combine(xnaBaseDir, config.xnaAssets.ScriptDir);
            assets.MeshDir = Path.Combine(xnaBaseDir, config.xnaAssets.MeshDir);
            assets.AudioDir = Path.Combine(xnaBaseDir, config.xnaAssets.AudioDir);
            assets.XmlDir = Path.Combine(xnaBaseDir, config.exportDir);
        }

        public ExportConfig config;
        public string xnaBaseDir = "";
        public int activeGroup = 0;
        private TypeResolver resolver;
        public AssetHelper assets;
        private List<string> allComponentsNotWritten = new List<string>();

        public void ExportResource(GameObject go)
        {
            if (go == null)
            {
                Debug.LogError("FFWD: Cannot export null resource");
                return;
            }

            SceneWriter scene = new SceneWriter(resolver, assets);
            scene.ExportDir = Path.Combine(xnaBaseDir, config.exportDir);
            ScriptTranslator.ScriptNamespace = config.scriptNamespace;

            string path = Path.ChangeExtension(AssetDatabase.GetAssetPath(go).Replace("Assets/", ""), "xml");
            Debug.Log("Start resource export of " + path);
            scene.WriteResource(path, go);
        }

        public void ExportScript(MonoBehaviour monoBehaviour)
        {
            Debug.Log("Start script export of " + monoBehaviour.GetType());
            assets.ExportScript(monoBehaviour.GetType(), false, true);
        }

        public void ExportTextAsset(TextAsset asset)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset.GetInstanceID()).Replace("Assets/", "");
            string exportPath = Path.Combine(assets.XmlDir, assetPath);
            if (!Directory.Exists(Path.GetDirectoryName(exportPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(exportPath));
            }
            File.WriteAllBytes(exportPath, asset.bytes);
            Debug.Log("Exported Text asset to " + exportPath);
        }

        public void ExportAudio(AudioClip asset)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset.GetInstanceID());
            string exportPath = Path.Combine(Path.Combine(assets.AudioDir, ".."), assetPath.Replace("Assets/", ""));
            if (!Directory.Exists(Path.GetDirectoryName(exportPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(exportPath));
            }
            File.Copy(assetPath, exportPath, true);
            Debug.Log("Exported Audio asset to " + exportPath);
        }

        public void ExportTexture(Texture2D asset)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset.GetInstanceID());
            string exportPath = Path.Combine(assets.TextureDir, assetPath.Replace("Assets/", ""));
            if (!Directory.Exists(Path.GetDirectoryName(exportPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(exportPath));
            }
            if (Path.GetExtension(assetPath) == ".png")
            {
                File.Copy(assetPath, exportPath, true);
            }
            else
            {
                if (asset.format == TextureFormat.ARGB32 || asset.format == TextureFormat.RGB24)
                {
                    File.WriteAllBytes(exportPath, asset.EncodeToPNG());
                }
                else
                {
                    Color[] texPixels = asset.GetPixels();
                    Texture2D tex2 = new Texture2D(asset.width, asset.height, TextureFormat.ARGB32, false);
                    tex2.SetPixels(texPixels);
                    File.WriteAllBytes(exportPath, tex2.EncodeToPNG());
                }
            }
            Debug.Log("Exported Texture asset to " + exportPath);
        }

        public void ExportTags()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?><XnaContent><Asset Type=\"System.String[]\">");
            for (int i = 0; i < 31; i++)
            {
                sb.AppendFormat("<Item>{0}</Item>", LayerMask.LayerToName(i));
            }
            sb.Append("</Asset></XnaContent>");
            string exportPath = Path.Combine(assets.XmlDir, "LayerNames.xml");
            File.WriteAllText(exportPath, sb.ToString());
            Debug.Log("Exported all tags to " + exportPath);
        }

        public void ExportOpenScene()
        {
            SceneWriter scene = new SceneWriter(resolver, assets);
            scene.ExportDir = Path.Combine(assets.XmlDir, "Scenes");
            ScriptTranslator.ScriptNamespace = config.scriptNamespace;

            Debug.Log("----------------------- " + Path.GetFileName(EditorApplication.currentScene) + " -------------------------Start scene export");
            scene.Write(Path.ChangeExtension(Path.GetFileName(EditorApplication.currentScene), "xml"));

            if (config.showComponentsNotWritten)
            {
                string skippedComponents = "";
                foreach (string item in scene.componentsNotWritten)
                {
                    if (config.allowedSkippedComponents.Contains(item)) { continue; }

                    if (!allComponentsNotWritten.Contains(item))
                    {
                        allComponentsNotWritten.Add(item);
                    }
                    skippedComponents += item + ", ";
                }
                if (skippedComponents != "")
                {
                    Debug.Log("Skipped component: " + skippedComponents);
                }
            }
            scene.componentsNotWritten.Clear();
            //Debug.Log("---End scene export of " + Path.GetFileName(EditorApplication.currentScene));
        }

        public void ExportAllScenes()
        {
            allComponentsNotWritten.Clear();

            foreach (SceneGroup group in config.groups)
            {
                Debug.Log("************************ START LEVEL EXPORT FOR GROUP " + group.name + " ****************************");

                foreach (string name in group.scenes)
                {
                    string levelName = name;

                    if (!levelName.EndsWith(".unity"))
                    {
                        levelName += ".unity";
                    }

                    if (EditorApplication.OpenScene(levelName))
                    {
                        ExportOpenScene();
                    }
                    else
                    {
                        Debug.Log("Could not open scene " + levelName);
                    }
                }
            }

            string notWritten = "";
            foreach (string item in allComponentsNotWritten)
            {
                notWritten += item + ", ";
            }

            Debug.Log(notWritten);
        }

        public void ExportActiveScenes()
        {
            allComponentsNotWritten.Clear();

            SceneGroup group = null;
            if (activeGroup >= 0 && activeGroup < config.groups.Count)
            {
                group = config.groups[activeGroup];
            }

            if (group == null)
            {
                Debug.LogError("Export failed: Active group " + activeGroup + " does not exist.");
                return;
            }

            Debug.Log("************************ START LEVEL EXPORT FOR GROUP " + group.name + " ****************************");

            foreach (string name in group.scenes)
            {
                string levelName = name;

                if (!levelName.EndsWith(".unity"))
                {
                    levelName += ".unity";
                }

                if (EditorApplication.OpenScene(levelName))
                {
                    ExportOpenScene();
                }
                else
                {
                    Debug.Log("Could not open scene " + levelName);
                }
            }

            string notWritten = "";
            foreach (string item in allComponentsNotWritten)
            {
                notWritten += item + ", ";
            }

            Debug.Log(notWritten);
        }

        private Dictionary<string, string> unityScriptFiles = new Dictionary<string, string>();
        private Dictionary<string, string> xnaScriptFiles = new Dictionary<string, string>();

        private void FindAllUnityScripts()
        {
            foreach (string file in Directory.GetFiles(Path.Combine(Application.dataPath, config.unityScriptDir), "*.cs", SearchOption.AllDirectories))
            {
                unityScriptFiles[Path.GetFileNameWithoutExtension(file)] = file;
            }
        }

        private void FindAllXnaScripts()
        {
            foreach (string file in Directory.GetFiles(Path.Combine(xnaBaseDir, config.xnaAssets.ScriptDir), "*.cs", SearchOption.AllDirectories))
            {
                xnaScriptFiles[Path.GetFileNameWithoutExtension(file)] = file;
            }
        }

        internal void ExportAllScripts()
        {
            FindAllUnityScripts();
            Debug.Log("Found " + unityScriptFiles.Count + " unity scripts");
            FindAllXnaScripts();
            Debug.Log("Found " + xnaScriptFiles.Count + " XNA scripts");

            int converted = 0;
            foreach (var className in unityScriptFiles.Keys)
            {
                if (config.excludedScripts.Contains(className))
                {
                    continue;
                }
                string scriptFile = unityScriptFiles[className];
                if (config.excludedPaths.Any(s => scriptFile.Contains("\\" + s + "\\")))
                {
                    continue;
                }
                TranslateScriptFile(scriptFile);
                converted++;
            }
            Debug.Log("Converted " + converted + " scripts");
            int purged = 0;
            foreach (var className in xnaScriptFiles.Keys)
            {
                if (config.excludedScripts.Contains(className))
                {
                    continue;
                }
                if (!unityScriptFiles.ContainsKey(className))
                {
                    File.Delete(xnaScriptFiles[className]);
                    purged++;
                }
            }
            Debug.Log("Purged " + purged + " script files");
        }

        private void TranslateScriptFile(string scriptFile)
        {
            Debug.Log("Converting script " + Path.GetFileName(scriptFile));
            string scriptText = File.ReadAllText(scriptFile);
            string[] textLines = scriptText.Split('\n');
            PressPlay.FFWD.Exporter.ScriptTranslator trans = new ScriptTranslator(textLines);
            trans.Translate();
            string newText = trans.ToString();
            string newPath = scriptFile.Replace(Path.Combine(Application.dataPath, config.unityScriptDir), Path.Combine(xnaBaseDir, config.xnaAssets.ScriptDir));
            if (!Directory.Exists(Path.GetDirectoryName(newPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(newPath));
            }
            File.WriteAllText(newPath, newText);
        }
    }

    public ExportConfig config;
    public string xnaBaseDir = "";
    public int activeGroup = 0;

    public ExportSceneWizard()
    {
        config = ExportConfig.Load();
        if (EditorPrefs.HasKey("FFWD XNA dir " + PlayerSettings.productName))
        {
            xnaBaseDir = EditorPrefs.GetString("FFWD XNA dir " + PlayerSettings.productName);
        }
        if (EditorPrefs.HasKey("FFWD active group"))
        {
            activeGroup = EditorPrefs.GetInt("FFWD active group");
        }
    }

    public void OnWizardCreate()
    {
        config.Save();
        EditorPrefs.SetString("FFWD XNA dir " + PlayerSettings.productName, xnaBaseDir);
        EditorPrefs.SetInt("FFWD active group", activeGroup);
    }

    public void OnWizardOtherButton()
    {
    }

    [MenuItem("Press Play/FFWD/Configuration")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Edit FFWD Configuration", typeof(ExportSceneWizard), "Save", "Cancel");
    }

    [MenuItem("Press Play/FFWD/Export settings")]
    static void ExportRenderSettings()
    {
        ExportCommands cmd = new ExportCommands();
        cmd.ExportTags();        
    }

    [MenuItem("Press Play/FFWD/Export all resources")]
    static void ExportAllResources()
    {
        ExportCommands cmd = new ExportCommands();
        UnityEngine.Object[] os = Resources.LoadAll("");
        foreach (var item in os)
        {
            try
            {
                if (item is GameObject)
                {
                    cmd.ExportResource(item as GameObject);
                }
                if (item is TextAsset)
                {
                    cmd.ExportTextAsset(item as TextAsset);
                }
                if (item is Texture2D)
                {
                    cmd.ExportTexture(item as Texture2D);
                }
                if (item is AudioClip)
                {
                    cmd.ExportAudio(item as AudioClip);
                }
            }
            catch (Exception ex)
            {
                Debug.Log("Could not export " + item.name + ". " + ex.Message);
            }
        }
        Resources.UnloadUnusedAssets();
    }

    [MenuItem("Press Play/FFWD/Export open scene")]
    static void ExportOpenScene()
    {
        ExportCommands cmd = new ExportCommands();
        cmd.ExportOpenScene();
    }

    [MenuItem("Press Play/FFWD/Export all scenes")]
    static void ExportAllScenes()
    {
        ExportCommands cmd = new ExportCommands();
        cmd.ExportAllScenes();
    }

    [MenuItem("Press Play/FFWD/Export active scene group")]
    static void ExportActiveScenes()
    {
        ExportCommands cmd = new ExportCommands();
        cmd.ExportActiveScenes();
    }

    [MenuItem("Press Play/FFWD/Export all scripts")]
    static void ExportAllScripts()
    {
        ExportCommands cmd = new ExportCommands();
        cmd.ExportAllScripts();
    }

    [MenuItem("Press Play/FFWD/Full export")]
    static void FullExport()
    {
        ExportAllScenes();
        ExportAllScripts();
        ExportAllResources();
        ExportTags(null);
    }

    [MenuItem("CONTEXT/Transform/FFWD export resource")]
    static void ExportTransform(MenuCommand command)
    {
        ExportCommands cmd = new ExportCommands();
        cmd.ExportResource(((Transform)command.context).gameObject);
    }

    [MenuItem("CONTEXT/MonoBehaviour/FFWD export script")]
    static void ExportScript(MenuCommand command)
    {
        ExportCommands cmd = new ExportCommands();
        cmd.ExportScript(command.context as MonoBehaviour);
    }

    [MenuItem("CONTEXT/TextAsset/FFWD export resource")]
    static void ExportTextAsset(MenuCommand command)
    {
        ExportCommands cmd = new ExportCommands();
        TextAsset asset = command.context as TextAsset;
        cmd.ExportTextAsset(asset);
    }

    [MenuItem("CONTEXT/TagManager/FFWD export tags")]
    static void ExportTags(MenuCommand command)
    {
        ExportCommands cmd = new ExportCommands();
        cmd.ExportTags();
    }
}

