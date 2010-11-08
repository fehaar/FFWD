using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.U2X.Xna
{
    public class GameObject
    {
        public int id { get; set; }
        public string name { get; set; }
        public Transform transform { get; set; }
        public String prefab { get; set; }
        [ContentSerializer(CollectionItemName = "component")]
        public List<Component> components { get; set; }
    }
}
