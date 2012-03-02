using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace PressPlay.FFWD
{
    public static class Resources
    {
        private static Dictionary<string, GameObject> cachedResources = new Dictionary<string, GameObject>();

        internal static AssetHelper AssetHelper;

        public static UnityObject Load(string name)
        {
            // NOTE: If refactoring this please be careful with the loadingScene flag!
            Application.loadingScene = true;
            object o = AssetHelper.Load<object>("Resources", Path.Combine("Resources", name));
            if (o is Scene)
            {
                UnityObject uo = LoadScene(o as Scene);
                Application.loadingScene = false;
                return uo;
            }
            Application.loadingScene = false;
            if (o == null)
            {
                return null;
            }
            if (o is UnityObject)
            {
                return o as UnityObject;
            }
            if (o is Microsoft.Xna.Framework.Graphics.Texture2D)
            {
                Texture2D Tex = new Texture2D(o as Microsoft.Xna.Framework.Graphics.Texture2D);
                int iIndex = name.LastIndexOf('/');
                iIndex = Mathf.Clamp(iIndex, iIndex, name.Length);
                Tex.name = name.Substring(iIndex + 1); 
                return Tex;
            }
            if (o is SoundEffect)
            {
                return new AudioClip(o as SoundEffect);
            }
            return null;
        }

        /// <summary>
        /// This should actually return UnityObjects, but since Texture2D is not a UO at the moment, it cannot do it.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Load(string name, Type type)
        {
            if (type.IsSubclassOf(typeof(UnityObject)))
            {
                return Load(name);
            }
            if (type == typeof(Song))
            {
                return AssetHelper.Load<Song>("Resources", Path.Combine("Resources", name));
            }
            throw new Exception("Unsupported file type " + type.Name + " when loading resource with explicit name");
        }

        private static UnityObject LoadScene(Scene scene)
        {
            // This is removed here. It is called in scene.Initialize just below.
            scene.Initialize(false);
            Application.LoadNewAssets(true);
            if (scene.prefabs.Count > 0)
            {
                UnityObject.DontDestroyOnLoad(scene.prefabs[0]);
                return scene.prefabs[0];
            }
            return null;
        }

        public static T[] FindObjectsOfTypeAll<T>() where T : UnityObject
        {
            return Application.FindObjectsOfType<T>();
        }

        public static UnityObject[] FindObjectsOfTypeAll(Type type)
        {
            return Application.FindObjectsOfType(type);
        }
    }
}
