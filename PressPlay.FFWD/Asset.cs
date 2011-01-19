using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD
{
    public abstract class Asset : UnityObject
    {
        public Asset()
            : base()
        {
            Application.AddNewAsset(this);
        }

        public string name { get; set; }

        internal abstract void LoadAsset(AssetHelper assetHelper);
    }
}
