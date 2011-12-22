using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Import
{
    [ContentTypeSerializer]
    public class Vector3ArrayTypeSerializer : ContentTypeSerializer<Vector3[]>
    {
        protected override Vector3[] Deserialize(IntermediateReader input, ContentSerializerAttribute format, Vector3[] existingInstance)
        {
            string str = input.Xml.ReadContentAsString().Trim();
            if (string.IsNullOrWhiteSpace(str)) { return new Vector3[0]; }

            string[] s = str.Split(' ');
            if ((s.Length % 3) != 0)
            {
                throw new Exception(String.Format("Not enough floats in the string for Vector3[] in element {0}. Was {1}.", format.ElementName, s.Length));
            }
            Vector3[] vs = new Vector3[s.Length / 3];
            for (int i = 0; i < vs.Length; i++)
            {
                try
                {
                    int triIndex = i * 3;
                    vs[i][0] = float.Parse(s[triIndex]);
                    vs[i][1] = float.Parse(s[triIndex + 1]);
                    vs[i][2] = float.Parse(s[triIndex + 2]);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new IndexOutOfRangeException("Error reading Vector3[] from string in element " + format.ElementName + ". i was " + i + " for a limit of " + vs.Length + " using " + s.Length + " numbers");
                }
            }
            return vs;
        }

        protected override void Serialize(IntermediateWriter output, Vector3[] value, ContentSerializerAttribute format)
        {
            throw new NotImplementedException();
        }
    }
}
