using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD
{
    public abstract class Component : UnityObject
    {
        public Component()
            : base()
        {
            Application.AddNewComponent(this);
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

        [ContentSerializerIgnore]
        public Renderer renderer
        {
            get
            {
                return gameObject.renderer;
            }
        }

        [ContentSerializerIgnore]
        public AudioSource audio
        {
            get
            {
                return gameObject.audio;
            }
        }

        [ContentSerializerIgnore]
        public Rigidbody rigidbody
        {
            get
            {
                return gameObject.rigidbody;
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

        internal override UnityObject Clone()
        {
            UnityObject obj = base.Clone();
            obj.isPrefab = false;
            Application.AddNewComponent(obj as Component);
            return obj;
        }

        public string name
        {
            get
            {
                return (gameObject == null) ? GetType().Name : gameObject.name;
            }
        }

        #region Component locator methods
        public T GetComponent<T>() where T : Component
        {
            return gameObject.GetComponent<T>();
        }

        public Component GetComponent(Type type)
        {
            return gameObject.GetComponent(type);
        }

        public T[] GetComponents<T>() where T : Component
        {
            return gameObject.GetComponents<T>();
        }

        public Component[] GetComponents(Type type)
        {
            return GetComponents(type);
        }

        public Component[] GetComponentsInChildren(Type type)
        {
            return gameObject.GetComponentsInChildren(type);
        }

        public Component GetComponentInChildren(Type type)
        {
            return gameObject.GetComponentInChildren(type);
        }

        public T[] GetComponentsInParents<T>() where T : Component
        {
            return GetComponentsInParents<T>();
        }
        #endregion
    }
}
