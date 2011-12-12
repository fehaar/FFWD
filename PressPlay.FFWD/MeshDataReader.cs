using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using PressPlay.FFWD.SkinnedModel;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD
{
    public class MeshDataReader : ContentTypeReader<MeshData>
    {
        protected override MeshData Read(ContentReader input, MeshData existingInstance)
        {
            MeshData data = new MeshData();
            data.skinnedModel = input.ReadObject<CpuSkinnedModel>();
            data.model = input.ReadObject<Model>();
            data.meshParts = input.ReadObject<Dictionary<string, MeshDataPart>>();
            data.boundingBox = input.ReadObject<Microsoft.Xna.Framework.BoundingBox>();
            return data;
        }
    }
}
