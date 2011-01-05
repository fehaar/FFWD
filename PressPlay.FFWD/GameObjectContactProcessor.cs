using System.Collections.Generic;
using Box2D.XNA;
using PressPlay.FFWD.Interfaces;

namespace PressPlay.FFWD
{
    internal class GameObjectContactProcessor : IContactProcessor
    {
        #region IContactListener Members
        private List<Contact> beginContacts = new List<Contact>();
        private List<Contact> endContacts = new List<Contact>();
        private struct PreSolveContact { public Contact c; public Manifold m;}
        private Dictionary<Contact, ContactImpulse> postSolveContacts = new Dictionary<Contact,ContactImpulse>();
        private List<PreSolveContact> preSolveContacts = new List<PreSolveContact>();
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
            preSolveContacts.Add(new PreSolveContact() { c = contact, m = oldManifold });
        }
        public void PostSolve(Contact contact, ref ContactImpulse impulse)
        {
            postSolveContacts.Add(contact, impulse);
        }
        #endregion

        #region IContactProcessor Members
        public void Update()
        {
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
                    compA.gameObject.OnCollisionEnter(coll);
                    coll.collider = compA.collider;
                    for (int j = 0; j < coll.contacts.Length; j++)
                    {
                        coll.contacts[j].thisCollider = compB.collider;
                        coll.contacts[j].otherCollider = compA.collider;
                    }
                    compB.gameObject.OnCollisionEnter(coll);
                }
            }

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
                    coll.collider = compA.collider;
                    for (int j = 0; j < coll.contacts.Length; j++)
                    {
                        coll.contacts[j].thisCollider = compB.collider;
                        coll.contacts[j].otherCollider = compA.collider;
                    }
                    compB.gameObject.OnCollisionExit(coll);
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
            postSolveContacts.Clear();
            preSolveContacts.Clear();
        }
        #endregion
    }
}
