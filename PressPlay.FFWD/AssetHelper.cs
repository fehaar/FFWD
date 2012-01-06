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
        private Dictionary<string, List<string>> managerContent = new Dictionary<string, List<string>>();
        private Dictionary<string, object> content = new Dictionary<string, object>();

        public Func<ContentManager> CreateContentManager;
        private static List<string> staticAssets = new List<string>();
        internal bool LoadingResources;

        public T Load<T>(string contentPath)
        {
            return Load<T>((LoadingResources) ? "Resources" : Application.loadedLevelName, contentPath);
        }

        public T Load<T>(string category, string contentPath)
        {
            if (!content.ContainsKey(contentPath))
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
                    Debug.Log("loading asset from disk : " + contentPath);
#endif
                    asset = manager.Load<T>(contentPath);
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.Log("Asset not found. " + typeof(T).Name + " at " + contentPath + ". " + ex.Message);
#endif
                }
                if (asset == null && category == "Resources")
                {
                    return default(T);
                }
                content.Add(contentPath, asset);
                managerContent[category].Add(contentPath);
            }
            else
            {
                if (!(content[contentPath] is T))
                {
                    return default(T);
                }
            }
            return (T)content[contentPath];
        }

        internal T LoadAsset<T>(int id)
        {
            return Load<T>("Assets", Path.Combine("Assets", id.ToString()));
        }

        public void Unload(string category)
        {
            if (contentManagers.ContainsKey(category))
            {
                ContentManager manager = GetContentManager(category);
                contentManagers.Remove(category);
                List<string> contentInManager = managerContent[category];
                for (int i = 0; i < contentInManager.Count; i++)
                {
                    content.Remove(contentInManager[i]);
                }
                managerContent.Remove(category);
                manager.Unload();
            }
        }

        private ContentManager GetContentManager(string category)
        {
            if (!contentManagers.ContainsKey(category))
            {
                contentManagers.Add(category, CreateContentManager());
                managerContent.Add(category, new List<string>());
            }
            return contentManagers[category];
        }

        public void AddStaticAsset(string name)
        {
            
            staticAssets.Add(name);
            
            //Load<T>(name);
        }

        public void Preload<T>(string name)
        {
            staticAssets.Add(name);
            //Load<T>(name);
        }

        public T PreloadInstant<T>(string name)
        {
            staticAssets.Add(name);
            return Load<T>(name);
        }
    }
}
