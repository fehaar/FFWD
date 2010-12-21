using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Import
{
    [ContentTypeSerializer]
    public class Vector3TypeSerializer : ContentTypeSerializer<Vector3>
    {
        protected override Vector3 Deserialize(IntermediateReader input, ContentSerializerAttribute format, Vector3 existingInstance)
        {
            string[] s = input.Xml.ReadContentAsString().Split(' ');
            if (s.Length != 3)
            {
                throw new Exception("Not enough floats in the string " + s);
            }
            Vector3 v = new Vector3();
            for (int i = 0; i < s.Length; i++)
            {
                v[i] = float.Parse(s[i]);
            }
            return v;
        }

        protected override void Serialize(IntermediateWriter output, Vector3 value, ContentSerializerAttribute format)
        {
            output.Xml.WriteString(value.x + " " + value.y + " " + value.z);
        }
    }
}
