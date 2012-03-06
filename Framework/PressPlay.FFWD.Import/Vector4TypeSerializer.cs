using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Import
{
    [ContentTypeSerializer]
    public class Vector4TypeSerializer : ContentTypeSerializer<Vector4>
    {
        protected override Vector4 Deserialize(IntermediateReader input, ContentSerializerAttribute format, Vector4 existingInstance)
        {
            string[] s = input.Xml.ReadContentAsString().Split(' ');
            if (s.Length != 4)
            {
                throw new Exception("Not enough floats in the string " + s);
            }
            Vector4 v = new Vector4();
            for (int i = 0; i < s.Length; i++)
            {
                v[i] = float.Parse(s[i]);
            }
            return v;
        }

        protected override void Serialize(IntermediateWriter output, Vector4 value, ContentSerializerAttribute format)
        {
            output.Xml.WriteString(value.x + " " + value.y + " " + value.z);
        }
    }
}
