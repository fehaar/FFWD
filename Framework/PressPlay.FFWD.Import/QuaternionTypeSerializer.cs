using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Import
{
    [ContentTypeSerializer]
    public class QuaternionTypeSerializer : ContentTypeSerializer<Quaternion>
    {
        protected override Quaternion Deserialize(IntermediateReader input, ContentSerializerAttribute format, Quaternion existingInstance)
        {
            string[] s = input.Xml.ReadContentAsString().Split(' ');
            if (s.Length != 4)
            {
                throw new Exception("We need four floats to convert to a Quaternion. Was " + s);
            }
            Quaternion v = new Quaternion();
            for (int i = 0; i < s.Length; i++)
            {
                v[i] = float.Parse(s[i]);
            }
            return v;
        }

        protected override void Serialize(IntermediateWriter output, Quaternion value, ContentSerializerAttribute format)
        {
            throw new NotImplementedException();
        }
    }
}
