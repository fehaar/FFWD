using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace PressPlay.FFWD.Import
{
    [ContentTypeSerializer]
    public class GuidSerializer : ContentTypeSerializer<Guid>
    {
        protected override Guid Deserialize(IntermediateReader input, Microsoft.Xna.Framework.Content.ContentSerializerAttribute format, Guid existingInstance)
        {
            string s = input.Xml.ReadContentAsString();
            Guid g;
            if (Guid.TryParse(s, out g))
            {
                return g;
            }
            return Guid.Empty;
        }

        protected override void Serialize(IntermediateWriter output, Guid value, Microsoft.Xna.Framework.Content.ContentSerializerAttribute format)
        {
            output.Xml.WriteElementString(format.ElementName, value.ToString());
        }
    }
}
