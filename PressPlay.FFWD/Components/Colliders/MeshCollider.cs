using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common;

namespace PressPlay.FFWD.Components
{
    public class MeshCollider : Collider
    {
        #region ContentProperties
        public List<Vertices> vertices { get; set; }
        #endregion

        internal override void AddCollider(Body body, float mass)
        {
            connectedBody = Physics.AddMesh(body, isTrigger, vertices, mass);
        }
    }
}
