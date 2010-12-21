using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD
{
    public class Material
    {
        [ContentSerializer]
        private string shader { get; set; }
        [ContentSerializer(Optional=true)]
        public Color color { get; set; }
        [ContentSerializer(Optional = true)]
        public string mainTexture { get; set; }
        [ContentSerializer(Optional = true)]
        public Vector2 mainTextureOffset { get; set; }
        [ContentSerializer(Optional = true)]
        public Vector2 mainTextureScale { get; set; }

        [ContentSerializerIgnore]
        public Texture2D texture;

        public bool IsAdditive()
        {
            return shader == "iPhone/Particles/Additive Culled";
        }
    }
}
