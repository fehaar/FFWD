using System;
using Box2D.XNA;
using PressPlay.FFWD.Interfaces;

namespace PressPlay.FFWD.Test.Core_framework
{
    internal class TestComponent : Component, IFixedUpdateable, IUpdateable, ICollidable
    {
        internal Action onAwake { get; set; }
        internal Action onStart { get; set; }
        internal Action onFixedUpdate { get; set; }
        internal Action onUpdate { get; set; }
        internal Action onTriggerEnter { get; set; }
        internal Action onTriggerExit { get; set; }
        internal Action onCollisionEnter { get; set; }
        internal Action onCollisionExit { get; set; }
        internal Action onPreSolve { get; set; }
        internal Action onPostSolve { get; set; }
        internal string Tag { get; set; }

        public override void Awake()
        {
            if (onAwake != null)
            {
                onAwake();
            }
        }

        public override void Start()
        {
            if (onStart != null)
            {
                onStart();
            }
        }

        #region IUpdateable Members
        public void Update()
        {
            if (onUpdate != null)
            {
                onUpdate();
            }
        }
        #endregion

        #region IFixedUpdateable Members
        public void FixedUpdate()
        {
            if (onFixedUpdate != null)
            {
                onFixedUpdate();
            }
        }
        #endregion

        #region ICollidable Members

        public void OnTriggerEnter(Contact contact)
        {
            if (onTriggerEnter != null)
            {
                onTriggerEnter();
            }
        }

        public void OnTriggerExit(Box2D.XNA.Contact contact)
        {
            if (onTriggerExit != null)
            {
                onTriggerExit();
            }
        }

        public void OnCollisionEnter(Box2D.XNA.Contact contact)
        {
            if (onCollisionEnter != null)
            {
                onCollisionEnter();
            }
        }

        public void OnCollisionExit(Box2D.XNA.Contact contact)
        {
            if (onCollisionExit != null)
            {
                onCollisionExit();
            }
        }

        public void OnPreSolve(Box2D.XNA.Contact contact, Box2D.XNA.Manifold manifold)
        {
            if (onPreSolve != null)
            {
                onPreSolve();
            }
        }

        public void OnPostSolve(Box2D.XNA.Contact contact, Box2D.XNA.ContactImpulse contactImpulse)
        {
            if (onPostSolve != null)
            {
                onPostSolve();
            }
        }

        #endregion
    }
}
