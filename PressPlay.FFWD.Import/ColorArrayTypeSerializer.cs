using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Import
{
    [ContentTypeSerializer]
    public class ColorArrayTypeSerializer : ContentTypeSerializer<Color[]>
    {
        protected override Color[] Deserialize(IntermediateReader input, ContentSerializerAttribute format, Color[] existingInstance)
        {
            string[] s = input.Xml.ReadContentAsString().Trim().Split(' ');
            Color[] vs = new Color[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                vs[i] = Color.Parse(s[i]);
            }
            return vs;
        }

        protected override void Serialize(IntermediateWriter output, Color[] value, ContentSerializerAttribute format)
        {
            if (value != null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    output.Xml.WriteString(value[i].ToString() + " ");
                }
            }
        }
    }
}
