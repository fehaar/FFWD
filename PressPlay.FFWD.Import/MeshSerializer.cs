using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace PressPlay.FFWD.Import
{
    [ContentTypeSerializer]
    public class MeshSerializer : ContentTypeSerializer<Mesh>
    {
        protected override Mesh Deserialize(IntermediateReader input, Microsoft.Xna.Framework.Content.ContentSerializerAttribute format, Mesh existingInstance)
        {
            return null;
        }

        protected override void Serialize(IntermediateWriter output, Mesh value, Microsoft.Xna.Framework.Content.ContentSerializerAttribute format)
        {
            throw new NotImplementedException();
        }
    }
}
