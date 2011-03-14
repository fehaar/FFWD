using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision;

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

        /// <summary>
        /// Do we allow the game to turn off the collider when the game object becomes inactive.
        /// Usually this is not needed, as it can be a costly affair. So only do it when nessecary.
        /// </summary>
        [ContentSerializerIgnore]
        public bool allowTurnOff = false;

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

                AABB aabb;
                connectedBody.FixtureList[0].GetAABB(out aabb, 0);
                return Physics.BoundsFromAABB(aabb, 10);
            }
        }

        public override void Awake()
        {
            if (rigidbody == null)
            {
                Body body = Physics.AddBody();
                body.Position = transform.position;
                body.Rotation = -MathHelper.ToRadians(transform.rotation.eulerAngles.y);
                body.UserData = this;
                body.BodyType = (gameObject.isStatic) ? BodyType.Static : BodyType.Kinematic;
                AddCollider(body, 1);
            }
        }

        internal void SetStatic(bool isStatic)
        {
            if (connectedBody != null)
            {
                connectedBody.BodyType = (isStatic) ? BodyType.Static : BodyType.Kinematic;
            }
        }

        internal abstract void AddCollider(Body body, float mass);

        internal void ResizeConnectedBody()
        {
            if (lastResizeScale == transform.lossyScale) { return; }

            for (int i = 0; i < connectedBody.FixtureList.Count; i++)
            {
                Fixture fixture = connectedBody.FixtureList[i];
                connectedBody.DestroyFixture(fixture);
            }
            
            AddCollider(connectedBody, connectedBody.Mass);
        }

        internal void MovePosition(Vector3 position)
        {
            if (connectedBody != null && connectedBody.BodyType != BodyType.Static)
            {
                //connectedBody.SetTransform(position, connectedBody.GetAngle());
                Microsoft.Xna.Framework.Vector2 pos = position;
                connectedBody.SetTransformIgnoreContacts(ref pos, connectedBody.Rotation);
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
