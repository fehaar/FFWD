using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework.Content;
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
            if (!Application.loadingScene)
            {
                AddComponent(new Transform());
            }
            active = true;
        }

        internal GameObject(bool isPrefab)
            : base()
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

        [ContentSerializer(Optional = true)]
        public string name { get; set; }
        [ContentSerializer(Optional = true)]
        public int layer { get; set; }
        [ContentSerializer(Optional = true)]
        public bool active { get; set; }
        [ContentSerializer(Optional = true)]
        public string tag { get; set; }

        private bool _isStatic = true;
        [ContentSerializer(Optional = true)]
        public bool isStatic { 
            get{return _isStatic;} 
            set
            {
                if (value == _isStatic) { return; }
                _isStatic = value;
                if (collider != null && rigidbody == null)
                {
                    collider.SetStatic(_isStatic);
                }
            }
        }

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

        protected AudioSource _audio;
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
        [ContentSerializer(ElementName = "cs", CollectionItemName = "c", Optional = true)]
        private List<Component> components { get; set; }

        internal override void AfterLoad(Dictionary<int, UnityObject> idMap)
        {
            base.AfterLoad(idMap);
            for (int j = 0; j < components.Count; j++)
            {
                components[j].isPrefab = isPrefab;
                components[j].AfterLoad(idMap);
                components[j].gameObject = this;
            }
        }

        public T AddComponent<T>() where T : Component
        {
            return (T)AddComponent(typeof(T));
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

        internal void RemoveComponent(Component component)
        {
            if (components.Remove(component))
            {
                component.gameObject = null;
            }
        }

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
        internal void OnTriggerEnter(Collider collider)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is MonoBehaviour)
                {
                    (components[i] as MonoBehaviour).OnTriggerEnter(collider);
                }
            }
        }

        internal void OnTriggerStay(Collider collider)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is MonoBehaviour)
                {
                    (components[i] as MonoBehaviour).OnTriggerStay(collider);
                }
            }
        }

        internal void OnTriggerExit(Collider collider)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is MonoBehaviour)
                {
                    (components[i] as MonoBehaviour).OnTriggerExit(collider);
                }
            }
        }

        internal void OnCollisionEnter(Collision collision)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is MonoBehaviour)
                {
                    (components[i] as MonoBehaviour).OnCollisionEnter(collision);
                }
            }
        }

        internal void OnCollisionStay(Collision collision)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is MonoBehaviour)
                {
                    (components[i] as MonoBehaviour).OnCollisionStay(collision);
                }
            }
        }

        internal void OnCollisionExit(Collision collision)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is MonoBehaviour)
                {
                    (components[i] as MonoBehaviour).OnCollisionExit(collision);
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
                if (type.IsAssignableFrom(components[i].GetType()))
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
                if (type.IsAssignableFrom(components[i].GetType()))
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
                if (type.IsAssignableFrom(components[i].GetType()))
                {
                    list.Add(components[i]);
                }
            }
            transform.GetComponentsInChildrenInt(type, list);
            return list.ToArray();
        }

        public T[] GetComponentsInChildren<T>() where T: Component
        {
            List<T> list = new List<T>();
            for (int i = 0; i < components.Count; i++)
            {
                T cmp = components[i] as T;
                if (cmp != null)
                {
                    list.Add(cmp);
                }
            }
            transform.GetComponentsInChildrenInt<T>(list);
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
                if (type.IsAssignableFrom(components[i].GetType()))
                {
                    return components[i];
                }
            }
            return null;
        }
        
        public T GetComponentInChildren<T>() where T : Component
        {
            return (T)GetComponentInChildren(typeof(T));
        }

        public T GetComponentInParents<T>() where T : Component
        {
            GameObject go = this;
            do
            {
                T comp = go.GetComponent<T>();
                if (comp != null)
                {
                    return comp;
                }
                if (go.transform.parent == null)
                {
                    go = null;
                }
                else
                {
                    go = go.transform.parent.gameObject;
                }
            } while (go != null);
            return null;
        }

        public T[] GetComponentsInParents<T>() where T : Component
        {
            List<T> list = new List<T>();
            GameObject go = this;
            while (go != null)
            {
                list.AddRange(go.GetComponents<T>());
                go = go.GetParent();
            }
            return list.ToArray();
        }

        private GameObject GetParent()
        {
            return (transform.parent != null) ? transform.parent.gameObject : null;
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
            return this.tag.Equals(tag, StringComparison.InvariantCultureIgnoreCase);
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
        /// Returns the first GameObject tagged tag. Returns null if no GameObject was found.
        /// Tags must be declared in the tag manager before using them.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static GameObject FindGameObjectWithTag(string p)
        {
            throw new NotImplementedException();
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

        protected override void Destroy()
        {
            for (int i = 0; i < components.Count; i++)
            {
                Destroy(components[i]);
            }
            base.Destroy();
        }

        public void SendMessageUpwards(string methodName)
        {
            SendMessageUpwards(methodName, null, SendMessageOptions.RequireReceiver);
        }

        public void SendMessageUpwards(string methodName, SendMessageOptions sendMessageOptions)
        {
            SendMessageUpwards(methodName, null, sendMessageOptions);
        }

        public void SendMessageUpwards(string methodName, object value, SendMessageOptions sendMessageOptions)
        {
            SendMessage(methodName, value, sendMessageOptions);
            if (transform.parent != null)
            {
                transform.parent.gameObject.SendMessageUpwards(methodName, value, sendMessageOptions);
            }
        }

        public void SendMessage(string methodName)
        {
            SendMessage(methodName, null, SendMessageOptions.RequireReceiver);
        }

        public void SendMessage(string methodName, SendMessageOptions sendMessageOptions)
        {
            SendMessage(methodName, null, sendMessageOptions);
        }

        public void SendMessage(string methodName, object value)
        {
            SendMessage(methodName, value, SendMessageOptions.RequireReceiver);
        }

        public void SendMessage(string methodName, object value, SendMessageOptions sendMessageOptions)
        {
            bool hadListener = false;
            for (int i = 0; i < components.Count; i++)
            {
                Component cmp = components[i];
                if (cmp is Transform)
                {
                    continue;
                }
                hadListener = cmp.SendMessage(methodName, value);
            }
            if (sendMessageOptions == SendMessageOptions.RequireReceiver && !hadListener)
            {
                Debug.Log("There were no listeners to the message " + methodName + " on " + this.ToString());
            }
        }

        public void BroadcastMessage(string methodName)
        {
            BroadcastMessage(methodName, null, SendMessageOptions.RequireReceiver);
        }

        public void BroadcastMessage(string methodName, object value)
        {
            BroadcastMessage(methodName, value, SendMessageOptions.RequireReceiver);
        }

        public void BroadcastMessage(string methodName, object value, SendMessageOptions sendMessageOptions)
        {
            SendMessage(methodName, value, sendMessageOptions);
            transform.BroadcastMessage(methodName, value, sendMessageOptions);
        }
        #endregion

        public override string ToString()
        {
            return name + "(" + GetInstanceID() + ")";
        }
    }
}
