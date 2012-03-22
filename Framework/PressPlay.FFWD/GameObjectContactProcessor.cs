using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using PressPlay.FFWD.Components;
using PressPlay.FFWD.Interfaces;
using FarseerPhysics.Dynamics.Contacts;
using System.Collections;

namespace PressPlay.FFWD
{
    internal class GameObjectContactProcessor : IContactProcessor
    {
        private struct ColliderContact
        {
            public ColliderContact(Contact contact)
            {
                this.colliderA = contact.FixtureA.Body.UserData;
                this.colliderB = contact.FixtureB.Body.UserData;
                involvesATrigger = colliderA.isTrigger || colliderB.isTrigger;

                // Creates collisions
                Microsoft.Xna.Framework.Vector2 normal;
                FarseerPhysics.Common.FixedArray2<Microsoft.Xna.Framework.Vector2> points;
                contact.GetWorldManifold(out normal, out points);
                collisionBToA = new Collision()
                {
                    collider = colliderB,
                    relativeVelocity = ((colliderA.rigidbody != null) ? colliderA.rigidbody.velocity : Vector3.zero) - ((colliderB.rigidbody != null) ? colliderB.rigidbody.velocity : Vector3.zero),
                    contacts = new ContactPoint[contact.Manifold.PointCount]
                };
                collisionAToB = new Collision()
                {
                    collider = colliderA,
                    relativeVelocity = ((colliderB.rigidbody != null) ? colliderB.rigidbody.velocity : Vector3.zero) - ((colliderA.rigidbody != null) ? colliderA.rigidbody.velocity : Vector3.zero),
                    contacts = new ContactPoint[contact.Manifold.PointCount]
                };
                for (int j = 0; j < collisionBToA.contacts.Length; j++)
                {
                    collisionBToA.contacts[j].thisCollider = colliderB;
                    collisionBToA.contacts[j].otherCollider = colliderA;
                    collisionBToA.contacts[j].point = VectorConverter.Convert(points[j], colliderB.to2dMode);
                    collisionBToA.contacts[j].normal = VectorConverter.Convert(-normal, colliderB.to2dMode);

                    collisionAToB.contacts[j].thisCollider = colliderA;
                    collisionAToB.contacts[j].otherCollider = colliderB;
                    collisionAToB.contacts[j].point = VectorConverter.Convert(points[j], colliderA.to2dMode);
                    collisionAToB.contacts[j].normal = VectorConverter.Convert(normal, colliderA.to2dMode);
                }
            }

            public Collider colliderA;
            public Collider colliderB;
            public Collision collisionAToB;
            public Collision collisionBToA;
            private bool involvesATrigger;

            public void Enter()
            {
                if (involvesATrigger)
                {
                    colliderA.gameObject.OnTriggerEnter(colliderB);
                    colliderB.gameObject.OnTriggerEnter(colliderA);
                }
                else
                {
                    colliderA.gameObject.OnCollisionEnter(collisionBToA);
                    colliderB.gameObject.OnCollisionEnter(collisionAToB);
                }
            }

            public void Stay()
            {
                if (involvesATrigger)
                {
                    colliderA.gameObject.OnTriggerStay(colliderB);
                    colliderB.gameObject.OnTriggerStay(colliderA);
                }
                else
                {
                    colliderA.gameObject.OnCollisionStay(collisionBToA);
                    colliderB.gameObject.OnCollisionStay(collisionAToB);
                }
            }

            public void Exit()
            {
                if (involvesATrigger)
                {
                    colliderA.gameObject.OnTriggerExit(colliderB);
                    colliderB.gameObject.OnTriggerExit(colliderA);
                }
                else
                {
                    colliderA.gameObject.OnCollisionExit(collisionBToA);
                    colliderB.gameObject.OnCollisionExit(collisionAToB);
                }
            }
        }

        #region IContactListener Members
        private readonly Queue<ColliderContact> beginContacts = new Queue<ColliderContact>(ApplicationSettings.DefaultCapacities.ColliderContacts);
        private readonly Queue<ColliderContact> endContacts = new Queue<ColliderContact>(ApplicationSettings.DefaultCapacities.ColliderContacts);
        private readonly List<ColliderContact> staying = new List<ColliderContact>(ApplicationSettings.DefaultCapacities.ColliderContacts);
        private BitArray staysToRemove = new BitArray(64);
        public bool BeginContact(Contact contact)
        {
            // If this is a collision between static objects, just disable it.
            if (contact.FixtureA.Body.BodyType == BodyType.Static && contact.FixtureB.Body.BodyType == BodyType.Static)
            {
                return false;
            }
            if (!contact.FixtureA.IsSensor && !contact.FixtureA.IsSensor)
            {
                Rigidbody rigidbodyA = contact.FixtureA.Body.UserData.rigidbody;
                Rigidbody rigidbodyB = contact.FixtureB.Body.UserData.rigidbody;

                // If we have real colliders clashing, one of them must have a rigid body
                // I don't dare disabling it though...
                if ((rigidbodyA == null) && (rigidbodyB == null))
                {
                    return true;
                }
                // If both are rigid bodies - none of them must be kinematic
                if ((rigidbodyA != null) && (rigidbodyB != null) && rigidbodyA.isKinematic && rigidbodyB.isKinematic)
                {
                    return true;
                }
            }
            beginContacts.Enqueue(new ColliderContact(contact));
            return true;
        }
        public void EndContact(Contact contact)
        {
            if (contact.FixtureA.Body.UserData != null && contact.FixtureB.Body.UserData != null)
            {
                endContacts.Enqueue(new ColliderContact(contact));
            }
        }
        #endregion

        #region IContactProcessor Members
        public void Update()
        {
            while (endContacts.Count > 0)
            {
                ColliderContact contact = endContacts.Dequeue();
#if DEBUG
                if (DebugSettings.LogCollisions)
                {
                    Debug.Log(string.Format("Collision End: {0} <-> {1}", contact.colliderA, contact.colliderB));
                }
#endif
                RemoveStay(contact.colliderA, contact.colliderB);
                contact.Exit();
            }

            for (int i = staying.Count - 1; i >= 0; i--)
            {
                ColliderContact contact = staying[i];
                if (staysToRemove[i] || contact.colliderA.gameObject == null || contact.colliderB.gameObject == null || !contact.colliderA.gameObject.active || !contact.colliderB.gameObject.active)
                {
                    staysToRemove[i] = false;
                    staying.RemoveAt(i);
                    continue;
                }
                else
                {
                    contact.Stay();
                }
            }

            while (beginContacts.Count > 0)
            {
                ColliderContact contact = beginContacts.Dequeue();

#if DEBUG
                if (DebugSettings.LogCollisions)
                {
                    Debug.Log(string.Format("Collision Begin: {0} <-> {1}", contact.colliderA, contact.colliderB));
                }
#endif

                staying.Add(contact);
                contact.Enter();
            }
        }

        internal void RemoveStay(Collider compA, Collider compB)
        {
            for (int i = staying.Count - 1; i >= 0; i--)
            {
                if ((staying[i].colliderA == compA && staying[i].colliderB == compB) || (staying[i].colliderA == compB && staying[i].colliderB == compA))
                {
                    staysToRemove[i] = true;
                }
            }
        }

        public void ResetStays(Collider collider)
        {
            for (int i = staying.Count - 1; i >= 0; i--)
            {
                if (staying[i].colliderA == collider || staying[i].colliderB == collider)
                {
                    staysToRemove[i] = true;
                }
            }    
        }
        #endregion
    }
}
