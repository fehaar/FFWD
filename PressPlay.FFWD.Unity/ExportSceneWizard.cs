using UnityEngine;
using UnityEditor;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using PressPlay.U2X;
using PressPlay.U2X.Writers;

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

    public void OnWizardCreate()  
    {
        resolver = TypeResolver.ReadConfiguration(configSource);
        if (resolver == null)
        {
            Debug.LogWarning("We have no TypeResolver so we will not export any components");
        }

        SceneWriter scene = new SceneWriter(resolver);
        scene.ExportDir = exportDir;
        scene.TextureDir = textureDir;

        scene.Write(Path.Combine(exportDir, Path.ChangeExtension(Path.GetFileName(EditorApplication.currentScene), "xml")));
    }
}

