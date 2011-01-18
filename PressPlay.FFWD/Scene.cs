using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Box2D.XNA;

namespace PressPlay.FFWD
{
    internal class Scene
    {
        internal Scene()
        {
            gameObjects = new List<GameObject>();
            prefabs = new List<GameObject>();
        }

        [ContentSerializer(FlattenContent = true, CollectionItemName = "gameObject")]
        public List<GameObject> gameObjects { get; set; }
        [ContentSerializer(FlattenContent = true, CollectionItemName = "prefab")]
        public List<GameObject> prefabs { get; set; }

        public void AfterLoad(Dictionary<int, UnityObject> idMap)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].AfterLoad(idMap);
            }
            for (int i = 0; i < prefabs.Count; i++)
            {
                prefabs[i].isPrefab = true;
                prefabs[i].AfterLoad(idMap);
            }
        }
    }
}
