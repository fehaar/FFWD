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
        @"Assets\Levels (Scenes)\Build Levels\petridish_tutorial.unity",

        @"Assets\Levels (Scenes)\Build Levels\Green_intro.unity",
        @"Assets\Levels (Scenes)\Build Levels\Green_VeryEasy_OnlySpikes.unity",
        @"Assets\Levels (Scenes)\Build Levels\Green_currentsAndTiming.unity",
        @"Assets\Levels (Scenes)\Build Levels\VEIN_SuperEasy.unity",

        @"Assets\Levels (Scenes)\Build Levels\INTESTINES_FirstMovingSpikesEasy.unity",
        @"Assets\Levels (Scenes)\Build Levels\Intestines_circleblocks.unity",
        @"Assets\Levels (Scenes)\Build Levels\Intestine_MeatHackerDmdChallMiniBossWorm.unity",
        @"Assets\Levels (Scenes)\Build Levels\Veins_EasyRotatingChallenge .unity",

        @"Assets\Levels (Scenes)\Build Levels\Green_AcidPenetratorPickupBonus.unity",
        @"Assets\Levels (Scenes)\Build Levels\Green_CrumblingBlocks.unity",
        @"Assets\Levels (Scenes)\Build Levels\Green_LotsOfCurrents.unity",
        @"Assets\Levels (Scenes)\Build Levels\Veins_ShooterOmniSpikePickup.unity",

        @"Assets\Levels (Scenes)\Build Levels\Brain_easyBrainMiniBossGobbler.unity",
        @"Assets\Levels (Scenes)\Build Levels\Brain_Vortex_LeapOfFaith_.unity",
        @"Assets\Levels (Scenes)\Build Levels\Brain_SpinningMovingBlocks.unity",
        @"Assets\Levels (Scenes)\Build Levels\Veins_WeedsOmniShooter.unity",

        @"Assets\Levels (Scenes)\Build Levels\Green_RollerGobblerTimeVortex.unity",
        @"Assets\Levels (Scenes)\Build Levels\Green_AcidChallenge+LeapOfFaith.unity",
        @"Assets\Levels (Scenes)\Build Levels\Green_AcidChallCurrentMixArenaMiniBossShooter.unity",
        @"Assets\Levels (Scenes)\Build Levels\Veins_MinesPickupChallenge.unity",

        @"Assets\Levels (Scenes)\Build Levels\Intestines_PoisonPenetrator.unity",
        @"Assets\Levels (Scenes)\Build Levels\INTESTINES_GobblersAcidMovingIntesties.unity",
        @"Assets\Levels (Scenes)\Build Levels\Intestine_SmileyMovingWalkersDNA.unity",
        @"Assets\Levels (Scenes)\Build Levels\Veins_DestructoBlocksTeethTripping.unity",

        @"Assets\Levels (Scenes)\Build Levels\Brain_ElectricPenetrator.unity",
        @"Assets\Levels (Scenes)\Build Levels\Brain_SpeedChallenge.unity",
        @"Assets\Levels (Scenes)\Build Levels\Brain_CurrentsSidewalking_BossWorm.unity",
        @"Assets\Levels (Scenes)\Build Levels\Veins_SwingingDmgChallMovingBlocks.unity",

        @"Assets\Levels (Scenes)\Build Levels\Green_AcidOpenAreaAsteroids.unity",
        @"Assets\Levels (Scenes)\Build Levels\Green_MovingCrumblingWeedDandelions.unity",
        @"Assets\Levels (Scenes)\Build Levels\Green_islandSpeedChallenge.unity",
        @"Assets\Levels (Scenes)\Build Levels\Veins_weedsDmgChallMovingBlocksTeeth.unity",

        @"Assets\Levels (Scenes)\Build Levels\Intestine_RollerCurrentAcidMixArenaGobBoss.unity",
        @"Assets\Levels (Scenes)\Build Levels\Intestine_OmniWalkersBlockChallArena.unity",
        @"Assets\Levels (Scenes)\Build Levels\intestine_RollercoasterSpikeWalkers.unity",
        @"Assets\Levels (Scenes)\Build Levels\Veins_RollersCrumblingDmgChallArena.unity",

        @"Assets\Levels (Scenes)\Build Levels\Brain_ElectricTimedBlocksHard.unity",
        @"Assets\Levels (Scenes)\Build Levels\Brain_ElectricCurrentsHard.unity",
        @"Assets\Levels (Scenes)\Build Levels\Brain_Final_boss.unity",
    
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

