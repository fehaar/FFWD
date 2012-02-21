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
        private struct Stay
        {
            public bool collision {
                get { return collisionAToB != null; }
            }

            public Stay(Collider colliderToTriggerA, Collider colliderToTriggerB, GameObject gameObjectA, GameObject gameObjectB)
            {
                this.colliderToTriggerA = colliderToTriggerA;
                this.colliderToTriggerB = colliderToTriggerB;
                this.gameObjectA = gameObjectA;
                this.gameObjectB = gameObjectB;
                collisionAToB = null;
                collisionBToA = null;
            }

            public Stay(Collision collisionAToB, Collision collisionBToA, GameObject gameObjectA, GameObject gameObjectB)
            {
                this.collisionAToB = collisionAToB;
                this.collisionBToA = collisionBToA;
                colliderToTriggerA = collisionAToB.collider;
                colliderToTriggerB = collisionBToA.collider;
                this.gameObjectA = gameObjectA;
                this.gameObjectB = gameObjectB;
            }

            public Collider colliderToTriggerA;
            public Collider colliderToTriggerB;
            public Collision collisionAToB;
            public Collision collisionBToA;
            public GameObject gameObjectA;
            public GameObject gameObjectB;
        }

        #region IContactListener Members
        private readonly List<Contact> beginContacts = new List<Contact>(50);
        private readonly List<Contact> endContacts = new List<Contact>(50);
        private readonly List<Stay> staying = new List<Stay>(50);
        private BitArray staysToRemove = new BitArray(64);
        public bool BeginContact(Contact contact)
        {
            beginContacts.Add(contact);
            return true;
        }
        public void EndContact(Contact contact)
        {
            endContacts.Add(contact);
        }
        #endregion

        #region IContactProcessor Members
        public void Update()
        {
            for (int i = 0; i < endContacts.Count; ++i)
            {
                Contact contact = endContacts[i];
                Fixture fixtureA = contact.FixtureA;
                Fixture fixtureB = contact.FixtureB;
                if (fixtureA == null || fixtureB == null)
                {
                    continue;
                }
                if (fixtureA.Body.BodyType == BodyType.Static && fixtureB.Body.BodyType == BodyType.Static)
                {
                    continue;
                }
                Component compA = fixtureA.Body.UserData;
                Component compB = fixtureB.Body.UserData;
                if (compA == null || compB == null)
                {
                    continue;
                }
                if (fixtureA.IsSensor || fixtureB.IsSensor)
                {
                    RemoveStay(compA.collider, compB.collider);
                    compA.gameObject.OnTriggerExit(compB.collider);
                    compB.gameObject.OnTriggerExit(compA.collider);
                }
                else
                {
                    if (compA.rigidbody == null && compB.rigidbody == null)
                    {
                        continue;
                    }
                    if (compA.rigidbody != null && compB.rigidbody != null && compA.rigidbody.isKinematic && compB.rigidbody.isKinematic)
                    {
                        continue;
                    }
                    RemoveStay(compA.collider, compB.collider);
                    Microsoft.Xna.Framework.Vector2 normal;
                    FarseerPhysics.Common.FixedArray2<Microsoft.Xna.Framework.Vector2> points;
                    contact.GetWorldManifold(out normal, out points);
                    Collision coll = new Collision()
                    {
                        collider = compB.collider,
                        relativeVelocity = ((compA.rigidbody != null) ? compA.rigidbody.velocity : Vector3.zero) - ((compB.rigidbody != null) ? compB.rigidbody.velocity : Vector3.zero),
                        contacts = new ContactPoint[2]
                    };
                    for (int j = 0; j < 2; j++)
                    {
                        coll.contacts[j].thisCollider = compA.collider;
                        coll.contacts[j].otherCollider = compB.collider;
                        coll.contacts[j].point = VectorConverter.Convert(points[j], compA.collider.to2dMode);
                        coll.contacts[j].normal = VectorConverter.Convert(normal, compA.collider.to2dMode);
                    }
                    compA.gameObject.OnCollisionExit(coll);
                    coll.SetColliders(compB.collider, compA.collider);
                    compB.gameObject.OnCollisionExit(coll);
                }
            }

            for (int i = staying.Count - 1; i >= 0; i--)
            {
                if (staysToRemove[i] || staying[i].colliderToTriggerA.gameObject == null || staying[i].colliderToTriggerB.gameObject == null || !staying[i].colliderToTriggerA.gameObject.active || !staying[i].colliderToTriggerB.gameObject.active)
                {
                    staysToRemove[i] = false;
                    staying.RemoveAt(i);
                    continue;
                }
                else if (!staying[i].collision)
                {
                    staying[i].gameObjectA.OnTriggerStay(staying[i].colliderToTriggerA);
                    staying[i].gameObjectB.OnTriggerStay(staying[i].colliderToTriggerB);
                }
                else
                {
                    staying[i].gameObjectA.OnCollisionStay(staying[i].collisionBToA);
                    staying[i].gameObjectB.OnCollisionStay(staying[i].collisionAToB);
                }
            }

            for (int i = 0; i < beginContacts.Count; ++i)
            {
                Contact contact = beginContacts[i];

                Fixture fixtureA = contact.FixtureA;
                Fixture fixtureB = contact.FixtureB;

                if (fixtureA == null || fixtureB == null)
                {
                    continue;
                }

#if DEBUG
                if (DebugSettings.LogCollisions)
                {
                    Debug.Log(string.Format("Collision Begin: {0} <-> {1}", fixtureA.Body, fixtureB.Body));
                }
#endif

                if (fixtureA.Body.BodyType == BodyType.Static && fixtureB.Body.BodyType == BodyType.Static)
                {
                    continue;
                }
                Component compA = fixtureA.Body.UserData;
                Component compB = fixtureB.Body.UserData;
                if (compA == null || compB == null)
                {
                    continue;
                }
                if (fixtureA.IsSensor || fixtureB.IsSensor)
                {
                    staying.Add(new Stay(compB.collider, compA.collider, compA.gameObject, compB.gameObject ));
                    compA.gameObject.OnTriggerEnter(compB.collider);
                    compB.gameObject.OnTriggerEnter(compA.collider);
                }
                else
                {
                    if (compA.rigidbody == null && compB.rigidbody == null)
                    {
                        continue;
                    }
                    if (compA.rigidbody != null && compB.rigidbody != null && compA.rigidbody.isKinematic && compB.rigidbody.isKinematic)
                    {
                        continue;
                    }
                    Microsoft.Xna.Framework.Vector2 normal;
                    FarseerPhysics.Common.FixedArray2<Microsoft.Xna.Framework.Vector2> points;
                    contact.GetWorldManifold(out normal, out points);
                    Collision collisionBToA = new Collision()
                    {
                        collider = compB.collider,
                        relativeVelocity = ((compA.rigidbody != null) ? compA.rigidbody.velocity : Vector3.zero) - ((compB.rigidbody != null) ? compB.rigidbody.velocity : Vector3.zero),
                        contacts = new ContactPoint[contact.Manifold.PointCount]
                    };
                    Collision collisionAToB = new Collision()
                    {
                        collider = compA.collider,
                        relativeVelocity = ((compB.rigidbody != null) ? compB.rigidbody.velocity : Vector3.zero) - ((compA.rigidbody != null) ? compA.rigidbody.velocity : Vector3.zero),
                        contacts = new ContactPoint[contact.Manifold.PointCount]
                    };
                    for (int j = 0; j < collisionBToA.contacts.Length; j++)
                    {
                        collisionBToA.contacts[j].thisCollider = compB.collider;
                        collisionBToA.contacts[j].otherCollider = compA.collider;
                        collisionBToA.contacts[j].point = VectorConverter.Convert(points[j], compB.collider.to2dMode);
                        collisionBToA.contacts[j].normal = VectorConverter.Convert(-normal, compB.collider.to2dMode);

                        collisionAToB.contacts[j].thisCollider = compA.collider;
                        collisionAToB.contacts[j].otherCollider = compB.collider;
                        collisionAToB.contacts[j].point = VectorConverter.Convert(points[j], compA.collider.to2dMode);
                        collisionAToB.contacts[j].normal = VectorConverter.Convert(normal, compA.collider.to2dMode);
                    }
                    Stay s = new Stay(collisionAToB, collisionBToA, compA.gameObject, compB.gameObject);
                    staying.Add(s);
                    s.gameObjectA.OnCollisionEnter(s.collisionBToA);
                    s.gameObjectB.OnCollisionEnter(s.collisionAToB);
                }
            }

            beginContacts.Clear();
            endContacts.Clear();
        }

        internal void RemoveStay(Collider compA, Collider compB)
        {
            for (int i = staying.Count - 1; i >= 0; i--)
            {
                if ((staying[i].colliderToTriggerA == compA && staying[i].colliderToTriggerB == compB) || (staying[i].colliderToTriggerA == compB && staying[i].colliderToTriggerB == compA))
                {
                    staysToRemove[i] = true;
                }
            }
        }

        public void ResetStays(Collider collider)
        {
            for (int i = staying.Count - 1; i >= 0; i--)
            {
                if (staying[i].colliderToTriggerA == collider || staying[i].colliderToTriggerB == collider)
                {
                    staysToRemove[i] = true;
                }
            }    
        }
        #endregion
    }
}
