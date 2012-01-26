using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework.Content;
using PressPlay.FFWD.Components;
using PressPlay.FFWD.Attributes;
using System.Collections;
using System.Text;

namespace PressPlay.FFWD
{
    public abstract class Component : UnityObject
    {
        public Component()
            : base()
        {
            Application.AddNewComponent(this);
        }

        public GameObject gameObject { get; internal set; }

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
                if (gameObject == null)
                {
                    return null;
                }
                return gameObject.renderer;
            }
        }

        [ContentSerializerIgnore]
        public AudioSource audio
        {
            get
            {
                if (gameObject == null)
                {
                    return null;
                }
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

        [ContentSerializerIgnore]
        public Camera camera
        {
            get
            {
                if (gameObject == null)
                {
                    return null;
                }
                return gameObject.camera;
            }
        }

        [ContentSerializerIgnore]
        public Light light
        {
            get
            {
                if (gameObject == null)
                {
                    return null;
                }
                return gameObject.light;
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

        protected override void Destroy()
        {
            if (gameObject != null)
            {
                gameObject.RemoveComponent(this);
            }
            base.Destroy();
        }

        private Dictionary<string, List<FieldInfo>> membersToFix = new Dictionary<string, List<FieldInfo>>();

        private void DoFixReferences(object objectToFix, Dictionary<int, UnityObject> idMap)
        {
            List<FieldInfo> memInfo = GetMembersToFix(objectToFix.GetType());
            for (int i = 0; i < memInfo.Count; i++)
            {
                FieldInfo field = memInfo[i];
                if (typeof(UnityObject).IsAssignableFrom(memInfo[i].FieldType))
                {
                    UnityObject val = (field.GetValue(objectToFix) as UnityObject);
                    if (val == null)
                    {
                        continue;
                    }
                    if (idMap.ContainsKey(val.GetInstanceID()))
                    {
                        if (val != idMap[val.GetInstanceID()])
                        {
                            field.SetValue(objectToFix, idMap[val.GetInstanceID()]);
                        }
                    }
                }
                if (typeof(IList).IsAssignableFrom(field.FieldType))
                {
                    if (field.FieldType.HasElementType && !field.FieldType.GetElementType().IsSubclassOf(typeof(UnityObject)))
                    {
                        continue;
                    }
                    IList list = (memInfo[i].GetValue(objectToFix) as IList);
                    if (list != null)
                    {
                        IList newList;
                        if (list is Array)
                        {
                            newList = (IList)(list as Array).Clone();
                        }
                        else
	                    {
                            ConstructorInfo ctor = list.GetType().GetConstructor(new Type[] { typeof(int) });
                            newList = (IList)ctor.Invoke(new object[] { list.Count });
	                    }
                        for (int j = 0; j < list.Count; j++)
                        {
                            if (!(newList is Array))
                            {
                                newList.Add(list[j]);
                            }
                            if (list[j] is UnityObject)
                            {
                                if (idMap.ContainsKey((list[j] as UnityObject).GetInstanceID()))
                                {
                                    newList[j] = idMap[(list[j] as UnityObject).GetInstanceID()];
                                }
                            }
                        }
                        memInfo[i].SetValue(objectToFix, newList);
                    }
                }

                if (Application.typeCaps.HasCaps(memInfo[i].FieldType, TypeSet.TypeCapabilities.FixReferences))
                {
                    DoFixReferences(memInfo[i].GetValue(objectToFix), idMap);
                }
            }
        }

        private List<FieldInfo> GetMembersToFix(Type typeToFix)
        {
            if (membersToFix.ContainsKey(typeToFix.FullName))
            {
                return membersToFix[typeToFix.FullName];
            }

            // We find all fields only - not properties as they cannot be set as references in Unity
            // We find all public fields and non-public fields that have the ContentSerializer property
            List<FieldInfo> memInfo = new List<FieldInfo>(typeToFix.GetFields(BindingFlags.Public | BindingFlags.Instance));
            FieldInfo[] privates = typeToFix.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < privates.Length; i++)
            {
                if (privates[i].GetCustomAttributes(typeof(ContentSerializerAttribute), true).Length > 0)
                {
                    memInfo.Add(privates[i]);
                }
            }
            if (typeToFix.BaseType != null)
            {
                memInfo.AddRange(GetMembersToFix(typeToFix.BaseType));
            }
            membersToFix[typeToFix.FullName] = memInfo;
            return memInfo;
        }

        internal override void FixReferences(Dictionary<int, UnityObject> idMap)
        {
            base.FixReferences(idMap);
            
            if (gameObject == null)
            {
                return;
            }

            DoFixReferences(this, idMap);
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

        public T GetComponent<T>() where T : Component
        {
            if (gameObject == null)
            {
                return null;
            }
            return gameObject.GetComponent<T>();
        }

        public T[] GetComponents<T>() where T : Component
        {
            if (gameObject == null)
            {
                return new T[0];
            }
            return gameObject.GetComponents<T>();
        }

        public Component[] GetComponentsInChildren(Type type)
        {
            if (gameObject == null)
            {
                return new Component[0];
            }
            return gameObject.GetComponentsInChildren(type);
        }

        public T GetComponentInParents<T>() where T : Component
        {
            if (gameObject == null)
            {
                return null;
            }
            return gameObject.GetComponentInParents<T>();
        }

        public T[] GetComponentsInParents<T>() where T : Component
        {
            throw new NotImplementedException();
        }

        public Component[] GetComponentsInChildren(Type type, bool includeInactive)
        {
            throw new NotImplementedException("Method not implemented.");
        }
        #endregion

        #region Overridden methods
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} ({1}){2}", GetType().Name, GetInstanceID(), (isPrefab) ? "P" : "");
            if (gameObject == null)
            {
                sb.Append(" on its own.");
            }
            else
            {
                sb.AppendFormat(" on {0}", gameObject.ToString());
            }
            return sb.ToString();
        }
        #endregion

        internal bool SendMessage(string methodName, object value)
        {
            Type tp = this.GetType();
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.InvokeMethod;
            while (tp != typeof(Component))
            {
                MethodInfo info = tp.GetCachedMethod(methodName, flags);
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
