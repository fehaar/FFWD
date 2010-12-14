using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
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

        #region Component shortcut properties
        [ContentSerializerIgnore]
        public Transform transform
        {
            get
            {
                if (gameObject == null)
                {
                    return null;
                }
                return gameObject.transform;
            }
        }

        [ContentSerializerIgnore]
        public Rigidbody rigidbody
        {
            get
            {
                if (gameObject == null)
                {
                    return null;
                }
                return gameObject.rigidbody;
            }
        }

        [ContentSerializerIgnore]
        public Collider collider
        {
            get
            {
                if (gameObject == null)
                {
                    return null;
                }
                return gameObject.collider;
            }
        }
        #endregion

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
            if (gameObject == null)
            {
                return GetType().Name + " on its own";
            }
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

        public Component GetComponent(Type type)
        {
            return gameObject.GetComponent(type);
        }
    }
}
