using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework.Content;
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
        public string name
        {
            get
            {
                return (gameObject == null) ? GetType().Name : gameObject.name;
            }
            set
            {
                if (gameObject != null) gameObject.name = value;
            }
        }

        public string tag
        {
            get
            {
                return gameObject.tag;
            }
        }

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

        #region Behaviour methods
        public virtual void Awake()
        {
            // NOTE: Do not do anything here as the convention in Unity is not to call base as it is not a virtual method
        }

        public virtual void Start()
        {
            // NOTE: Do not do anything here as the convention in Unity is not to call base as it is not a virtual method
        }
        #endregion

        #region Public methods
        public bool CompareTag(string tag)
        {
            return gameObject.CompareTag(tag);
        }
        #endregion

        #region Internal methods
        internal override UnityObject Clone()
        {
            UnityObject obj = base.Clone();
            obj.isPrefab = false;
            Application.AddNewComponent(obj as Component);
            return obj;
        }

        internal override void FixReferences(Dictionary<int, UnityObject> idMap)
        {
            base.FixReferences(idMap);
            
            if (gameObject == null)
            {
                return;
            }

            // We find all fields only - not properties as they cannot be set as references in Unity
            FieldInfo[] memInfo = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < memInfo.Length; i++)
            {
                if (typeof(UnityObject).IsAssignableFrom(memInfo[i].FieldType))
                {
                    UnityObject val = (memInfo[i].GetValue(this) as UnityObject);
                    if (val == null || !val.isPrefab)
	                {
                        continue;
	                }
                    if (idMap.ContainsKey(val.GetInstanceID()))
                    {
                        if (val != idMap[val.GetInstanceID()])
                        {
                            memInfo[i].SetValue(this, idMap[val.GetInstanceID()]);
                        }
                    }
                }
                if (memInfo[i].FieldType.IsArray && typeof(UnityObject).IsAssignableFrom(memInfo[i].FieldType.GetElementType()))
                {
                    UnityObject[] arr = (memInfo[i].GetValue(this) as UnityObject[]);
                    for (int j = 0; j < arr.Length; j++)
                    {
                        if ((arr[j] != null) && (arr[j].isPrefab))
                        {
                            if (idMap.ContainsKey(arr[j].GetInstanceID()))
                            {
                                if (arr[j] != idMap[arr[j].GetInstanceID()])
                                {
                                    arr[j] = idMap[arr[j].GetInstanceID()];
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Component locator methods
        public Component GetComponent(Type type)
        {
            return gameObject.GetComponent(type);
        }

        public Component GetComponent(string type)
        {
            // TODO: Objects should be destroyed after Update but before Rendering
            throw new NotImplementedException("Method not implemented.");
        }

        public Component[] GetComponents(Type type)
        {
            return gameObject.GetComponents(type);
        }

        public Component GetComponentInChildren(Type type)
        {
            return gameObject.GetComponentInChildren(type);
        }

        public T GetComponentInChildren<T>() where T: Component
        {
            return gameObject.GetComponentInChildren<T>();
        }

        public T[] GetComponentsInChildren<T>() where T : Component
        {
            return gameObject.GetComponentsInChildren<T>();
        }

        #region Component locator methods
        public T GetComponent<T>() where T : Component
        {
            return gameObject.GetComponent<T>();
        }

        public T[] GetComponents<T>() where T : Component
        {
            return gameObject.GetComponents<T>();
        }

        public Component[] GetComponentsInChildren(Type type)
        {
            return gameObject.GetComponentsInChildren(type);
        }

        public T GetComponentInParents<T>() where T : Component
        {
            return gameObject.GetComponentInParents<T>();
        }

        public T[] GetComponentsInParents<T>() where T : Component
        {
            throw new NotImplementedException();
        }
        #endregion

        public Component[] GetComponentsInChildren(Type type, bool includeInactive)
        {
            // TODO: Objects should be destroyed after Update but before Rendering
            throw new NotImplementedException("Method not implemented.");
        }
        #endregion

        #region Overridden methods
        public override string ToString()
        {
            if (gameObject == null)
            {
                return GetType().Name + " (" + GetInstanceID() + ") on its own";
            }
            return GetType().Name + " (" + GetInstanceID() + ") on " + gameObject.name + " (" + gameObject.GetInstanceID() + ") active: "+gameObject.active;
        }
        #endregion

        internal bool SendMessage(string methodName, object value)
        {
            Type tp = this.GetType();
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.InvokeMethod;
            while (tp != typeof(Component))
            {
                MethodInfo info = tp.GetMethod(methodName, flags);
                if (info != null)
                {
                    info.Invoke(this, (value == null) ? null : new object[1] { value });
                    return true;
                }
                tp = tp.BaseType;
                flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod;
            }
            return false;
        }
    }
}
