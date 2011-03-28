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

        protected override void DoAddCollider(Body body, float mass)
        {
            Microsoft.Xna.Framework.Vector2 scale = transform.lossyScale;
            for (int i = 0; i < vertices.Count; i++)
            {
                Vertices v = vertices[i];
                for (int j = 0; j < v.Count; j++)
                {
                    v[j] = v[j] * scale;
                }
            }

            connectedBody = body;
            Physics.AddMesh(body, isTrigger, vertices, mass);
        }
    }
}
