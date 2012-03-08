using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Content;
using PressPlay.FFWD;

namespace PressPlay.FFWD.Import
{
    [ContentTypeSerializer]
    public class LayerMaskTypeSerializer : ContentTypeSerializer<LayerMask>
    {
        protected override LayerMask Deserialize(IntermediateReader input, ContentSerializerAttribute format, LayerMask existingInstance)
        {
            return new LayerMask() { value = input.Xml.ReadContentAsInt() };
        }

        protected override void Serialize(IntermediateWriter output, LayerMask value, ContentSerializerAttribute format)
        {
            throw new NotImplementedException();
        }
    }
}
