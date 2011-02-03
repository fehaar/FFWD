using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.SkinnedModel;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD
{
    internal class MeshDataContent
    {
        [ContentSerializer]
        internal Microsoft.Xna.Framework.Vector3[] vertices { get; set; }
        [ContentSerializer]
        internal Microsoft.Xna.Framework.Vector3[] normals { get; set; }
        [ContentSerializer]
        internal Microsoft.Xna.Framework.Vector2[] uv { get; set; }
        [ContentSerializer]
        internal short[] triangles { get; set; }
    }
}   
