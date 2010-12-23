using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using PressPlay.FFWD.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using Box2D.XNA;
using PressPlay.FFWD;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD
{

    public enum SendMessageOptions
    {
        RequireReceiver = 0,
        DontRequireReceiver	= 1
    }

    public class GameObject : UnityObject
    {
        public GameObject()
            : base()
        {
            components = new List<Component>();
            AddComponent(new Transform());
            active = true;
        }

        internal GameObject(bool isPrefab)
        {
            this.isPrefab = isPrefab;
            components = new List<Component>();
            AddComponent(new Transform());
            active = true;
        }

        public GameObject(string name) : this()
        {
            this.name = name;
        }

        public string name { get; set; }
        public int layer { get; set; }
        public bool active { get; set; }
        public string tag { get; set; }

        #region Component shortcut methods
        private Rigidbody _rigidbody;
        [ContentSerializerIgnore]
        public Rigidbody rigidbody
        {
            get
            {
                if (_rigidbody == null)
                {
                    _rigidbody = GetComponent<Rigidbody>();
                }
                return _rigidbody;
            }
        }

        private Transform _transform;
        [ContentSerializerIgnore]
        public Transform transform 
        { 
            get
            {
                if (_transform == null)
                {
                    _transform = GetComponent<Transform>();
                }
                return _transform;
            }
        }

        private Collider _collider;
        [ContentSerializerIgnore]
        public Collider collider
        {
            get
            {
                if (_collider == null)
                {
                    _collider = GetComponent<Collider>();
                }
                return _collider;
            }
        }

        private Renderer _renderer;
        [ContentSerializerIgnore]
        public Renderer renderer
        {
            get
            {
                if (_renderer == null)
                {
                    _renderer = GetComponent<Renderer>();
                }
                return _renderer;
            }
        }

        [ContentSerializerIgnore]
        public AudioSource audio
        {
            get
            {
                if (_audio == null)
                {
                    _audio = GetComponent<AudioSource>();
                }
                return _audio;
            }
        }
        #endregion

        //public String prefab { get; set; }
        [ContentSerializer(CollectionItemName = "component")]
        private List<Component> components { get; set; }

        internal override void AfterLoad()
        {
            base.AfterLoad();
            for (int j = 0; j < components.Count; j++)
            {
                components[j].isPrefab = isPrefab;
                components[j].AfterLoad();
                components[j].gameObject = this;
            }
        }

        public T AddComponent<T>(T component) where T : Component
        {
            if (component is Transform && components.Count > 0)
            {
                throw new InvalidOperationException("A GameObject already has a Transform");
            }
            components.Add(component);
            component.gameObject = this;
            component.isPrefab = isPrefab;
            return component;
        }

        public Component AddComponent(Type tp)
        {
            Component cmp = Activator.CreateInstance(tp) as Component;
            return AddComponent(cmp);
        }

        protected AudioSource _audio;

        #region Internal methods
        internal override UnityObject Clone()
        {
            GameObject obj = base.Clone() as GameObject;
            obj.name = name + "(Clone)";
            obj.active = true;
            obj.isPrefab = false;

            // Reset lazy shortcut properties
            obj._transform = null;
            obj._rigidbody = null;
            obj._collider = null;
            obj._renderer = null;
            obj._audio = null;

            obj.components = new List<Component>();
            for (int i = 0; i < components.Count; i++)
            {
                obj.AddComponent(components[i].Clone() as Component);
            }
            return obj;
        }

        internal override void SetNewId(Dictionary<int, UnityObject> idMap)
        {
            base.SetNewId(idMap);
            for (int i = 0; i < components.Count; i++)
            {
                components[i].SetNewId(idMap);
            }
        }

        internal override void FixReferences(Dictionary<int, UnityObject> idMap)
        {
            base.FixReferences(idMap);
            for (int i = 0; i < components.Count; i++)
            {
                components[i].FixReferences(idMap);
            }
        }
        #endregion

        #region Update and event methods
        internal void FixedUpdate()
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (!components[i].isStarted)
                {
                    components[i].Start();
                    components[i].isStarted = true;
                }
                if (Application.IsAwake(components[i]) && components[i] is IFixedUpdateable)
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
                if (Application.IsAwake(components[i]) && components[i] is IUpdateable)
                {
                    (components[i] as IUpdateable).Update();
                }
            }
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (Application.IsAwake(components[i]) && components[i] is IRenderable)
                {
                    (components[i] as IRenderable).Draw(spriteBatch);
                }
            }
        }

        internal void OnTriggerEnter(Contact contact)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (Application.IsAwake(components[i]) && components[i] is ICollidable)
                {
                    (components[i] as ICollidable).OnTriggerEnter(contact);
                }
            }
        }

        internal void OnTriggerExit(Contact contact)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (Application.IsAwake(components[i]) && components[i] is ICollidable)
                {
                    (components[i] as ICollidable).OnTriggerExit(contact);
                }
            }
        }

        internal void OnCollisionEnter(Contact contact)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (Application.IsAwake(components[i]) && components[i] is ICollidable)
                {
                    (components[i] as ICollidable).OnCollisionEnter(contact);
                }
            }
        }

        internal void OnCollisionExit(Contact contact)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (Application.IsAwake(components[i]) && components[i] is ICollidable)
                {
                    (components[i] as ICollidable).OnCollisionExit(contact);
                }
            }
        }

        internal void OnPreSolve(Contact contact, Manifold manifold)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (Application.IsAwake(components[i]) && components[i] is ICollidable)
                {
                    (components[i] as ICollidable).OnPreSolve(contact, manifold);
                }
            }
        }

        internal void OnPostSolve(Contact contact, ContactImpulse contactImpulse)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (Application.IsAwake(components[i]) && components[i] is ICollidable)
                {
                    (components[i] as ICollidable).OnPostSolve(contact, contactImpulse);
                }
            }
        }
        #endregion

        #region Component locator methods
        public T GetComponent<T>() where T : Component
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is T)
                {
                    return components[i] as T;
                }
            }
            return default(T);
        }

        public Component GetComponent(Type type)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType().IsAssignableFrom(type))
                {
                    return components[i];
                }
            }
            return null;
        }

        public T[] GetComponents<T>() where T : Component
        {
            List<T> list = new List<T>();
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is T)
                {
                    list.Add(components[i] as T);
                }
            }
            return list.ToArray();
        }

        public Component[] GetComponents(Type type)
        {
            List<Component> list = new List<Component>();
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType().IsAssignableFrom(type))
                {
                    list.Add(components[i]);
                }
            }
            return list.ToArray();
        }

        public Component[] GetComponentsInChildren(Type type)
        {
            List<Component> list = new List<Component>();
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType().IsAssignableFrom(type))
                {
                    list.Add(components[i]);
                }
            }
            transform.GetComponentsInChildrenInt(type, list);
            return list.ToArray();
        }

        public Component GetComponentInChildren(Type type)
        {
            Component cmp = transform.GetComponentInChildrenInt(type);
            if (cmp != null)
            {
                return cmp;
            }
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType().IsAssignableFrom(type))
                {
                    return components[i];
                }
            }
            return null;
        }

        public T[] GetComponentsInParents<T>() where T : Component
        {
            List<T> list = new List<T>();
            GameObject go = GetParent();
            while (go != null)
            {
                list.AddRange(go.GetComponents<T>());
                go = go.GetParent();
            }
            return list.ToArray();
        }

        private GameObject GetParent()
        {
            return (transform != null && transform.parent != null) ? transform.parent.gameObject : null;
        }
        #endregion

        #region Unity methods
        public void SetActiveRecursively(bool state)
        {
            active = state;
            transform.SetActiveRecursively(state);
        }

        public bool CompareTag(string tag)
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");

        }

        /// <summary>
        /// Returns a list of active GameObjects tagged tag. Returns null if no GameObject was found.
        /// Tags must be declared in the tag manager before using them.
        /// </summary>
        /// <param name="tag">The tag to find</param>
        /// <returns></returns>
        public static GameObject FindWithTag(string tag)
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");
        }

        /// <summary>
        /// Returns a list of active GameObjects tagged tag. Returns null if no GameObject was found.
        /// Tags must be declared in the tag manager before using them.
        /// </summary>
        /// <param name="tag">The tag to find</param>
        /// <returns></returns>
        public static GameObject[] FindGameObjectsWithTag(string tag)
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");
        }

        /// <summary>
        /// Finds a game object by name and returns it.
        /// If no game object with name can be found, null is returned. If name contains a '/' character it will traverse the hierarchy like a path name. This function only returns active gameobjects.
        /// For performance reasons it is recommended to not use this function every frame Instead cache the result in a member variable at startup or use GameObject.FindWithTag.
        /// </summary>
        /// <param name="name">The name of the GameObject to find</param>
        /// <returns></returns>
        public static GameObject Find(string name)
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");
        }

        public void SendMessageUpwards(string methodName, object value, SendMessageOptions SendMessageOptions)
        {
            throw new NotImplementedException("SendMessageUpwards is not implemented");
        }

        public void SendMessage(string methodName, object value, SendMessageOptions SendMessageOptions)
        {
            throw new NotImplementedException("SendMessage is not implemented");
        }

        #endregion

        public override string ToString()
        {
            return name + "(" + GetInstanceID() + ")";
        }
    }
}
