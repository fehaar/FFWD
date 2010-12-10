using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;

namespace PressPlay.FFWD.Components
{
    public abstract class Collider : Component
    {
        #region ContentProperties
        public bool isTrigger { get; set; }
        public string material { get; set; }
        #endregion

        public override void Awake()
        {
            Body body = Physics.AddBody();
            AddCollider(body);
            if (isTrigger)
            {
                body.SetType(BodyType.Kinematic);
            }
            body.SetUserData(this);
        }

        protected virtual BodyDef GetBodyDefinition()
        {
            return new BodyDef();
        }

        protected abstract void AddCollider(Body body);
    }
}
