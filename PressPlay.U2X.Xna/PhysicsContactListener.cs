using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;

namespace PressPlay.U2X.Xna
{
    internal class PhysicsContactListener : IContactListener
    {
        #region IContactListener Members
        public void BeginContact(Contact contact)
        {
        }

        public void EndContact(Contact contact)
        {
        }

        public void PreSolve(Contact contact, ref Manifold oldManifold)
        {
        }

        public void PostSolve(Contact contact, ref ContactImpulse impulse)
        {
        }
        #endregion
    }
}
