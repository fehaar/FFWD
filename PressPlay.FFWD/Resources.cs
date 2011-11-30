using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD
{
    public static class Resources
    {
        private static Dictionary<string, GameObject> cachedResources = new Dictionary<string, GameObject>();

        internal static AssetHelper AssetHelper;

        public static UnityObject Load(string name)
        {
            Application.loadingScene = true;
            Scene scene = AssetHelper.Load<Scene>(Path.Combine("Resources", name));

            // This is removed here. It is called in scene.Initialize just below.
            //scene.AfterLoad(null); 
            scene.Initialize();
            Application.loadingScene = false;
            Application.LoadNewAssets();

            if (scene.gameObjects.Count > 0)
            {
                return scene.gameObjects[0];
            } 
            else if (scene.prefabs.Count > 0)
            {
                return scene.prefabs[0];
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
            if (type == typeof(Texture2D))
            {
                try
                {
                    return AssetHelper.Load<Texture2D>(Path.Combine("Resources", name));
                }
                catch (Exception ex)
                {
                    Debug.LogError("Cannot load resource " + name + " as Texture2D. " + ex.Message);
                }
            }
            return null;
        }
    }
}
