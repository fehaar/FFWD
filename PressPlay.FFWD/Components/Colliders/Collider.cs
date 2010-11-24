using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD.Components
{
    public class Collider : Component
    {
        #region ContentProperties
        public bool isTrigger { get; set; }
        public string material { get; set; }
        #endregion
    }
}
