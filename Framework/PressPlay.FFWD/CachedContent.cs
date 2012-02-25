using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace PressPlay.FFWD
{
    public class CachedContent
    {
        public ContentManager content;
        private Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        public static Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();

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
            if (String.IsNullOrEmpty(source))
            {
                return;
            }            
            
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

        public SoundEffect LoadInstantSound(string source)
        {
            LoadSound(source);
            return GetSound(source);
        }

        public void LoadSound(string source)
        {
            if (String.IsNullOrEmpty(source))
            {
                return;
            }

            if (String.IsNullOrEmpty(source) || sounds.ContainsKey(source))
            {
                return;
            }

            sounds.Add(source, content.Load<SoundEffect>(source));
        }

        public SoundEffect GetSound(string source)
        {
            if (String.IsNullOrEmpty(source))
            {
                return null;
            }

            SoundEffect sound;
            sounds.TryGetValue(source, out sound);
            return sound;
        }
    }
}
