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
        public short[] triangles { get; set; }
        public Vector3[] vertices { get; set; }
        #endregion

        internal override void AddCollider(Body body, float mass)
        {
            List<Triangle> tris = new List<Triangle>();
            for (int i = 0; i < triangles.Length; i += 3)
            {
#if DEBUG
            if (vertices[triangles[i]].y + vertices[triangles[i + 1]].y + vertices[triangles[i + 2]].y > 1)
            {
                Debug.Log(" Warning: " + ToString() + " has non zero Y in collider");
            }
#endif
                Triangle tri = new Triangle(
                    vertices[triangles[i]].x * transform.lossyScale.x, vertices[triangles[i]].z * transform.lossyScale.z,
                    vertices[triangles[i + 2]].x * transform.lossyScale.x, vertices[triangles[i + 2]].z * transform.lossyScale.z,
                    vertices[triangles[i + 1]].x * transform.lossyScale.x, vertices[triangles[i + 1]].z * transform.lossyScale.z
                );
                tris.Add(tri);
            }

            List<Vertices> verts = EarclipDecomposer.PolygonizeTriangles(tris, int.MaxValue, 0);

            connectedBody = Physics.AddMesh(body, isTrigger, verts, mass);
        }
    }
}
