using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.U2X.Xna
{
    public class Scene
    {
        [ContentSerializer(FlattenContent = true, CollectionItemName = "gameObject")]
        public List<GameObject> gameObjects { get; set; }
    }
}
