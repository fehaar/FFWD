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

        [ContentSerializer(Optional = true)]
        protected string asset;
        [ContentSerializer(Optional = true)]
        public string name { get; set; }

        private bool _isLoaded = false;

        internal void LoadAsset(AssetHelper assetHelper)
        {
            if (!_isLoaded)
            {
                DoLoadAsset(assetHelper);
                _isLoaded = true;
            }
        }

        protected abstract void DoLoadAsset(AssetHelper assetHelper);

        public override string ToString()
        {
            return String.Format("{0} {2} ({1})", GetType().Name, GetInstanceID(), name);
        }

        internal override void SetNewId(Dictionary<int, UnityObject> idMap)
        {
            throw new InvalidOperationException("This should not happen");
        }
    }
}
