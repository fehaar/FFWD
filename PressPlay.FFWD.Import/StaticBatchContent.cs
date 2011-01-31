using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace PressPlay.FFWD.Import
{
    [ContentSerializerRuntimeType("PressPlay.FFWD.Components.StaticBatchRenderer, PressPlay.FFWD")]
    public class StaticBatchContent : Component
    {
        public ModelContent model;

        public Material[] materials;

        //public short[] triangles;
        //public float[] vertices;
        //public float[] normals;
        //public float[] uv;
    }
}
