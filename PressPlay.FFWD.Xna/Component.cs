using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.U2X.Xna
{
    public abstract class Component
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

        public GameObject gameObject { get; internal set; }

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
            return GetType().Name + " on " + gameObject.name + " (" + gameObject.id + ")";
        }
    }
}
