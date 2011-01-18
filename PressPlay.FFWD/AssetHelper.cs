using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD
{
    public class AssetHelper
    {
        private Dictionary<string, ContentManager> contentManagers = new Dictionary<string, ContentManager>();
        private Dictionary<string, object> content = new Dictionary<string, object>();

        public Func<ContentManager> CreateContentManager;
        public static List<string> staticAssets = new List<string>();

        public T Load<T>(string contentPath)
        {
            return Load<T>(Application.loadedLevelName, contentPath);
        }

        public T Load<T>(string category, string contentPath)
        {
            if (!content.ContainsKey(contentPath))
            {
                if (staticAssets.Contains(contentPath))
                {
                    category = "Static";
                }
                ContentManager manager = GetContentManager(category);
                try
                {
                    content.Add(contentPath, manager.Load<T>(contentPath));
                }
                catch
                {
                    Debug.Log("Asset not found. " + typeof(T).Name + " at " + contentPath);
                    return default(T);
                }
            }
            return (T)content[contentPath];
        }

        private ContentManager GetContentManager(string category)
        {
            if (!contentManagers.ContainsKey(category))
            {
                contentManagers.Add(category, CreateContentManager());
            }
            return contentManagers[category];
        }
    }
}
