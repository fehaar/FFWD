using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD
{
    public class Mesh : UnityObject
    {
        public Vector3[] vertices { get; set; }
        public Vector3[] normals { get; set; }
        public Vector2[] uv { get; set; }
        public short[] triangles { get; set; }

        public void Clear()
        {
            vertices = null;
            normals = null;
            uv = null;
            triangles = null;
        }
    }
}
