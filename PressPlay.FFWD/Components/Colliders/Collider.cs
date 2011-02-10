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
        #endregion

        [ContentSerializerIgnore]
        public Body connectedBody;

        protected Vector3 lastResizeScale;

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
                def.type = (gameObject.isStatic) ? BodyType.Static : BodyType.Kinematic;
                Body body = Physics.AddBody(def);
                AddCollider(body, 1);
                body.Rotation = -MathHelper.ToRadians(transform.rotation.eulerAngles.y);
            }
        }

        internal void SetStatic(bool isStatic)
        {
            if (connectedBody != null)
            {
                connectedBody.SetType((isStatic) ? BodyType.Static : BodyType.Kinematic);
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

        internal void MovePosition(Vector3 position)
        {
            if (connectedBody != null && connectedBody.GetType() != BodyType.Static)
            {
                //connectedBody.SetTransform(position, connectedBody.GetAngle());
                connectedBody.SetTransformIgnoreContacts(position, connectedBody.GetAngle());
                Physics.RemoveStays(this);
            }
        }

        public bool Raycast(Ray ray, out RaycastHit hitInfo, float distance)
        {
            //TODO: optimize this to only raycast to the relevant body. The current implementation is VERY VERY SLOW
            RaycastHit[] hits = Physics.RaycastAll(ray.origin,ray.direction,distance);

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider == this)
                {
                    hitInfo = hits[i];
                    return true;
                }
            }

            hitInfo = new RaycastHit();
            return false;
        }
    }
}
