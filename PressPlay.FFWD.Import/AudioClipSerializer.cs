using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace PressPlay.FFWD.Import
{
    [ContentTypeSerializer]
    public class AudioClipSerializer : ContentTypeSerializer<AudioClip>
    {
        protected override AudioClip Deserialize(IntermediateReader input, Microsoft.Xna.Framework.Content.ContentSerializerAttribute format, AudioClip existingInstance)
        {
            string s = input.Xml.ReadContentAsString();
            AudioClip c = new AudioClip() { clip = s };
            return c;
        }

        protected override void Serialize(IntermediateWriter output, AudioClip value, Microsoft.Xna.Framework.Content.ContentSerializerAttribute format)
        {
            throw new NotImplementedException();
        }
    }
}
