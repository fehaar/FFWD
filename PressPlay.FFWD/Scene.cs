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

        public bool hasBeenProcessed { get; set; }
        [ContentSerializer(Optional = true)]
        public int componentCount { get; set; }
        [ContentSerializer(FlattenContent = true, CollectionItemName = "go")]
        public List<GameObject> gameObjects { get; set; }
        [ContentSerializer(FlattenContent = true, CollectionItemName = "p")]
        public List<GameObject> prefabs { get; set; }
        [ContentSerializer(Optional = true, ElementName = "tc")]
        internal List<string> typeCaps = new List<string>();

        private Queue<Component> components;

        public void AfterLoad(Dictionary<int, UnityObject> idMap)
        {
            components = new Queue<Component>((componentCount > 0) ? componentCount : ApplicationSettings.DefaultCapacities.ComponentLists);
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].AfterLoad(idMap, components);
            }
            for (int i = 0; i < prefabs.Count; i++)
            {
                prefabs[i].isPrefab = true;
                prefabs[i].AfterLoad(idMap, components);
            }
        }

        internal void Initialize(bool doGarbageCollectAfterAwake)
        {
            Dictionary<int, UnityObject> idMap = new Dictionary<int, UnityObject>();
            AfterLoad(idMap);
            // Remove placeholder references and replace them with live ones
            Queue<IdMap> idMaps = new Queue<IdMap>();
            int count = components.Count;
            for (int i = 0; i < count; i++)
            {
                Component cmp = components.Dequeue();
                cmp.FixReferences(idMap);
                if (cmp is IdMap)
                {
                    idMaps.Enqueue(cmp as IdMap);
                }
                components.Enqueue(cmp);
            }
            idMap.Clear();
            // Issue new ids to all objects from a scene now that references have been fixed
            count = gameObjects.Count;
            for (int i = 0; i < count; i++)
            {
                gameObjects[i].SetNewId(idMap);
            }
            count = prefabs.Count;
            for (int i = 0; i < count; i++)
            {
                prefabs[i].SetNewId(idMap);
            }
            while (idMaps.Count > 0)
            {
                IdMap map = idMaps.Dequeue();
                map.UpdateIdReferences(idMap);
            }
            // Register and awake all components
            Application.RegisterComponents(components);
            Application.AwakeNewComponents();
            if (doGarbageCollectAfterAwake)
            {
                GC.Collect();
            }
        }
    }
}
