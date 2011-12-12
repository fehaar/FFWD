using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.SkinnedModel;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD
{
    public class MeshData
    {
        internal CpuSkinnedModel skinnedModel;
        internal Model model;

        internal Dictionary<string, MeshDataPart> meshParts;

        internal BoundingBox boundingBox;
    }
}   
