using PressPlay.FFWD.Components;

namespace PressPlay.FFWD
{
    public class Collision
    {
        public Vector3 relativeVelocity { get; internal set; }
        public Collider collider { get; internal set; }
        public ContactPoint[] contacts { get; set; }

        public Transform transform
        {
            get
            {
                return collider.transform;
            }
        }

        public Rigidbody rigidbody
        {
            get
            {
                return collider.rigidbody;
            }
        }

        public GameObject gameObject
        {
            get
            {
                return collider.gameObject;
            }
        }

        internal void SetColliders(Collider a, Collider b)
        {
            collider = b;
            for (int j = 0; j < contacts.Length; j++)
            {
                contacts[j].thisCollider = a;
                contacts[j].otherCollider = b;
            }
        }

        /*internal void ReverseNormals()
        {
            for (int j = 0; j < contacts.Length; j++)
            {
                contacts[j].normal = -contacts[j].normal;

            }
        }*/
    }
}
