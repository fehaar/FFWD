using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;

namespace PressPlay.U2X.Xna.Interfaces
{
    public interface ICollidable
    {
        void OnTriggerEnter(Contact contact);
        void OnTriggerExit(Contact contact);
        void OnCollisionEnter(Contact contact);
        void OnCollisionExit(Contact contact);
        void OnPreSolve(Contact contact, Manifold manifold);
        void OnPostSolve(Contact contact, ContactImpulse contactImpulse);
    }
}
