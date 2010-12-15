using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Components
{
    public class MeshFilter : Component
    {
        [ContentSerializerIgnore]
        public Mesh sharedMesh { get; set; }
    }
}
