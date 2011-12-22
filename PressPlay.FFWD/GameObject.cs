using System;
using System.Linq;
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
        }

        internal GameObject(bool isPrefab)
            : base()
        {
            this.isPrefab = isPrefab;
            components = new List<Component>();
            AddComponent(new Transform());
        }

        public GameObject(string name) : base()
        {
            this.name = name;
            components = new List<Component>();
            AddComponent(new Transform());
        }

        [ContentSerializer(Optional = true)]
        public string name;
        [ContentSerializer(Optional = true)]
        public int layer;

        [ContentSerializer(ElementName = "active", Optional = true)]
        private bool _active = true;
        [ContentSerializerIgnore]
        public bool active 
        { 
            get
            {
                return _active;
            }
            set
            {
                _active = value;
                Application.UpdateGameObjectActive(components);
            }
        }
        [ContentSerializer(Optional = true)]
        public string tag;

        [ContentSerializer(ElementName = "static", Optional = true)]
        private bool _isStatic = false;
        [ContentSerializerIgnore]
        public bool isStatic
        { 
            get {return _isStatic;} 
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

        [ContentSerializerIgnore]
        internal int ComponentCount
        {
            get
            {
                return components.Count;
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

        protected ParticleEmitter _particleEmitter;
        [ContentSerializerIgnore]
        public ParticleEmitter particleEmitter
        {
            get
            {
                if (_particleEmitter == null)
                {
                    _particleEmitter = GetComponent<ParticleEmitter>();
                }
                return _particleEmitter;
            }
        }

        private Camera _camera;
        [ContentSerializerIgnore]
        public Camera camera
        {
            get
            {
                if (_camera == null)
                {
                    _camera = GetComponent<Camera>();
                }
                return _camera;
            }
        }

        [ContentSerializerIgnore]
        public GameObject gameObject
        {
            get
            {
                return this;
            }
        }
        #endregion

        [ContentSerializer(ElementName = "cs", CollectionItemName = "c", Optional = true)]
        private List<Component> components;

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
            obj.active = true;
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
        private static List<Component> locatorList = new List<Component>(50);

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
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is T)
                {
                    locatorList.Add(components[i] as T);
                }
            }
            T[] arr = new T[locatorList.Count];
            for (int i = 0; i < locatorList.Count; i++)
            {
                arr[i] = (T)locatorList[i];
            }
            locatorList.Clear();
            return arr;
        }

        public Component[] GetComponents(Type type)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (type.IsAssignableFrom(components[i].GetType()))
                {
                    locatorList.Add(components[i]);
                }
            }
            Component[] arr = locatorList.ToArray();
            locatorList.Clear();
            return arr;
        }

        public Component[] GetComponentsInChildren(Type type)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (type.IsAssignableFrom(components[i].GetType()))
                {
                    locatorList.Add(components[i]);
                }
            }
            transform.GetComponentsInChildrenInt(type, locatorList);
            Component[] arr = locatorList.ToArray();
            locatorList.Clear();
            return arr;
        }

        internal void GetComponentsInChildren(Type type, List<Component> list)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (type.IsAssignableFrom(components[i].GetType()))
                {
                    list.Add(components[i]);
                }
            }
            transform.GetComponentsInChildrenInt(type, list);
        }

        public T[] GetComponentsInChildren<T>() where T: Component
        {
            for (int i = 0; i < components.Count; i++)
            {
                T cmp = components[i] as T;
                if (cmp != null)
                {
                    locatorList.Add(cmp);
                }
            }
            transform.GetComponentsInChildrenInt<T>(locatorList);

            T[] arr = new T[locatorList.Count];
            for (int i = 0; i < locatorList.Count; i++)
            {
                arr[i] = (T)locatorList[i];
            }
            locatorList.Clear();
            return arr;
        }

        internal void GetComponentsInChildren<T>(List<Component> list) where T : Component
        {
            for (int i = 0; i < components.Count; i++)
            {
                T cmp = components[i] as T;
                if (cmp != null)
                {
                    list.Add(cmp);
                }
            }
            transform.GetComponentsInChildrenInt<T>(list);
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
            GameObject go = this;
            while (go != null)
            {
                locatorList.AddRange(go.GetComponents<T>());
                go = go.GetParent();
            }
            T[] arr = new T[locatorList.Count];
            for (int i = 0; i < locatorList.Count; i++)
            {
                arr[i] = (T)locatorList[i];
            }
            locatorList.Clear();
            return arr;
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
            return FindGameObjectWithTag(tag);
        }

        /// <summary>
        /// Returns the first GameObject tagged tag. Returns null if no GameObject was found.
        /// Tags must be declared in the tag manager before using them.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static GameObject FindGameObjectWithTag(string tag)
        {
            return Application.FindByTag(tag).FirstOrDefault();
        }

        /// <summary>
        /// Returns a list of active GameObjects tagged tag. Returns null if no GameObject was found.
        /// Tags must be declared in the tag manager before using them.
        /// </summary>
        /// <param name="tag">The tag to find</param>
        /// <returns></returns>
        public static GameObject[] FindGameObjectsWithTag(string tag)
        {
            return Application.FindByTag(tag).ToArray();
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
            return Application.FindByName(name);
        }

        protected override void Destroy()
        {
            for (int i = components.Count - 1; i >= 0; i--)
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
#if DEBUG
            if (sendMessageOptions == SendMessageOptions.RequireReceiver && !hadListener)
            {
                Debug.Log("There were no listeners to the message " + methodName + " on " + this.ToString());
            }
#endif
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
            return String.Format("{0} ({1}){2}{3}", String.IsNullOrWhiteSpace(name) ? "?" : name, GetInstanceID(), (isPrefab) ? "P" : "", (active) ? "A" : "");
        }

        internal string FullName()
        {
            if (transform.parent == null)
            {
                return name;
            }
            else
            {
                return transform.parent.gameObject.FullName() + "/" + name;
            }
        }
    }
}
