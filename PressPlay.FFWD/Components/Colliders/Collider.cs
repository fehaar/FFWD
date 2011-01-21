using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Components
{
    public abstract class Collider : Component
    {
        #region ContentProperties
        public bool isTrigger;
        [ContentSerializer(Optional = true)]
        public string material;
        protected Body connectedBody;
        protected Vector3 lastResizeScale;
        #endregion

        public Bounds bounds
        {
            get
            {
                // TODO: TEST this!
                if (connectedBody == null) 
                {
                    return new Bounds(); 
                }

                return Physics.BoundsFromAABB(connectedBody._fixtureList._aabb, 10);
            }
        }

        public override void Awake()
        {
            if (rigidbody == null)
            {
                BodyDef def = GetBodyDefinition();
                def.userData = this;
                Body body = Physics.AddBody(def);
                AddCollider(body, 1);
                body.Rotation = -MathHelper.ToRadians(transform.rotation.eulerAngles.y);
            }
        }

        internal virtual BodyDef GetBodyDefinition()
        {
            return new BodyDef() { position = transform.position };
        }

        internal abstract void AddCollider(Body body, float mass);

        internal void ResizeConnectedBody()
        {
            if (lastResizeScale == transform.lossyScale) { return; }

            Fixture fixture = connectedBody.GetFixtureList();
            connectedBody.DestroyFixture(fixture);
            
            AddCollider(connectedBody, connectedBody._mass);
        }
    }
}
