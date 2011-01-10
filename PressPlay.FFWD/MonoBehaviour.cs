using PressPlay.FFWD.Interfaces;

namespace PressPlay.FFWD.Components
{
    public class MonoBehaviour : Behaviour, IUpdateable, IFixedUpdateable
    {
        public virtual void Update()
        {
            // NOTE: Do not do anything here as the convention in Unity is not to call base as it is not a virtual method
        }

        public virtual void FixedUpdate()
        {
            // NOTE: Do not do anything here as the convention in Unity is not to call base as it is not a virtual method
        }

        public virtual void OnCollisionEnter(Collision collision)
        {
            // NOTE: Do not do anything here as the convention in Unity is not to call base as it is not a virtual method
        }

        public virtual void OnCollisionStay(Collision collision)
        {
            // NOTE: Do not do anything here as the convention in Unity is not to call base as it is not a virtual method
        }

        public virtual void OnCollisionExit(Collision collision)
        {
            // NOTE: Do not do anything here as the convention in Unity is not to call base as it is not a virtual method
        }

        public virtual void OnTriggerStay(Collider collider)
        {
            // NOTE: Do not do anything here as the convention in Unity is not to call base as it is not a virtual method
        }

        public virtual void OnTriggerEnter(Collider collider)
        {
            // NOTE: Do not do anything here as the convention in Unity is not to call base as it is not a virtual method
        }

        public virtual void OnTriggerExit(Collider collider)
        {
            // NOTE: Do not do anything here as the convention in Unity is not to call base as it is not a virtual method
        }
    }
}
