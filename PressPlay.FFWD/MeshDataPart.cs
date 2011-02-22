using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    internal class MeshDataPart
    {
        internal Microsoft.Xna.Framework.Vector3[] vertices;
        internal Microsoft.Xna.Framework.Vector3[] normals;
        internal Microsoft.Xna.Framework.Vector2[] uv;
        internal short[] triangles;

        internal Microsoft.Xna.Framework.BoundingSphere boundingSphere;
    }
}
