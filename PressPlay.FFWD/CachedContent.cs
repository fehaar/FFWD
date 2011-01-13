using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD
{
    public class CachedContent
    {
        public ContentManager content;
        private Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        public CachedContent(ContentManager content)
        {
            this.content = content;
        }

        public Texture2D LoadInstantTexture(string source)
        {
            LoadTexture(source);
            return GetTexture(source);
        }

        public void LoadTexture(string source)
        {
            if (String.IsNullOrEmpty(source) || textures.ContainsKey(source))
            {
                return;
            }

            textures.Add(source, content.Load<Texture2D>(source));
        }

        public Texture2D GetTexture(string source)
        {
            if (String.IsNullOrEmpty(source))
            {
                return null;
            }

            Texture2D texture;
            textures.TryGetValue(source, out texture);
            return texture;
        }
    }
}
