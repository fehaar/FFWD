using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD
{
    public class Scene
    {
        internal Scene()
        {
            gameObjects = new List<GameObject>();
            prefabs = new List<GameObject>();
        }

        [ContentSerializer(FlattenContent = true, CollectionItemName = "go")]
        public List<GameObject> gameObjects { get; set; }
        [ContentSerializer(FlattenContent = true, CollectionItemName = "p")]
        public List<GameObject> prefabs { get; set; }
        [ContentSerializer(Optional = true, ElementName = "tc")]
        internal List<string> typeCaps = new List<string>();

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

        internal void Initialize()
        {
            Dictionary<int, UnityObject> idMap = new Dictionary<int, UnityObject>();
            AfterLoad(idMap);
            // Remove placeholder references and replace them with live ones
            List<IdMap> idMaps = new List<IdMap>();
            for (int i = 0; i < Application.newComponents.Count; i++)
            {
                Application.newComponents[i].FixReferences(idMap);
                if (Application.newComponents[i] is IdMap)
                {
                    idMaps.Add(Application.newComponents[i] as IdMap);
                }
            }
            idMap.Clear();
            // Issue new ids to all objects from a scene now that references have been fixed
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].SetNewId(idMap);
            }
            for (int i = 0; i < prefabs.Count; i++)
            {
                prefabs[i].SetNewId(idMap);
            }
            for (int i = 0; i < idMaps.Count; i++)
            {
                idMaps[i].UpdateIdReferences(idMap);
                Application.newComponents.Remove(idMaps[i]);
            }
        }
    }
}
