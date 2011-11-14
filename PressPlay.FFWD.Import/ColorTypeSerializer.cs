using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace PressPlay.FFWD.Import
{
    [ContentTypeSerializer]
    public class ColorTypeSerializer : ContentTypeSerializer<Color>
    {
        protected override Color Deserialize(IntermediateReader input, Microsoft.Xna.Framework.Content.ContentSerializerAttribute format, Color existingInstance)
        {
            string s = input.Xml.ReadContentAsString();
            if (s.Length != 8 && s.Length != 6)
            {
                throw new Exception("Color string must either be AARRGGBB or RRGGBB. Was: " + s);
            }
            return Color.Parse(s);
        }

        protected override void Serialize(IntermediateWriter output, Color c, Microsoft.Xna.Framework.Content.ContentSerializerAttribute format)
        {
            output.Xml.WriteElementString(format.ElementName, ((int)(c.a * 255)).ToString("X") + ((int)(c.r * 255)).ToString("X") + ((int)(c.g * 255)).ToString("X") + ((int)(c.b * 255)).ToString("X"));
        }
    }
}
