using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Components
{
    internal class LightmapData
    {
        public Texture2D lightmapFar;
        [ContentSerializer(Optional = true)]
        public Texture2D lightmapNear;
    }
}
