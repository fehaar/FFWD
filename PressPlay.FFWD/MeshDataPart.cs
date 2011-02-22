using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD
{
    internal class MeshDataPart
    {
        [ContentSerializer]
        internal Microsoft.Xna.Framework.Vector3[] vertices;
        [ContentSerializer]
        internal Microsoft.Xna.Framework.Vector3[] normals;
        [ContentSerializer]
        internal Microsoft.Xna.Framework.Vector2[] uv;
        [ContentSerializer]
        internal short[] triangles;

        [ContentSerializer]
        internal Microsoft.Xna.Framework.BoundingSphere boundingSphere;
    }
}
