using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Test.Core_framework
{
    public class ReferencingComponent : Component
    {
        public Component reference;
        public Component[] componentArray;
        public List<Component> componentList;

        public Component referenceProperty { get; set; }
        [ContentSerializerIgnore]
        public Component ignoredReferenceProperty { get; set; }
    }
}
