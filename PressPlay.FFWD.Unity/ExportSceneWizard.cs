using System.IO;
using PressPlay.FFWD.Exporter;
using PressPlay.FFWD.Exporter.Writers;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class ExportSceneWizard : ScriptableWizard
{

    private List<string> allComponentsNotWritten = new List<string>();
    private List<string> allowedSkippedComponents = new List<string>(new string[]{"PPMetrics", "PPCommunicator", "CheatManager", "UIManager", "DamageVisualizer", "SnapToAxis", "SnapToYAxis",
    	"PoolableTrailRenderer", "UnityEngine.MeshCollider", "LoadPreloaderFirst", "DrawIcon", "DoNotRenderAtRuntime", "MetricDataHandler", "XNAEllipsoidParticleEmitter", 
		"UnityEngine.ConfigurableJoint", "LemmySquishedTester", "GameObjectPreloader", "SpriteText", "ReturnPoolableObject", "GameObjectPreloaderSet", 
		"SnapLocalPosition", "InputRayPlaneHandler"});

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
        if (EditorPrefs.HasKey("FFWD textureDir"))
        {
            textureDir = EditorPrefs.GetString("FFWD textureDir");
        }
        if (EditorPrefs.HasKey("FFWD scriptDir"))
        {
            scriptDir = EditorPrefs.GetString("FFWD scriptDir");
        }
        if (EditorPrefs.HasKey("FFWD meshDir"))
        {
            meshDir = EditorPrefs.GetString("FFWD meshDir");
        }
        if (EditorPrefs.HasKey("FFWD audioDir"))
        {
            audioDir = EditorPrefs.GetString("FFWD audioDir");
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
        @"Assets\Levels (Scenes)\Build Levels\petridish_tutorial",

        @"Assets\Levels (Scenes)\Build Levels\Green_intro",
        @"Assets\Levels (Scenes)\Build Levels\Green_VeryEasy_OnlySpikes",
        @"Assets\Levels (Scenes)\Build Levels\Green_currentsAndTiming",
        @"Assets\Levels (Scenes)\Build Levels\VEIN_SuperEasy",

        @"Assets\Levels (Scenes)\Build Levels\INTESTINES_FirstMovingSpikesEasy",
        @"Assets\Levels (Scenes)\Build Levels\Intestines_circleblocks",
        @"Assets\Levels (Scenes)\Build Levels\Intestine_MeatHackerDmdChallMiniBossWorm",
        @"Assets\Levels (Scenes)\Build Levels\Veins_EasyRotatingChallenge ",

        @"Assets\Levels (Scenes)\Build Levels\Green_AcidPenetratorPickupBonus",
        @"Assets\Levels (Scenes)\Build Levels\Green_CrumblingBlocks",
        @"Assets\Levels (Scenes)\Build Levels\Green_LotsOfCurrents",
        @"Assets\Levels (Scenes)\Build Levels\Veins_ShooterOmniSpikePickup",

        @"Assets\Levels (Scenes)\Build Levels\Brain_easyBrainMiniBossGobbler",
        @"Assets\Levels (Scenes)\Build Levels\Brain_Vortex_LeapOfFaith_",
        @"Assets\Levels (Scenes)\Build Levels\Brain_SpinningMovingBlocks",
        @"Assets\Levels (Scenes)\Build Levels\Veins_WeedsOmniShooter",

        @"Assets\Levels (Scenes)\Build Levels\Green_RollerGobblerTimeVortex",
        @"Assets\Levels (Scenes)\Build Levels\Green_AcidChallenge+LeapOfFaith",
        @"Assets\Levels (Scenes)\Build Levels\Green_AcidChallCurrentMixArenaMiniBossShooter",
        @"Assets\Levels (Scenes)\Build Levels\Veins_MinesPickupChallenge",

        @"Assets\Levels (Scenes)\Build Levels\Intestines_PoisonPenetrator",
        @"Assets\Levels (Scenes)\Build Levels\INTESTINES_GobblersAcidMovingIntesties",
        @"Assets\Levels (Scenes)\Build Levels\Intestine_SmileyMovingWalkersDNA",
        @"Assets\Levels (Scenes)\Build Levels\Veins_DestructoBlocksTeethTripping",

        @"Assets\Levels (Scenes)\Build Levels\Brain_ElectricPenetrator",
        @"Assets\Levels (Scenes)\Build Levels\Brain_SpeedChallenge",
        @"Assets\Levels (Scenes)\Build Levels\Brain_CurrentsSidewalking_BossWorm",
        @"Assets\Levels (Scenes)\Build Levels\Veins_SwingingDmgChallMovingBlocks",

        @"Assets\Levels (Scenes)\Build Levels\Green_AcidOpenAreaAsteroids",
        @"Assets\Levels (Scenes)\Build Levels\Green_MovingCrumblingWeedDandelions",
        @"Assets\Levels (Scenes)\Build Levels\Green_islandSpeedChallenge",
        @"Assets\Levels (Scenes)\Build Levels\Veins_weedsDmgChallMovingBlocksTeeth",

        @"Assets\Levels (Scenes)\Build Levels\Intestine_RollerCurrentAcidMixArenaGobBoss",
        @"Assets\Levels (Scenes)\Build Levels\Intestine_OmniWalkersBlockChallArena",
        @"Assets\Levels (Scenes)\Build Levels\intestine_RollercoasterSpikeWalkers",
        @"Assets\Levels (Scenes)\Build Levels\Veins_RollersCrumblingDmgChallArena",

        @"Assets\Levels (Scenes)\Build Levels\Brain_ElectricTimedBlocksHard",
        @"Assets\Levels (Scenes)\Build Levels\Brain_ElectricCurrentsHard",
        @"Assets\Levels (Scenes)\Build Levels\Brain_Final_boss",
    
        @"Assets\Levels (Scenes)\Base Scenes\Preloader.unity"
    };


    private TypeResolver resolver;
    private AssetHelper assets;

    public void OnWizardUpdate()
    {
        EditorPrefs.SetString("FFWD XNA dir", xnaDir);
        EditorPrefs.SetString("FFWD configSource", configSource);
        EditorPrefs.SetString("FFWD scenes", string.Join(";", scenes));

        EditorPrefs.SetString("FFWD textureDir", textureDir);
        EditorPrefs.SetString("FFWD scriptDir", scriptDir);
        EditorPrefs.SetString("FFWD meshDir", meshDir);
        EditorPrefs.SetString("FFWD audioDir", audioDir);
    }

    public void OnWizardCreate()
    {
        SceneWriter scene = new SceneWriter(resolver, assets);
        scene.ExportDir = Path.Combine(Path.Combine(xnaDir, exportDir), "Scenes");
        scene.FlipYInTransforms = flipYInTransforms;
        ScriptTranslator.ScriptNamespace = scriptNamespace;

        Debug.Log("----------------------- " + Path.GetFileName(EditorApplication.currentScene) + " -------------------------Start scene export");
        scene.Write(Path.ChangeExtension(Path.GetFileName(EditorApplication.currentScene), "xml"));

        if (showComponentsNotWritten)
        {
            string skippedComponents = "";
            foreach (string item in scene.componentsNotWritten)
            {
                if (allowedSkippedComponents.Contains(item)) { continue; }

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

    public void OnWizardOtherButton()
    {
        allComponentsNotWritten.Clear();

        Debug.Log("******************************* START LEVEL EXPORT ***************************************");

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
        scene.ExportDir = Path.Combine(xnaDir, exportDir);
        scene.FlipYInTransforms = flipYInTransforms;
        ScriptTranslator.ScriptNamespace = scriptNamespace;

        string path = Path.ChangeExtension(AssetDatabase.GetAssetPath(go).Replace("Assets/", ""), "xml");
        Debug.Log("Start resource export of " + path);
        scene.WriteResource(path, go);
    }
}

