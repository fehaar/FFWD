using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Test.Core_framework
{
    public class ReferencingComponent : Component
    {
        public ReferencingComponent()
        {
        }

        public ReferencingComponent(Component serialized)
        {
            serializedReference = serialized;
        }

        public Component reference;
        public Component[] componentArray;
        public List<Component> componentList;

        public Component referenceProperty { get; set; }
        [ContentSerializerIgnore]
        public Component ignoredReferenceProperty { get; set; }
        [ContentSerializer]
        Component serializedReference;

        public Component GetSerializedProperty()
        {
            return serializedReference;
        }
    }

    public class ReferencingComponentSubClass : ReferencingComponent
    {
        public ReferencingComponentSubClass(Component serialized)
            : base(serialized)
        {
        }
    }
}
