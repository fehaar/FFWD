using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Components
{
    internal class LightmapSettings
    {
        public static LightmapData[] lightmaps;
        public static LightmapsMode lightmapsMode;

        public enum LightmapsMode { Single, Dual, Directional }

        [ContentSerializer(FlattenContent = true, CollectionItemName = "lightmapData")]
        public LightmapData[] lightmapData;
        [ContentSerializer(ElementName = "lightmapsMode")]
        public LightmapsMode lightmapMode;
    }
}
