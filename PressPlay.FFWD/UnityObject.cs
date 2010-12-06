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
        [ContentSerializer(ElementName="id")]
        private int _id;

        public int GetInstanceID()
        {
            return _id;
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
        public static UnityObject Instantiate(UnityObject original)
        {
            // TODO : Add implementation of method
            throw new NotImplementedException("Method not implemented.");
        }

        public static UnityObject[] FindObjectsOfType(Type type)
        {
            List<UnityObject> list = new List<UnityObject>();
            if (Application.Instance.currentScene != null)
            {
                foreach (GameObject go in Application.Instance.currentScene.gameObjects)
                {
                    list.AddRange(go.GetComponentsInChildren(type));
                }
            }
            return list.ToArray();
        }

        public static UnityObject FindObjectOfType(Type type)
        {
            Component cmp = null;
            if (Application.Instance.currentScene != null)
            {
                foreach (GameObject go in Application.Instance.currentScene.gameObjects)
                {
                    cmp = go.GetComponentInChildren(type);
                    if (cmp != null)
                    {
                        return cmp;
                    }
                }
            }
            return null;
        }
    }
}
