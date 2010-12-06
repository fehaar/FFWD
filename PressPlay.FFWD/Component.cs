using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD
{
    public abstract class Component : UnityObject
    {
        public Component()
        {
            NewComponents.Add(this);
        }

        private static List<Component> NewComponents = new List<Component>();

        public static void AwakeNewComponents()
        {
            for (int i = 0; i < NewComponents.Count; i++)
            {
                NewComponents[i].Awake();
            }
            NewComponents.Clear();
        }

        internal static bool IsAwake(Component component)
        {
            return !NewComponents.Contains(component);
        }

        internal bool isStarted = false;

        public virtual GameObject gameObject { get; internal set; }

        [ContentSerializerIgnore]
        public Transform transform
        {
            get
            {
                return gameObject.transform;
            }
        }

        public virtual void Awake()
        {
            // NOTE: Do not do anything here as the convention in Unity is not to call base as it is not a virtual method
        }

        public virtual void Start()
        {
            // NOTE: Do not do anything here as the convention in Unity is not to call base as it is not a virtual method
        }

        public override string ToString()
        {
            return GetType().Name + " on " + gameObject.name + " (" + gameObject.GetInstanceID() + ")";
        }

        public void Destroy(Component component)
        {
            // TODO: Objects should be destroyed after Update but before Rendering
        }

        public void Destroy(GameObject go)
        {
            // TODO: Objects should be destroyed after Update but before Rendering
        }

        public string name
        {
            get
            {
                return (gameObject == null) ? GetType().Name : gameObject.name;
            }
        }
    }
}
