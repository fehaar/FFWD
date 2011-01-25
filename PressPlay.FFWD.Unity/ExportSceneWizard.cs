using System.IO;
using PressPlay.FFWD.Exporter;
using PressPlay.FFWD.Exporter.Writers;
using UnityEditor;
using UnityEngine;

public class ExportSceneWizard : ScriptableWizard {
    public ExportSceneWizard()
    {
        if (EditorPrefs.HasKey("FFWD configSource"))
        {
            configSource = EditorPrefs.GetString("FFWD configSource");
        }
        if (EditorPrefs.HasKey("FFWD XNA dir"))
        {
            xnaDir = EditorPrefs.GetString("FFWD XNA dir");
        }
        if (EditorPrefs.HasKey("FFWD scenes"))
        {
            scenes = EditorPrefs.GetString("FFWD scenes").Split(';');
        }

        string configPath = Path.Combine(Application.dataPath, configSource);

        if (File.Exists(configPath))
        {
            resolver = TypeResolver.ReadConfiguration(configPath);
        }

        if (resolver == null)
        {
            Debug.LogWarning("We have no TypeResolver so we will not export any components");
        }

        assets = new AssetHelper();
        assets.TextureDir = Path.Combine(xnaDir, textureDir);
        assets.ScriptDir = Path.Combine(xnaDir, scriptDir);
        assets.MeshDir = Path.Combine(xnaDir, meshDir);
        assets.AudioDir = Path.Combine(xnaDir, audioDir);        
    }
    

    [MenuItem("Press Play/FFWD/Export Scene")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Export Scene to XNA", typeof(ExportSceneWizard), "Open scene", "Selected scenes");
    }

    [MenuItem("CONTEXT/Transform/FFWD Export Resource")]
    static void ExportTransform(MenuCommand command)
    {
        ExportSceneWizard wiz = new ExportSceneWizard();
        wiz.ExportResource(((Transform)command.context).gameObject);
    }

    public string scriptNamespace = @"PressPlay.Tentacles.Scripts";
    public string xnaDir = @"C:\Projects\PressPlay\Tentacles\XNA";
    public string exportDir = @"PressPlay.Tentacles.XmlContent";
    public string scriptDir = @"PressPlay.Tentacles.Scripts";
    public string textureDir = @"PressPlay.Tentacles.Win\PressPlay.Tentacles.WinContent\Textures";
    public string audioDir = @"PressPlay.Tentacles.Win\PressPlay.Tentacles.WinContent\Sounds";
    public string meshDir = @"PressPlay.Tentacles.Win\PressPlay.Tentacles.WinContent\Models";
    public string configSource = @"Editor\FFWD\PressPlay.FFWD.Exporter.dll.config";
    public bool showComponentsNotWritten = true;
    public bool flipYInTransforms = true;

    public string[] scenes = { 
        @"Assets\Levels (Scenes)\Build Levels\Green_intro.unity",
        @"Assets\Levels (Scenes)\Build Levels\petridish_tutorial.unity",
        @"Assets\Levels (Scenes)\Build Levels\Green_currentsAndTiming.unity",
        @"Assets\Levels (Scenes)\Build Levels\INTESTINES_FirstMovingSpikesEasy.unity",
        @"Assets\Levels (Scenes)\Build Levels\VEIN_SuperEasy.unity",
        @"Assets\Levels (Scenes)\Build Levels\Green_VeryEasy_OnlySpikes.unity",
        @"Assets\Levels (Scenes)\Base Scenes\Preloader.unity"
    };


    private TypeResolver resolver;
    private AssetHelper assets;

    public void OnWizardUpdate()
    {
        EditorPrefs.SetString("FFWD XNA dir", xnaDir);
        EditorPrefs.SetString("FFWD configSource", configSource);
        EditorPrefs.SetString("FFWD scenes", string.Join(";", scenes));
    }

    public void OnWizardCreate()  
    {
        SceneWriter scene = new SceneWriter(resolver, assets);
        scene.ExportDir = Path.Combine(Path.Combine(xnaDir, exportDir), "Scenes");
        scene.FlipYInTransforms = flipYInTransforms;
        ScriptTranslator.ScriptNamespace = scriptNamespace;

        Debug.Log("Start scene export of " + Path.GetFileName(EditorApplication.currentScene));
        scene.Write(Path.ChangeExtension(Path.GetFileName(EditorApplication.currentScene), "xml"));

        if (showComponentsNotWritten)
        {
            foreach (string item in scene.componentsNotWritten)
            {
                Debug.Log("Skipped component: " + item);
            }
        }
        scene.componentsNotWritten.Clear();
        Debug.Log("End scene export of " + Path.GetFileName(EditorApplication.currentScene));
    }

    public void OnWizardOtherButton()
    {
        foreach (string name in scenes)
        {
            if (EditorApplication.OpenScene(name))
            {
                OnWizardCreate();
            }
            else
            {
                Debug.Log("Could not open scene " + name);
            }
        }
    }

    public void ExportResource(GameObject go)
    {
        if (go == null)
        {
            Debug.LogError("FFWD: Cannot export null resource");
            return;
        }

        SceneWriter scene = new SceneWriter(resolver, assets);
        scene.ExportDir = Path.Combine(xnaDir, exportDir);
        scene.FlipYInTransforms = flipYInTransforms;
        ScriptTranslator.ScriptNamespace = scriptNamespace;

        string path = Path.ChangeExtension(AssetDatabase.GetAssetPath(go).Replace("Assets/", ""), "xml");
        Debug.Log("Start resource export of " + path);
        scene.WriteResource(path, go);
        Debug.Log("End resource export of " + path);
    }
}

