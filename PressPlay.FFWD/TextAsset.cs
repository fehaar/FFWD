using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public class TextAsset : Asset
    {
        public string text { get; private set; }

        protected override void DoLoadAsset(AssetHelper assetHelper)
        {
            throw new NotImplementedException();
        }
    }
}
