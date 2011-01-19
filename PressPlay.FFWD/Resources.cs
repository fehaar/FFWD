using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PressPlay.FFWD
{
    public static class Resources
    {
        private static Dictionary<string, GameObject> cachedResources = new Dictionary<string, GameObject>();

        internal static AssetHelper AssetHelper;

        public static UnityObject Load(string name)
        {
            //TODO: remove this when loaded resources are handled correctly
            //if (cachedResources.ContainsKey(name)) 
            //{
            //    return cachedResources[name];
            //}

            Application.loadingScene = true;
            GameObject go = AssetHelper.Load<GameObject>(Path.Combine("Resources", name));
            go.AfterLoad(null);
            go.SetNewId(null);
            Application.loadingScene = false;

            //TODO: remove this when loaded resources are handled correctly
            //GameObject.DontDestroyOnLoad(go);
            //cachedResources.Add(name,go);

            return go;
        }
    }
}
