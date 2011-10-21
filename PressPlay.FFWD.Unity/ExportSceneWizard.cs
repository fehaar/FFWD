using System.IO;
using PressPlay.FFWD.Exporter;
using PressPlay.FFWD.Exporter.Writers;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Xml.Serialization;

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
        public AssetHelper assets = new AssetHelper();
        public string exportDir = "";
        public string configSource = @"Editor\FFWD\PressPlay.FFWD.Exporter.dll.config";
        public bool showComponentsNotWritten = true;
        public bool flipYInTransforms = true;
        public List<SceneGroup> groups;
        public List<string> allowedSkippedComponents = new List<string>();
    }

    public string xnaBaseDir = "";
    public int activeGroup = 0;
    public ExportConfig config;

    private List<string> allComponentsNotWritten = new List<string>();

    private XmlSerializer xmlSer = new XmlSerializer(typeof(ExportConfig));
    private TypeResolver resolver;
    private AssetHelper assets;

    public ExportSceneWizard()
    {
        LoadConfiguration();
        
        if (EditorPrefs.HasKey("FFWD configSource"))
        {
            EditorPrefs.DeleteKey("FFWD configSource");
        }
        if (EditorPrefs.HasKey("FFWD scenes"))
        {
            EditorPrefs.DeleteKey("FFWD scenes");
        }
        if (EditorPrefs.HasKey("FFWD textureDir"))
        {
            EditorPrefs.DeleteKey("FFWD textureDir");
        }
        if (EditorPrefs.HasKey("FFWD scriptDir"))
        {
            EditorPrefs.DeleteKey("FFWD scriptDir");
        }
        if (EditorPrefs.HasKey("FFWD meshDir"))
        {
            EditorPrefs.DeleteKey("FFWD meshDir");
        }
        if (EditorPrefs.HasKey("FFWD audioDir"))
        {
            EditorPrefs.DeleteKey("FFWD audioDir");
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
        assets.TextureDir = Path.Combine(xnaBaseDir, config.assets.TextureDir);
        assets.ScriptDir = Path.Combine(xnaBaseDir, config.assets.ScriptDir);
        assets.MeshDir = Path.Combine(xnaBaseDir, config.assets.MeshDir);
        assets.AudioDir = Path.Combine(xnaBaseDir, config.assets.AudioDir);
    }

    private void LoadConfiguration()
    {
        try
        {
            using (StreamReader sr = new StreamReader(Path.Combine(Application.dataPath, @"Editor\FFWD\ExportSceneWizard.config")))
            {
                config = (ExportConfig)xmlSer.Deserialize(sr);
            }
            if (EditorPrefs.HasKey("FFWD XNA dir"))
            {
                xnaBaseDir = EditorPrefs.GetString("FFWD XNA dir");
            }
            if (EditorPrefs.HasKey("FFWD active group"))
            {
                activeGroup = EditorPrefs.GetInt("FFWD active group");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Could not read configuration data. " + ex.Message);
            config = new ExportConfig();
            SaveConfiguration();
        }
    }

    private void SaveConfiguration()
    {
        using (StreamWriter sw = new StreamWriter(Path.Combine(Application.dataPath, @"Editor\FFWD\ExportSceneWizard.config")))
        {
            xmlSer.Serialize(sw, config);
        }
        EditorPrefs.SetString("FFWD XNA dir", xnaBaseDir);
        EditorPrefs.SetInt("FFWD active group", activeGroup);
    }

    [MenuItem("Press Play/FFWD/Export Scenes")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Export Scenes to XNA", typeof(ExportSceneWizard), "Export all groups", "Export active group");
    }

    [MenuItem("CONTEXT/Transform/FFWD Export Resource")]
    static void ExportTransform(MenuCommand command)
    {
        ExportSceneWizard wiz = new ExportSceneWizard();
        wiz.ExportResource(((Transform)command.context).gameObject);
    }

    [MenuItem("CONTEXT/MonoBehaviour/FFWD Export Script")]
    static void ExportScript(MenuCommand command)
    {
        ExportSceneWizard wiz = new ExportSceneWizard();
        wiz.ExportScript(command.context as MonoBehaviour);
    }

    public void OnWizardUpdate()
    {
        SaveConfiguration();
    }

    [MenuItem("Press Play/FFWD/Export Open Scene")]
    static void ExportOpenScene()
    {
        ExportSceneWizard wiz = new ExportSceneWizard();
        wiz.ExportScene();
    }

    private void ExportScene()
    {
        SceneWriter scene = new SceneWriter(resolver, assets);
        scene.ExportDir = Path.Combine(Path.Combine(xnaBaseDir, config.exportDir), "Scenes");
        scene.FlipYInTransforms = config.flipYInTransforms;
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

    public void OnWizardCreate()
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
                    ExportScene();
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

    public void OnWizardOtherButton()
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
                ExportScene();
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

    public void ExportResource(GameObject go)
    {
        if (go == null)
        {
            Debug.LogError("FFWD: Cannot export null resource");
            return;
        }

        SceneWriter scene = new SceneWriter(resolver, assets);
        scene.ExportDir = Path.Combine(xnaBaseDir, config.exportDir);
        scene.FlipYInTransforms = config.flipYInTransforms;
        ScriptTranslator.ScriptNamespace = config.scriptNamespace;

        string path = Path.ChangeExtension(AssetDatabase.GetAssetPath(go).Replace("Assets/", ""), "xml");
        Debug.Log("Start resource export of " + path);
        scene.WriteResource(path, go);
    }

    private void ExportScript(MonoBehaviour monoBehaviour)
    {
        Debug.Log("Start script export of " + monoBehaviour.GetType());
        assets.ExportScript(monoBehaviour.GetType(), false, true);
    }
}

