using System.Collections.Generic;
using Box2D.XNA;
using PressPlay.FFWD.Components;
using PressPlay.FFWD.Interfaces;

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
        private List<Contact> beginContacts = new List<Contact>();
        private List<Contact> endContacts = new List<Contact>();
        private List<Stay> staying = new List<Stay>();
        //private struct PreSolveContact { public Contact c; public Manifold m;}
        //private Dictionary<Contact, ContactImpulse> postSolveContacts = new Dictionary<Contact,ContactImpulse>();
        //private List<PreSolveContact> preSolveContacts = new List<PreSolveContact>();
        public void BeginContact(Contact contact)
        {
            beginContacts.Add(contact);
        }
        public void EndContact(Contact contact)
        {
            endContacts.Add(contact);
        }

        public void PreSolve(Contact contact, ref Manifold oldManifold)
        {
            //preSolveContacts.Add(new PreSolveContact() { c = contact, m = oldManifold });
        }
        public void PostSolve(Contact contact, ref ContactImpulse impulse)
        {
            //postSolveContacts.Add(contact, impulse);
        }
        #endregion

        #region IContactProcessor Members
        public void Update()
        {
            for (int i = 0; i < endContacts.Count; ++i)
            {
                Contact contact = endContacts[i];
                Fixture fixtureA = contact.GetFixtureA();
                Fixture fixtureB = contact.GetFixtureB();
                if (fixtureA == null || fixtureB == null)
                {
                    continue;
                }
                if (fixtureA.GetBody().GetType() == BodyType.Static && fixtureB.GetBody().GetType() == BodyType.Static)
                {
                    continue;
                }
                Component compA = fixtureA.GetBody().GetUserData() as Component;
                Component compB = fixtureB.GetBody().GetUserData() as Component;
                if (compA == null || compB == null)
                {
                    continue;
                }
                if (fixtureA.IsSensor() || fixtureB.IsSensor())
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
                    WorldManifold wManifold;
                    contact.GetWorldManifold(out wManifold);
                    Collision coll = new Collision()
                    {
                        collider = compB.collider,
                        relativeVelocity = ((compA.rigidbody != null) ? compA.rigidbody.velocity : Vector3.zero) - ((compB.rigidbody != null) ? compB.rigidbody.velocity : Vector3.zero),
                        contacts = new ContactPoint[contact._manifold._pointCount]
                    };
                    for (int j = 0; j < coll.contacts.Length; j++)
                    {
                        coll.contacts[j].thisCollider = compA.collider;
                        coll.contacts[j].otherCollider = compB.collider;
                        coll.contacts[j].point = wManifold._points[j];
                        coll.contacts[j].normal = wManifold._normal;
                    }
                    compA.gameObject.OnCollisionExit(coll);
                    coll.SetColliders(compB.collider, compA.collider);
                    compB.gameObject.OnCollisionExit(coll);
                }
            }


            for (int i = staying.Count - 1; i >= 0; i--)
            {
                if (staying[i].colliderToTriggerA.gameObject == null || staying[i].colliderToTriggerB.gameObject == null || !staying[i].colliderToTriggerA.gameObject.active || !staying[i].colliderToTriggerB.gameObject.active)
                {
                    staying.RemoveAt(i);
                }
                else if (!staying[i].collision)
                {
                    staying[i].gameObjectA.OnTriggerStay(staying[i].colliderToTriggerA);
                    staying[i].gameObjectB.OnTriggerStay(staying[i].colliderToTriggerB);
                }
                else
                {
                    //Collision coll = staying[i].collision;
                    //coll.SetColliders(staying[i].collA, staying[i].collB);
                    staying[i].gameObjectA.OnCollisionStay(staying[i].collisionBToA);
                    //coll.SetColliders(staying[i].collB, staying[i].collA);
                    staying[i].gameObjectB.OnCollisionStay(staying[i].collisionAToB);
                }
            }

            for (int i = 0; i < beginContacts.Count; ++i)
            {
                Contact contact = beginContacts[i];

                Fixture fixtureA = contact.GetFixtureA();
                Fixture fixtureB = contact.GetFixtureB();

                if (fixtureA == null || fixtureB == null)
                {
                    continue;
                }

                if (fixtureA.GetBody().GetType() == BodyType.Static && fixtureB.GetBody().GetType() == BodyType.Static)
                {
                    continue;
                }
                Component compA = fixtureA.GetBody().GetUserData() as Component;
                Component compB = fixtureB.GetBody().GetUserData() as Component;
                if (compA == null || compB == null)
                {
                    continue;
                }
                if (fixtureA.IsSensor() || fixtureB.IsSensor())
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
                    WorldManifold wManifold;
                    contact.GetWorldManifold(out wManifold);
                    Collision collisionBToA = new Collision()
                    {
                        collider = compB.collider,
                        relativeVelocity = ((compA.rigidbody != null) ? compA.rigidbody.velocity : Vector3.zero) - ((compB.rigidbody != null) ? compB.rigidbody.velocity : Vector3.zero),
                        contacts = new ContactPoint[contact._manifold._pointCount]
                    };
                    Collision collisionAToB = new Collision()
                    {
                        collider = compA.collider,
                        relativeVelocity = ((compB.rigidbody != null) ? compB.rigidbody.velocity : Vector3.zero) - ((compA.rigidbody != null) ? compA.rigidbody.velocity : Vector3.zero),
                        contacts = new ContactPoint[contact._manifold._pointCount]
                    };
                    for (int j = 0; j < collisionBToA.contacts.Length; j++)
                    {
                        collisionBToA.contacts[j].thisCollider = compB.collider;
                        collisionBToA.contacts[j].otherCollider = compA.collider;
                        collisionBToA.contacts[j].point = wManifold._points[j];
                        collisionBToA.contacts[j].normal = -wManifold._normal;

                        collisionAToB.contacts[j].thisCollider = compA.collider;
                        collisionAToB.contacts[j].otherCollider = compB.collider;
                        collisionAToB.contacts[j].point = wManifold._points[j];
                        collisionAToB.contacts[j].normal = wManifold._normal;
                    }
                    Stay s = new Stay(collisionAToB, collisionBToA, compA.gameObject, compB.gameObject);
                    staying.Add(s);
                    s.gameObjectA.OnCollisionEnter(s.collisionBToA);
                    //coll.SetColliders(staying[i].collB, staying[i].collA);
                    s.gameObjectB.OnCollisionEnter(s.collisionAToB);

                    //compA.gameObject.OnCollisionEnter(collisionBToA);
                    //compB.gameObject.OnCollisionEnter(collisionAToB);
                }
            }

            //for (int i = 0; i < preSolveContacts.Count; ++i)
            //{
            //    PreSolveContact contact = preSolveContacts[i];

            //    Fixture fixtureA = contact.c.GetFixtureA();
            //    if (fixtureA != null && fixtureA.GetUserData() is GameObject)
            //        (fixtureA.GetUserData() as GameObject).OnPreSolve(contact.c, contact.m);

            //    Fixture fixtureB = contact.c.GetFixtureB();
            //    if (fixtureB != null && fixtureB.GetUserData() is GameObject)
            //        (fixtureB.GetUserData() as GameObject).OnPreSolve(contact.c, contact.m);
            //}

            //foreach (var contact in postSolveContacts)
            //{
            //    Fixture fixtureA = contact.Key.GetFixtureA();
            //    if (fixtureA != null && fixtureA.GetUserData() is GameObject)
            //        (fixtureA.GetUserData() as GameObject).OnPostSolve(contact.Key, contact.Value);

            //    Fixture fixtureB = contact.Key.GetFixtureB();
            //    if (fixtureB != null && fixtureB.GetUserData() is GameObject)
            //        (fixtureB.GetUserData() as GameObject).OnPostSolve(contact.Key, contact.Value);
            //}

            beginContacts.Clear();
            endContacts.Clear();
            //postSolveContacts.Clear();
            //preSolveContacts.Clear();
        }

        private void RemoveStay(Collider compA, Collider compB)
        {
            for (int i = staying.Count - 1; i >= 0; i--)
            {
                if ((staying[i].colliderToTriggerA == compA && staying[i].colliderToTriggerB == compB) || (staying[i].colliderToTriggerA == compB && staying[i].colliderToTriggerB == compA))
                {
                    staying.RemoveAt(i);
                }
            }
        }

        public void ResetStays(Collider collider)
        {
            for (int i = staying.Count - 1; i >= 0; i--)
            {
                if (staying[i].colliderToTriggerA == collider || staying[i].colliderToTriggerB == collider)
                {
                    staying.RemoveAt(i);
                }
            }    
        }
        #endregion
    }
}
