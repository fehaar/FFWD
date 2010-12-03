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

        /// <summary>
        /// This should be called on every Update in the game
        /// </summary>
        public void FixedUpdate()
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].FixedUpdate();
            }
        }

        /// <summary>
        /// This should be called on every Draw call in the game before Draw is called
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].Update();
            }
        }

        /// <summary>
        /// This should be called on every Draw call in the game
        /// </summary>
        /// <param name="spriteBatch"></param>
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
                gameObjects[i].AfterLoad();
            }
        }
    }
}
