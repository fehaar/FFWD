using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public struct LayerMask
    {
        public int value { get; set; }

        private static List<string> layerNames;

        public static implicit operator int(LayerMask mask)
        {
            return mask.value;
        }

        public static implicit operator LayerMask(int mask)
        {
            return new LayerMask() { value = mask };
        }

        public static int NameToLayer(string name)
        {
            return layerNames.IndexOf(name);
        }

        public static string LayerToName(int layer)
        {
            if (layer > 0 && layer < layerNames.Count)
            {
                return layerNames[layer];
            }
            return String.Empty;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        internal static void LoadLayerNames(AssetHelper helper)
        {
            if (layerNames == null)
            {
                helper.AddStaticAsset("LayerNames");
                layerNames = new List<string>();
                string[] names = helper.Load<String[]>("LayerNames");
                if (names != null)
                {
                    layerNames.AddRange(names);
                }
            }
        }
    }
}
