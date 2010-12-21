using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

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
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");
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
                GameObject toClone = (original as GameObject);
                while (toClone.transform != null && toClone.transform.parent != null)
                {
                    toClone = toClone.transform.parent.gameObject;
                }
                clone = toClone.Clone() as GameObject;
            }
            UnityObject ret = clone.GetObjectById(original.GetInstanceID());
            // NOTE: It is very important that this is done at the end otherwise we cannot find the correct object to return.
            clone.SetNewId();
            return ret;
        }

        internal virtual void SetNewId()
        {
            _id = nextId++;
        }

        internal virtual UnityObject Clone()
        {
            UnityObject obj = MemberwiseClone() as UnityObject;
            return obj;
        }

        internal virtual UnityObject GetObjectById(int id)
        {
            if (_id == id)
            {
                return this;
            }
            return null;
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
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");
        }
    }
}
