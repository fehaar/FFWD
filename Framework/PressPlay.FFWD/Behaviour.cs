using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD
{
    public class Behaviour : Component
    {
        [ContentSerializer(Optional = true, ElementName = "enabled")]
        private bool _enabled = true;
        [ContentSerializerIgnore]
        public bool enabled 
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
                Application.UpdateComponentActive(this);
            }
        }

        internal bool hasBeenEnabled = false;
    }
}
