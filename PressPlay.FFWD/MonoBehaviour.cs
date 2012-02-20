using PressPlay.FFWD.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PressPlay.FFWD.Components
{
    public class MonoBehaviour : Behaviour, IUpdateable, IFixedUpdateable
    {
        #region Overridable methods
        public virtual void Update()
        {
            // NOTE: Do not make any code here. Typically base method is NOT called in MonoScripts so this will not be called either!!!!!
        }

        public virtual void LateUpdate()
        {
            // NOTE: Do not make any code here. Typically base method is NOT called in MonoScripts so this will not be called either!!!!!
        }

        public virtual void FixedUpdate()
        {
            // NOTE: Do not make any code here. Typically base method is NOT called in MonoScripts so this will not be called either!!!!!
        }

        public virtual void OnEnable()
        {
            // NOTE: Do not make any code here. Typically base method is NOT called in MonoScripts so this will not be called either!!!!!
        }

        public virtual void OnDisable()
        {
            // NOTE: Do not make any code here. Typically base method is NOT called in MonoScripts so this will not be called either!!!!!
        }

        public virtual void OnGUI()
        {
            // NOTE: Do not make any code here. Typically base method is NOT called in MonoScripts so this will not be called either!!!!!
        }

        public virtual void OnCollisionEnter(Collision collision)
        {
            // NOTE: Do not make any code here. Typically base method is NOT called in MonoScripts so this will not be called either!!!!!
        }

        public virtual void OnCollisionStay(Collision collision)
        {
            // NOTE: Do not make any code here. Typically base method is NOT called in MonoScripts so this will not be called either!!!!!
        }

        public virtual void OnCollisionExit(Collision collision)
        {
            // NOTE: Do not make any code here. Typically base method is NOT called in MonoScripts so this will not be called either!!!!!
        }

        public virtual void OnTriggerStay(Collider collider)
        {
            // NOTE: Do not make any code here. Typically base method is NOT called in MonoScripts so this will not be called either!!!!!
        }

        public virtual void OnTriggerEnter(Collider collider)
        {
            // NOTE: Do not make any code here. Typically base method is NOT called in MonoScripts so this will not be called either!!!!!
        }

        public virtual void OnTriggerExit(Collider collider)
        {
            // NOTE: Do not make any code here. Typically base method is NOT called in MonoScripts so this will not be called either!!!!!
        }
        #endregion

        #region Sealed methods
        internal override sealed void SetNewId(Dictionary<int, UnityObject> idMap)
        {
            base.SetNewId(idMap);
        }

        internal override sealed void AfterLoad(Dictionary<int, UnityObject> idMap, Queue<Component> comps)
        {
            base.AfterLoad(idMap, comps);
        }

        internal override sealed void FixReferences(Dictionary<int, UnityObject> idMap)
        {
            base.FixReferences(idMap);
        }

        protected override void Destroy()
        {
            OnDisable();
            OnDestroy();
            base.Destroy();
        }

        public virtual void OnDestroy()
        {
            // NOTE: Do not make any code here. Typically base method is NOT called in MonoScripts so this will not be called either!!!!!
        }
        #endregion

        #region Invoke
        public void Invoke(string methodName, float time)
        {
            Application.AddInvokeCall(this, methodName, time, 0);
        }

        public void InvokeRepeating(string methodName, float time, float repeatRate)
        {
            throw new NotImplementedException("Method not implemented");
        }

        public void CancelInvoke(string methodName)
        {
            throw new NotImplementedException("Method not implemented");
        }

        public bool IsInvoking(string methodName)
        {
            return Application.IsInvoking(this, methodName);
        }
        #endregion

        public void StartCoroutine(IEnumerator routine)
        {
            throw new NotImplementedException("Method not implemented.");
        }

        public void StopCoroutine(string methodName)
        {
            throw new NotImplementedException("Method not implemented.");
        }
    }
}
