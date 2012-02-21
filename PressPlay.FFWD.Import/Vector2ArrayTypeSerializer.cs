using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Import
{
    [ContentTypeSerializer]
    public class Vector2ArrayTypeSerializer : ContentTypeSerializer<Vector2[]>
    {
        protected override Vector2[] Deserialize(IntermediateReader input, ContentSerializerAttribute format, Vector2[] existingInstance)
        {
            string str = input.Xml.ReadContentAsString().Trim();
            if (string.IsNullOrWhiteSpace(str)) { return new Vector2[0]; }

            string[] s = str.Split(' ');
            if ((s.Length % 2) != 0)
            {
                throw new Exception(String.Format("Not enough floats in the string for Vector2[] in element {0}. Was {1}.", format.ElementName, s.Length));
            }
            Vector2[] vs = new Vector2[s.Length / 2];
            for (int i = 0; i < vs.Length; i++)
            {
                try
                {
                    int triIndex = i * 2;
                    vs[i][0] = float.Parse(s[triIndex]);
                    vs[i][1] = float.Parse(s[triIndex + 1]);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new IndexOutOfRangeException("Error reading Vector2[] from string in element " + format.ElementName + ". i was " + i + " for a limit of " + vs.Length + " using " + s.Length + " numbers");
                }
            }
            return vs;
        }

        protected override void Serialize(IntermediateWriter output, Vector2[] value, ContentSerializerAttribute format)
        {
            throw new NotImplementedException();
        }
    }
}
