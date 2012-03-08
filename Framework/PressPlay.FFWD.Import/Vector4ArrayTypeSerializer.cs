using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Content;
using System.Globalization;

namespace PressPlay.FFWD.Import
{
    [ContentTypeSerializer]
    public class Vector4ArrayTypeSerializer : ContentTypeSerializer<Vector4[]>
    {
        protected override Vector4[] Deserialize(IntermediateReader input, ContentSerializerAttribute format, Vector4[] existingInstance)
        {
            string str = input.Xml.ReadContentAsString().Trim();
            if (string.IsNullOrWhiteSpace(str)) { return new Vector4[0]; }

            string[] s = str.Split(' ');
            if ((s.Length % 4) != 0)
            {
                throw new Exception(String.Format("Not enough floats in the string for Vector4[] in element {0}. Was {1}.", format.ElementName, s.Length));
            }
            Vector4[] vs = new Vector4[s.Length / 4];
            for (int i = 0; i < vs.Length; i++)
            {
                try
                {
                    int triIndex = i * 4;
                    vs[i][0] = float.Parse(s[triIndex]);
                    vs[i][1] = float.Parse(s[triIndex + 1]);
                    vs[i][2] = float.Parse(s[triIndex + 2]);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new IndexOutOfRangeException("Error reading Vector4[] from string in element " + format.ElementName + ". i was " + i + " for a limit of " + vs.Length + " using " + s.Length + " numbers");
                }
            }
            return vs;
        }

        protected override void Serialize(IntermediateWriter output, Vector4[] value, ContentSerializerAttribute format)
        {
            StringBuilder sb = new StringBuilder();
            if (value != null)
            {
                foreach (var v in value)
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture, "{0:0.#####} {1:0.#####} {2:0.#####} {3:0.#####} ", v.x, v.y, v.z, v.w);
                }
            }
            output.Xml.WriteString(sb.ToString());
        }
    }
}
