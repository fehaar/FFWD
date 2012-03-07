using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
using UnityEditor;

namespace PressPlay.FFWD.Exporter
{
    [Serializable]
    public class AssetHelper
    {
        public string TextureDir;
        public string AudioDir; 
        public string MeshDir; 
        public string ScriptDir;
        public string XmlDir;

        private List<string> exportedTextures = new List<string>( new string[] { "Default-Particle" });
        private List<string> exportedAudio = new List<string>();
        private List<string> exportedScripts = new List<string>();
        private List<string> exportedMeshes = new List<string>();

        private Dictionary<string, string> _scripts;
        private Dictionary<string, string> scripts  
        {
            get
            {
                if (_scripts == null)
	            {
                    _scripts = new Dictionary<string, string>();
                    foreach (string file in Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories))
                    {
                        _scripts[Path.GetFileNameWithoutExtension(file)] = file;
                    }
	            }
                return _scripts;
            }
        }

        public string GetAssetName(UnityEngine.Object asset)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset.GetInstanceID());
            return assetPath.Remove(0, assetPath.IndexOf('/') + 1);
        }

        public void ExportTexture(Texture2D asset)
        {
            if (asset == null) return;

            string assetPath = GetAssetName(asset);
            if (exportedTextures.Contains(assetPath) || exportedTextures.Contains(asset.name)) return;
            exportedTextures.Add(assetPath);

            string path = Path.Combine(TextureDir, Path.ChangeExtension(assetPath, "png"));
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            try
            {
                if (Path.GetExtension(assetPath) == ".png")
                {
                    File.Copy(Path.Combine(Application.dataPath, assetPath), path, true);
                }
                else
                {
                    if (asset.format == TextureFormat.ARGB32 || asset.format == TextureFormat.RGB24)
                    {
                        File.WriteAllBytes(path, asset.EncodeToPNG());
                    }
                    else
                    {
                        Color[] texPixels = asset.GetPixels();
                        if (Path.GetExtension(assetPath) == ".exr")
                        {
                            // Post-process lightmap data. We do it like this for WP7 as we have to use the DualTexture effect.
                            for (int i = 0; i < texPixels.Length; i++)
                            {
                                Color color = texPixels[i];
                                color.r = (8.0f * color.a) * color.r;
                                color.g = (8.0f * color.a) * color.g;
                                color.b = (8.0f * color.a) * color.b;
                                texPixels[i] = color;
                            }
                        }
                        Texture2D tex2 = new Texture2D(asset.width, asset.height, TextureFormat.ARGB32, false);
                        tex2.SetPixels(texPixels);
                        File.WriteAllBytes(path, tex2.EncodeToPNG());
                    }
                }
            }
            catch (UnityException ue)
            {
                Debug.LogError(ue.ToString(), asset);
            }
        }

        public void ExportScript(Type tp, bool stubOnly, bool overwrite)
        {
            string key = tp.Name;
            if (exportedScripts.Contains(key)) return;
            try
            {
                string scriptPath = Path.Combine(ScriptDir, key + ".cs");
                if (!overwrite && File.Exists(scriptPath))
                {
                    return;
                }
                if (scripts.ContainsKey(key))
                {
                    ScriptTranslator translator = new ScriptTranslator(File.ReadAllLines(scripts[key]));
                    if (stubOnly)
                    {
                        translator.CreateStub();
                    }
                    else
                    {
                        translator.Translate();
                    }
                    File.WriteAllText(scriptPath, translator.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(key + " export exception: " + ex.Message);
            }
            finally
            {
                exportedScripts.Add(key);
            }
        }

        public void ExportMesh(Mesh mesh)
        {
            if (mesh == null) return;
            string path = AssetDatabase.GetAssetPath(mesh.GetInstanceID());
            if (String.IsNullOrEmpty(path)) return;
            if (exportedMeshes.Contains(path)) return;

            exportedMeshes.Add(path);
            //path = Path.Combine(@"C:\Projects\PressPlay\Tentacles\Unity\Assets\Level Building Blocks\Worlds\_worlds_imports\XNA", Path.GetFileName(path));
            string dest = Path.Combine(MeshDir, Path.GetFileName(path));

            try
            {
                File.Copy(path, dest, true);
            }
            catch (Exception ex)
            {
                Debug.Log("Could not copy mesh '" + path + "' for " + mesh.name + ". " + ex.Message, mesh);
                throw;
            }
        }

        internal void ExportAudio(AudioClip audio)
        {
            if (audio == null) return;
            string path = AssetDatabase.GetAssetPath(audio.GetInstanceID());
            if (exportedAudio.Contains(path)) return;

            exportedAudio.Add(path);
            string dest = Path.Combine(AudioDir, Path.GetFileName(path));

            try
            {
                File.Copy(path, dest, true);
            }
            catch (UnityException ue)
            {
                Debug.Log(ue.ToString());
                throw;
            }

        }
    }
}
