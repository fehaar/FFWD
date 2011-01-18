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
            // NOTE: Do not make any code here. Typically base.Update() is NOT called in MonoScripts so this will not be called either!!!!!
        }

        public virtual void LateUpdate()
        {
            // NOTE: Do not make any code here. Typically base.Update() is NOT called in MonoScripts so this will not be called either!!!!!
        }


        public virtual void FixedUpdate()
        {
            // NOTE: Do not make any code here. Typically base.Update() is NOT called in MonoScripts so this will not be called either!!!!!
        }

        public virtual void OnCollisionEnter(Collision collision)
        {
            // NOTE: Do not make any code here. Typically base.Update() is NOT called in MonoScripts so this will not be called either!!!!!
        }

        public virtual void OnCollisionStay(Collision collision)
        {
            // NOTE: Do not make any code here. Typically base.Update() is NOT called in MonoScripts so this will not be called either!!!!!
        }

        public virtual void OnCollisionExit(Collision collision)
        {
            // NOTE: Do not make any code here. Typically base.Update() is NOT called in MonoScripts so this will not be called either!!!!!
        }

        public virtual void OnTriggerStay(Collider collider)
        {
            // NOTE: Do not make any code here. Typically base.Update() is NOT called in MonoScripts so this will not be called either!!!!!
        }

        public virtual void OnTriggerEnter(Collider collider)
        {
            // NOTE: Do not make any code here. Typically base.Update() is NOT called in MonoScripts so this will not be called either!!!!!
        }

        public virtual void OnTriggerExit(Collider collider)
        {
            // NOTE: Do not make any code here. Typically base.Update() is NOT called in MonoScripts so this will not be called either!!!!!
        }
        #endregion

        #region Sealed methods
        internal override sealed void SetNewId(Dictionary<int, UnityObject> idMap)
        {
            base.SetNewId(idMap);
        }

        internal override sealed void AfterLoad(Dictionary<int, UnityObject> idMap)
        {
            base.AfterLoad(idMap);
        }

        internal override sealed void FixReferences(Dictionary<int, UnityObject> idMap)
        {
            base.FixReferences(idMap);
        }
        #endregion

        #region Invoke
        private struct InvokeCall
        {
            public InvokeCall(string methodName, float time)
            {
                this.methodName = methodName;
                this.time = time;
                this.repeatRate = 0;
            }

            public InvokeCall(string methodName, float time, float repeatRate)
            {
                this.methodName = methodName;
                this.time = time;
                this.repeatRate = repeatRate;
            }

            internal string methodName;
            internal float time;
            internal float repeatRate;

            public bool Update(float deltaTime)
            {
                time -= deltaTime;
                if (time <= 0)
                {
                    return true;
                }
                return false;
            }
        }

        private List<InvokeCall> invokeCalls;

        internal void UpdateInvokeCalls()
        {
            if (invokeCalls == null)
            {
                return;
            }
            for (int i = invokeCalls.Count - 1; i >= 0; i--)
            {
                InvokeCall call = invokeCalls[i];
                if (call.Update(Time.deltaTime))
                {
                    SendMessage(invokeCalls[i].methodName, null);
                    invokeCalls.RemoveAt(i);
                }
                else
                {
                    invokeCalls[i] = call;
                }
            }
        }

        public void Invoke(string methodName, float time)
        {
            if (invokeCalls == null)
            {
                invokeCalls = new List<InvokeCall>();
            }
            invokeCalls.Add(new InvokeCall(methodName, time));
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
            if (invokeCalls == null)
            {
                return false;
            }
            for (int i = 0; i < invokeCalls.Count; i++)
            {
                if (invokeCalls[i].methodName == methodName)
                {
                    return true;
                }
            }
            return false;
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
