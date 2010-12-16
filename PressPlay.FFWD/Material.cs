using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD
{
    public class Material
    {
        public Color color;

        public void SetColor(string name, Color color)
        {
            this.color = color;
        }
    }
}
