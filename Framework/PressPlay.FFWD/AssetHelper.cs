using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace PressPlay.FFWD
{
    public class AssetHelper
    {
        private Dictionary<string, ContentManager> contentManagers = new Dictionary<string, ContentManager>();

        public Func<ContentManager> CreateContentManager;
        private static List<string> staticAssets = new List<string>();
        internal bool LoadingResources;

        public T Load<T>(string contentPath)
        {
            return Load<T>((LoadingResources) ? "Resources" : Application.loadedLevelName, contentPath);
        }

        public T Load<T>(string category, string contentPath)
        {
            if (staticAssets.Contains(contentPath) || staticAssets.Contains(category))
            {
                category = "Static";
            }
            ContentManager manager = GetContentManager(category);
            T asset = default(T);
            try
            {
#if DEBUG
                if (DebugSettings.LogAssetLoads)
                {
                    Debug.Log(String.Format("Loading asset {0} from content manager {1}.", contentPath, category));                    
                }
#endif              
                asset = manager.Load<T>(contentPath);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.Log("Asset not found. " + typeof(T).Name + " at " + contentPath + ". " + ex.Message);
#endif
            }
            return asset;
        }

        internal T LoadAsset<T>(string name)
        {
            return Load<T>("Assets", name);
        }

        public void Unload(string category)
        {
            if (contentManagers.ContainsKey(category))
            {
                ContentManager manager = GetContentManager(category);
                contentManagers.Remove(category);
                manager.Unload();
            }
        }

        private ContentManager GetContentManager(string category)
        {
            if (!contentManagers.ContainsKey(category))
            {
                contentManagers.Add(category, CreateContentManager());
            }
            return contentManagers[category];
        }

        public void AddStaticAsset(string name)
        {            
            staticAssets.Add(name);
        }

        public T Preload<T>(string name)
        {
            staticAssets.Add(name);
            return Load<T>(name);
        }
    }
}
