using System.IO;
using PressPlay.FFWD.Exporter;
using PressPlay.FFWD.Exporter.Writers;
using UnityEditor;
using UnityEngine;

public class ExportSceneWizard : ScriptableWizard {
    public ExportSceneWizard()
    {
        resolver = TypeResolver.ReadConfiguration(configSource);
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
        ScriptableWizard.DisplayWizard("Export Scene to XNA", typeof(ExportSceneWizard), "Execute");
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
    public string configSource = @"C:\Projects\PressPlay\Tentacles\Unity\Assets\Editor\FFWD\PressPlay.FFWD.Exporter.dll.config";
    public bool flipYInTransforms = true;

    private TypeResolver resolver;
    private AssetHelper assets;

    public void OnWizardCreate()  
    {
        SceneWriter scene = new SceneWriter(resolver, assets);
        scene.ExportDir = Path.Combine(Path.Combine(xnaDir, exportDir), "Scenes");
        scene.FlipYInTransforms = flipYInTransforms;
        ScriptTranslator.ScriptNamespace = scriptNamespace;

        Debug.Log("Start scene export of " + Path.GetFileName(EditorApplication.currentScene));
        scene.Write(Path.Combine(exportDir, Path.ChangeExtension(Path.GetFileName(EditorApplication.currentScene), "xml")));
        Debug.Log("End scene export of " + Path.GetFileName(EditorApplication.currentScene));
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

