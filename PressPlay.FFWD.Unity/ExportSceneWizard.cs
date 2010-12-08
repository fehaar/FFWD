using UnityEngine;
using UnityEditor;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using PressPlay.FFWD.Exporter;
using PressPlay.FFWD.Exporter.Writers;

public class ExportSceneWizard : ScriptableWizard {
    
    [MenuItem("Press Play/FFWD/Export Scene")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Export Scene to XNA", typeof(ExportSceneWizard), "Execute");
    }

    public string scriptNamespace = @"PressPlay.Tentacles.Scripts";
    public string exportDir = @"C:\Projects\PressPlay\Tentacles\XNA\PressPlay.Tentacles.XmlContent\Scenes";
    public string scriptDir = @"C:\Projects\PressPlay\Tentacles\XNA\PressPlay.Tentacles.Scripts";
    public string textureDir = @"C:\Projects\PressPlay\Tentacles\XNA\PressPlay.Tentacles.Win\PressPlay.Tentacles.WinContent\Textures";
    public string meshDir = @"C:\Projects\PressPlay\Tentacles\XNA\PressPlay.Tentacles.Win\PressPlay.Tentacles.WinContent\Models";
    public string configSource = @"C:\Projects\PressPlay\Tentacles\Unity\Assets\Editor\FFWD\PressPlay.FFWD.Exporter.dll.config";
    public bool flipYInTransforms = true;

    private TypeResolver resolver;

    public void OnWizardCreate()  
    {
        resolver = TypeResolver.ReadConfiguration(configSource);
        if (resolver == null)
        {
            Debug.LogWarning("We have no TypeResolver so we will not export any components");
        }

        AssetHelper assets = new AssetHelper();
        assets.TextureDir = textureDir;
        assets.ScriptDir = scriptDir;
        assets.MeshDir = meshDir;

        SceneWriter scene = new SceneWriter(resolver, assets);
        scene.ExportDir = exportDir;
        scene.FlipYInTransforms = flipYInTransforms;
        ScriptTranslator.ScriptNamespace = scriptNamespace;

        Debug.Log("Start scene export of " + Path.GetFileName(EditorApplication.currentScene));
        scene.Write(Path.Combine(exportDir, Path.ChangeExtension(Path.GetFileName(EditorApplication.currentScene), "xml")));
        Debug.Log("End scene export of " + Path.GetFileName(EditorApplication.currentScene));
    }
}

