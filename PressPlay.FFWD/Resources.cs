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

        public static object Load(string name, Type type)
        {
            throw new NotImplementedException();
        }
    }
}
