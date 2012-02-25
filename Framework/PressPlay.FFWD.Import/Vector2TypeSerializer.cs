using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Import
{
    [ContentTypeSerializer]
    public class Vector2TypeSerializer : ContentTypeSerializer<Vector2>
    {
        protected override Vector2 Deserialize(IntermediateReader input, ContentSerializerAttribute format, Vector2 existingInstance)
        {
            string[] s = input.Xml.ReadContentAsString().Split(' ');
            if (s.Length != 2)
            {
                throw new Exception("Not enough floats in the string " + s);
            }
            Vector2 v = new Vector2();
            for (int i = 0; i < s.Length; i++)
            {
                v[i] = float.Parse(s[i]);
            }
            return v;
        }

        protected override void Serialize(IntermediateWriter output, Vector2 value, ContentSerializerAttribute format)
        {
            output.Xml.WriteString(value.x + " " + value.y);
        }
    }
}
