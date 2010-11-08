using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using PressPlay.U2X.Xna.Interfaces;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.U2X.Xna
{
    public class GameObject
    {
        public GameObject()
        {
            components = new List<Component>();
        }

        public int id { get; set; }
        public string name { get; set; }
        public Transform transform { get; set; }
        public String prefab { get; set; }
        [ContentSerializer(CollectionItemName = "component")]
        public List<Component> components { get; set; }

        internal void FixedUpdate()
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (!components[i].isStarted)
                {
                    components[i].Start();
                    components[i].isStarted = true;
                }
                if (Component.IsAwake(components[i]) && components[i] is IFixedUpdateable)
                {
                    (components[i] as IFixedUpdateable).FixedUpdate();
                }
            }
        }

        internal void Update()
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (!components[i].isStarted)
                {
                    components[i].Start();
                    components[i].isStarted = true;
                }
                if (Component.IsAwake(components[i]) && components[i] is IUpdateable)
                {
                    (components[i] as IUpdateable).Update(null);
                }
            }            
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (Component.IsAwake(components[i]) && components[i] is IRenderable)
                {
                    (components[i] as IRenderable).Draw(spriteBatch);
                }
            }            
        }
    }
}
