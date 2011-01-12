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
            public Stay(Collider a, Collider b)
            {
                collA = a;
                collB = b;
                collision = null;
            }

            public Stay(Collider a, Collider b, Collision coll)
            {
                collA = a;
                collB = b;
                collision = coll;
            }

            public Collider collA;
            public Collider collB;
            public Collision collision;
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
                if (staying[i].collA.gameObject == null || !staying[i].collA.gameObject.active || !staying[i].collB.gameObject.active)
                {
                    staying.RemoveAt(i);
                }
                else if (staying[i].collision == null)
                {
                    staying[i].collA.gameObject.OnTriggerStay(staying[i].collB);
                    staying[i].collB.gameObject.OnTriggerStay(staying[i].collA);
                }
                else
                {
                    Collision coll = staying[i].collision;
                    coll.SetColliders(staying[i].collA, staying[i].collB);
                    staying[i].collA.gameObject.OnCollisionStay(coll);
                    coll.SetColliders(staying[i].collB, staying[i].collA);
                    staying[i].collB.gameObject.OnCollisionStay(coll);
                }
            }

            for (int i = 0; i < beginContacts.Count; ++i)
            {
                Contact contact = beginContacts[i];
                Fixture fixtureA = contact.GetFixtureA();
                Fixture fixtureB = contact.GetFixtureB();
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
                    staying.Add(new Stay(compA.collider, compB.collider));
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
                    staying.Add(new Stay(compA.collider, compB.collider, coll));
                    compA.gameObject.OnCollisionEnter(coll);
                    coll.SetColliders(compB.collider, compA.collider);
                    compB.gameObject.OnCollisionEnter(coll);
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
                if (staying[i].collA == compA && staying[i].collB == compB)
                {
                    staying.RemoveAt(i);
                }
            }
        }

        public void ResetStays(Collider collider)
        {
            for (int i = staying.Count - 1; i >= 0; i--)
            {
                if (staying[i].collA == collider || staying[i].collB == collider)
                {
                    staying.RemoveAt(i);
                }
            }    
        }
        #endregion
    }
}
