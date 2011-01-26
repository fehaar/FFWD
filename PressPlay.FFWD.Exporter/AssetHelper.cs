using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
using UnityEditor;

namespace PressPlay.FFWD.Exporter
{
    public class AssetHelper
    {
        public string TextureDir { get; set; }
        public string AudioDir { get; set; }
        public string MeshDir { get; set; }
        public string ScriptDir { get; set; }

        //private HashSet<string> exportedTextures = new HashSet<string>();
        //private HashSet<string> exportedScripts = new HashSet<string>();
        //private HashSet<string> exportedMeshes = new HashSet<string>();
        private List<string> exportedTextures = new List<string>();
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

        public void ExportTexture(Texture2D tex)
        {
            if (tex == null) return;
            if (exportedTextures.Contains(tex.name)) return;

            string path = Path.Combine(TextureDir, tex.name + ".png");
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
                throw;
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
                Debug.Log(key + " export exception: " + ex.Message);
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
