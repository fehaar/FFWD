using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD
{
    public class Material
    {
        public string shader { get; set; }
        [ContentSerializer(Optional = true)]
        public Color color { get; set; }
        public string mainTexture { get; set; }
        public Vector2 mainTextureOffset { get; set; }
        public Vector2 mainTextureScale { get; set; }

        public void SetColor(string name, Color color)
        {
            this.color = color;
        }
    }
}
