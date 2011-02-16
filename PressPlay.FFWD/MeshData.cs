using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.SkinnedModel;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD
{
    public class MeshData
    {
        internal CpuSkinnedModel skinnedModel;
        internal Model model;

        internal Microsoft.Xna.Framework.Vector3[] vertices { get; set; }
        internal Microsoft.Xna.Framework.Vector3[] normals { get; set; }
        internal Microsoft.Xna.Framework.Vector2[] uv { get; set; }
        internal short[] triangles { get; set; }
    }
}   
