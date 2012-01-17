using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace PressPlay.FFWD.Import
{
    [ContentTypeSerializer]
    public class RectTypeSerializer : ContentTypeSerializer<Rect>
    {
        protected override Rect Deserialize(IntermediateReader input, Microsoft.Xna.Framework.Content.ContentSerializerAttribute format, Rect existingInstance)
        {
            string[] s = input.Xml.ReadContentAsString().Split(' ');
            if (s.Length != 4)
            {
                throw new Exception("We need four ints to convert to a Rect. Was " + s);
            }
            Rect r = new Rect();
            r.x = Int32.Parse(s[0]);
            r.y = Int32.Parse(s[1]);
            r.width = Int32.Parse(s[2]);
            r.height = Int32.Parse(s[3]);
            return r;
        }

        protected override void Serialize(IntermediateWriter output, Rect value, Microsoft.Xna.Framework.Content.ContentSerializerAttribute format)
        {
            throw new NotImplementedException();
        }
    }
}
