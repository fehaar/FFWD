using System;
using PressPlay.FFWD.Interfaces;

namespace PressPlay.FFWD.Test.Core_framework
{
    internal class TestComponent : Component, IFixedUpdateable, IUpdateable
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

        public void LateUpdate()
        {

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
    }
}
