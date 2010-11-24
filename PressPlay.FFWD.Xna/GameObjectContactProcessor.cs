using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
using PressPlay.U2X.Xna.Interfaces;

namespace PressPlay.U2X.Xna
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
                if (fixtureA != null && fixtureA.GetUserData() is GameObject)
                {
                    GameObject go = fixtureA.GetUserData() as GameObject;
                    bool otherIsStaticTrigger = fixtureB.IsSensor() && fixtureB.GetBody().GetType() == BodyType.Static;
                    if (fixtureA.IsSensor() && !otherIsStaticTrigger)
                    {
                        go.OnTriggerEnter(contact);
                    }
                    else if (!otherIsStaticTrigger)
                    {
                        go.OnCollisionEnter(contact);
                    }
                }

                // Fixture B may have been destroyed in fixture A's onCollision, so check how it's doing.
                fixtureB = contact.GetFixtureB();
                if (fixtureB != null && fixtureB.GetBody() != null && fixtureB.GetUserData() is GameObject)
                {
                    GameObject go = fixtureB.GetUserData() as GameObject;
                    bool otherIsStaticTrigger = fixtureA.IsSensor() && fixtureA.GetBody().GetType() == BodyType.Static;
                    if (fixtureB.IsSensor() && !otherIsStaticTrigger)
                    {
                        go.OnTriggerEnter(contact);
                    }
                    else if (!otherIsStaticTrigger)
                    {
                        go.OnCollisionEnter(contact);
                    }
                }
            }

            for (int i = 0; i < endContacts.Count; ++i)
            {
                Contact contact = endContacts[i];
                Fixture fixtureA = contact.GetFixtureA();
                Fixture fixtureB = contact.GetFixtureB();
                // The fixtures may not exist at this point if the object has been removed from the world in OnCollisionEnter...
                if (fixtureA != null && fixtureA.GetUserData() is GameObject)
                {
                    bool otherIsStaticTrigger = fixtureB.IsSensor() && fixtureB.GetBody().GetType() == BodyType.Static;
                    if (fixtureA.IsSensor() && !otherIsStaticTrigger)
                        (fixtureA.GetUserData() as GameObject).OnTriggerExit(contact);
                    else
                        (fixtureA.GetUserData() as GameObject).OnCollisionExit(contact);
                    fixtureA.GetBody().SetAwake(true);
                }

                fixtureB = contact.GetFixtureB();
                if (fixtureB != null && fixtureB.GetBody() != null && fixtureB.GetUserData() is GameObject)
                {
                    bool otherIsStaticTrigger = fixtureA.IsSensor() && fixtureA.GetBody().GetType() == BodyType.Static;
                    if (fixtureB.IsSensor() && !otherIsStaticTrigger)
                        (fixtureB.GetUserData() as GameObject).OnTriggerExit(contact);
                    else
                        (fixtureB.GetUserData() as GameObject).OnCollisionExit(contact);
                    fixtureB.GetBody().SetAwake(true);
                }
            }

            for (int i = 0; i < preSolveContacts.Count; ++i)
            {
                PreSolveContact contact = preSolveContacts[i];

                Fixture fixtureA = contact.c.GetFixtureA();
                if (fixtureA != null && fixtureA.GetUserData() is GameObject)
                    (fixtureA.GetUserData() as GameObject).OnPreSolve(contact.c, contact.m);

                Fixture fixtureB = contact.c.GetFixtureB();
                if (fixtureB != null && fixtureB.GetUserData() is GameObject)
                    (fixtureB.GetUserData() as GameObject).OnPreSolve(contact.c, contact.m);
            }

            foreach (var contact in postSolveContacts)
            {
                Fixture fixtureA = contact.Key.GetFixtureA();
                if (fixtureA != null && fixtureA.GetUserData() is GameObject)
                    (fixtureA.GetUserData() as GameObject).OnPostSolve(contact.Key, contact.Value);

                Fixture fixtureB = contact.Key.GetFixtureB();
                if (fixtureB != null && fixtureB.GetUserData() is GameObject)
                    (fixtureB.GetUserData() as GameObject).OnPostSolve(contact.Key, contact.Value);
            }

            beginContacts.Clear();
            endContacts.Clear();
            postSolveContacts.Clear();
            preSolveContacts.Clear();
        }
        #endregion
    }
}
