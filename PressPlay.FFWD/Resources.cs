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

        private static Func<string, UnityObject>[] loaders = new Func<string, UnityObject>[] { LoadScene, LoadTexture, LoadText };

        public static UnityObject Load(string name)
        {
            UnityObject result = null;
            int loaderIndex = 0;
            while (result == null && loaderIndex < loaders.Length)
	        {
                result = loaders[loaderIndex++](name);
	        }
            return result;
        }

        /// <summary>
        /// This should actually return UnityObjects, but since Texture2D is not a UO at the moment, it cannot do it.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Load(string name, Type type)
        {
            try
            {
                if (type == typeof(Texture2D))
                {
                    Microsoft.Xna.Framework.Graphics.Texture2D t = AssetHelper.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Resources", Path.Combine("Resources", name));
                    return new Texture2D(t);
                }
                if (type == typeof(Song))
                {
                    return AssetHelper.Load<Song>("Resources", Path.Combine("Resources", name));
                }
                if (type == typeof(AudioClip))
                {
                    SoundEffect sfx = AssetHelper.Load<SoundEffect>("Resources", Path.Combine("Resources", name));
                    return new AudioClip(sfx);
                }
                throw new Exception("Unsupported file type.");
            }
            catch (Exception ex)
            {
                Debug.LogError("Cannot load resource " + name + " as Texture2D. " + ex.Message);
            }
            return null;
        }

        private static UnityObject LoadScene(string name)
        {
            Application.loadingScene = true;
            Scene scene = AssetHelper.Load<Scene>("Resources", Path.Combine("Resources", name));
            if (scene == null)
            {
                Application.loadingScene = false;
                return null;
            }
            // This is removed here. It is called in scene.Initialize just below.
            scene.Initialize();
            Application.loadingScene = false;
            Application.newAssets.AddRange(scene.assets);
            Application.LoadNewAssets();

            if (scene.prefabs.Count > 0)
            {
                return scene.prefabs[0];
            }
            return null;
        }

        private static UnityObject LoadText(string name)
        {
            return AssetHelper.Load<TextAsset>("Resources", Path.Combine("Resources", name));
        }

        private static UnityObject LoadTexture(string name)
        {
            Microsoft.Xna.Framework.Graphics.Texture2D t = AssetHelper.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Resources", Path.Combine("Resources", name));
            if (t == null)
            {
                return null;
            }
            return new Texture2D(t);
        }
    }
}
