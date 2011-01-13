using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD
{
    public class UnityObject
    {
        public UnityObject()
        {
            _id = nextId++;
            isPrefab = false;
        }

        private static int nextId = 1;

        [ContentSerializer(ElementName="id")]
        private int _id;

        public int GetInstanceID()
        {
            return _id;
        }

        [ContentSerializer(ElementName = "isPrefab", Optional = true)]
        internal bool isPrefab;

        internal virtual void AfterLoad()
        {
            if (_id > nextId)
            {
                nextId = _id + 1;
            }
        }

        /// <summary>
        /// The object obj will be destroyed now or if a time is specified t seconds from now. 
        /// If obj is a Component it will remove the component from the GameObject and destroy it. 
        /// If obj is a GameObject it will destroy the GameObject, all its components and all transform children of the GameObject. 
        /// Actual object destruction is always delayed until after the current Update loop, but will always be done before rendering.
        /// </summary>
        /// <param name="obj"></param>
        public static void Destroy(UnityObject obj)
        {
            Destroy(obj, 0.0f);
        }

        public static void Destroy(UnityObject obj, float time)
        {
            obj.Destroy();
        }

        protected virtual void Destroy()
        {
            Application.markedForDestruction.Add(this);
        }

        /// <summary>
        /// Clones the object original, places it at position and sets the rotation to rotation, then returns the cloned object. 
        /// This is essentially the same as using duplicate command (cmd-d) in Unity and then moving the object to the given location. 
        /// If a game object, component or script instance is passed, Instantiate will clone the entire game object hierarchy, with all children cloned as well. 
        /// All game objects are activated.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public static UnityObject Instantiate(UnityObject original, Vector3 position, Quaternion rotation)
        {
            UnityObject obj = Instantiate(original);
            if (obj is GameObject)
            {
                (obj as GameObject).transform.localPosition = position;
                (obj as GameObject).transform.localRotation = rotation;
            }
            else if (obj is Component)
            {
                (obj as Component).transform.localPosition = position;
                (obj as Component).transform.localRotation = rotation;
            }
            return obj;
        }

        /// <summary>
        /// This is not used at the moment as we do not treat meshes in the same way as Unity and have no notion of shared vs. normal meshes.
        /// </summary>
        /// <param name="original">The original mesh</param>
        /// <returns></returns>
        public static Mesh Instantiate(Mesh original)
        {
            return original;
        }

        /// <summary>
        /// Clones the object original, places it at position and sets the rotation to rotation, then returns the cloned object. 
        /// This is essentially the same as using duplicate command (cmd-d) in Unity and then moving the object to the given location. 
        /// If a game object, component or script instance is passed, Instantiate will clone the entire game object hierarchy, with all children cloned as well. 
        /// All game objects are activated.
        /// </summary>
        /// <param name="original"></param>
        public static UnityObject Instantiate(UnityObject original)
        {
            if (original == null)
            {
                return null;
            }
            GameObject clone = null;
            if (original is Component)
            {
                clone = (original as Component).gameObject.Clone() as GameObject;
            }
            else if (original is GameObject)
            {
                GameObject toClone = (original as GameObject).transform.root.gameObject;
                clone = toClone.Clone() as GameObject;
            }
            // NOTE: It is very important that this is done at the end otherwise we cannot find the correct object to return.
            Dictionary<int, UnityObject> idMap = new Dictionary<int, UnityObject>();
            clone.SetNewId(idMap);
            clone.FixReferences(idMap);
            return idMap[original.GetInstanceID()];
        }

        internal virtual void SetNewId(Dictionary<int, UnityObject> idMap)
        {
            if (!isPrefab)
            {
                idMap[_id] = this;
                _id = nextId++;
            }
        }

        internal virtual void FixReferences(Dictionary<int, UnityObject> idMap)
        {
        }

        internal virtual UnityObject Clone()
        {
            UnityObject obj = MemberwiseClone() as UnityObject;
            return obj;
        }

        public static T[] FindObjectsOfType<T>() where T : UnityObject
        {
            return Application.FindObjectsOfType<T>();
        }

        public static UnityObject[] FindObjectsOfType(Type type)
        {
            return Application.FindObjectsOfType(type);
        }

        public static UnityObject FindObjectOfType(Type type)
        {
            return Application.FindObjectOfType(type);
        }

        public static implicit operator bool(UnityObject obj)
        {            
            if (obj == null)
            {
                return false;
            }
            return (Application.Find(obj.GetInstanceID()) != null);
        }

        public static void DontDestroyOnLoad(UnityObject target)
        {
            Application.DontDestroyOnLoad(target);
        }
    }
}
