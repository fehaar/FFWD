using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using PressPlay.Tentacles.Scripts;

namespace PressPlay.FFWD.Import
{
    [ContentTypeSerializer]
    public class ObjectReferenceTypeSerializer : ContentTypeSerializer<ObjectReference>
    {
        protected override ObjectReference Deserialize(IntermediateReader input, ContentSerializerAttribute format, ObjectReference existingInstance)
        {
            return new ObjectReference() { Id = input.Xml.ReadContentAsInt() };
        }

        protected override void Serialize(IntermediateWriter output, ObjectReference value, ContentSerializerAttribute format)
        {
            throw new NotImplementedException();
        }
    }
}
