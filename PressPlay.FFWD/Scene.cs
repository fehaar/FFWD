using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Box2D.XNA;

namespace PressPlay.FFWD
{
    public class Scene
    {
        public Scene()
        {
            gameObjects = new List<GameObject>();
        }

        [ContentSerializer(FlattenContent = true, CollectionItemName = "gameObject")]
        public List<GameObject> gameObjects { get; set; }

        public void FixedUpdate()
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].FixedUpdate();
            }
        }

        public void Update()
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].Draw(spriteBatch);
            }
        }

        public void DebugDraw(DebugDraw drawer)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].OnDebugDraw(drawer);
            }
        }

        public void AfterLoad()
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                AfterLoad(gameObjects[i]);
            }
        }

        private void AfterLoad(GameObject gameObject)
        {
            for (int j = 0; j < gameObject.components.Count; j++)
            {
                gameObject.components[j].gameObject = gameObject;
            }
            if (gameObject.transform != null && gameObject.transform.children != null)
            {
                for (int i = 0; i < gameObject.transform.children.Count; i++)
                {
                    gameObject.transform.children[i].transform._parent = gameObject.transform;
                    AfterLoad(gameObject.transform.children[i]);
                }
            }
        }
    }
}
