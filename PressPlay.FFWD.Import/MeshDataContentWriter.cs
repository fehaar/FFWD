using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace PressPlay.FFWD.Import
{
    [ContentTypeWriter]
    class MeshDataContentWriter : ContentTypeWriter<MeshDataContent>
    {
        protected override void Write(ContentWriter output, MeshDataContent value)
        {
            output.WriteObject(value.skinnedModel);
            output.WriteObject(value.model);
            output.WriteObject(value.meshParts);
            output.WriteObject(value.boundingBox);
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "PressPlay.FFWD.MeshData, PressPlay.FFWD";
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "PressPlay.FFWD.MeshDataReader, PressPlay.FFWD";
        }
    }
}
