using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.Import.Animation;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace PressPlay.FFWD.Import
{
    class MeshDataContent
    {
        internal CpuSkinnedModelContent skinnedModel;
        internal ModelContent model;

        internal Microsoft.Xna.Framework.Vector3[] vertices { get; set; }
        internal Microsoft.Xna.Framework.Vector3[] normals { get; set; }
        internal Microsoft.Xna.Framework.Vector2[] uv { get; set; }
        internal short[] triangles { get; set; }
    }
}
