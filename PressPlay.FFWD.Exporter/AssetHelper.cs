using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

namespace PressPlay.FFWD.Exporter
{
    public static class AssetHelper
    {
        public static string TextureDir { get; set; }
        public static string ScriptDir { get; set; }
        
        private static HashSet<string> exportedTextures = new HashSet<string>();
        private static HashSet<string> exportedScripts = new HashSet<string>();
        private static Dictionary<string, string> _scripts;
        private static Dictionary<string, string> scripts  
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

        public static void ExportTexture(Texture2D tex)
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
            }
        }

        public static void ExportScript(MonoBehaviour component)
        {
            if (component == null) return;
            if (exportedScripts.Contains(component.name)) return;
            try
            {
                string key = component.GetType().Name;
                if (scripts.ContainsKey(key))
                {
                    ScriptTranslator translator = new ScriptTranslator(File.ReadAllLines(scripts[key]));
                    translator.Translate();
                    File.WriteAllText(Path.Combine(ScriptDir, key + ".cs"), translator.ToString());
                }
                exportedScripts.Add(component.name);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

    }
}
