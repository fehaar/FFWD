using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.U2X.Xna
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

        public void AfterLoad()
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                for (int j = 0; j < gameObjects[i].components.Count; j++)
                {
                    gameObjects[i].components[j].gameObject = gameObjects[i];
                }
            }
        }
    }
}
