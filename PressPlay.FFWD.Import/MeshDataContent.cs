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
        public MeshDataContent()
        {
            meshParts = new Dictionary<string, MeshDataPart>();
        }

        internal CpuSkinnedModelContent skinnedModel;
        internal ModelContent model;

        internal Dictionary<string, MeshDataPart> meshParts;

        internal Microsoft.Xna.Framework.BoundingSphere boundingSphere;
    }
}
