using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.U2X.Xna.Components
{
    public class MeshCollider : Component
    {
        #region ContentProperties
        public string Material { get; set; }
        public bool IsTrigger { get; set; }
        public string Mesh { get; set; }
        #endregion
    }
}
