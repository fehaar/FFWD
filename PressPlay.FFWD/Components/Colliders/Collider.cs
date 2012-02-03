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
        [ContentSerializer(Optional = true)]
        internal Physics.To2dMode to2dMode = Physics.To2dMode.DropY;
        #endregion

        [ContentSerializerIgnore]
        public Body connectedBody;


        private bool _allowTurnOff = false;
        /// <summary>
        /// Do we allow the game to turn off the collider when the game object becomes inactive.
        /// Usually this is not needed, as it can be a costly affair. So only do it when nessecary.
        /// </summary>
        [ContentSerializerIgnore]
        public bool allowTurnOff
        {
            get
            {
                return _allowTurnOff;
            }
            set
            {
                _allowTurnOff = value;
                if (_allowTurnOff)
                {
                    Physics.AddMovingBody(connectedBody);
                }
            }
        }

        protected Vector3 lastResizeScale;

        public Bounds bounds
        { 
            get
            {
                if (connectedBody == null)
                {
                    return new Bounds();
                }
                AABB aabb_full = new AABB();
                for (int i = 0; i < connectedBody.FixtureList.Count; i++)
                {
                    AABB aabb;
                    connectedBody.FixtureList[0].Shape.ComputeAABB(out aabb, ref connectedBody.Xf, 0);
                    aabb_full.Combine(ref aabb);
                }
                return Bounds.FromAABB(ref aabb_full, to2dMode, GetSize());
            }
        }

        public override void Awake()
        {
            if (rigidbody == null)
            {
                connectedBody = Physics.AddBody();
                connectedBody.Position = VectorConverter.Convert(transform.position, to2dMode);
                connectedBody.Rotation = -MathHelper.ToRadians(VectorConverter.Reduce(transform.rotation.eulerAngles, to2dMode));
                connectedBody.UserData = this;
                connectedBody.BodyType = (gameObject.isStatic) ? BodyType.Static : BodyType.Kinematic;
                AddCollider(connectedBody, 1);
            }
        }

        protected override void Destroy()
        {
            base.Destroy();
            if (connectedBody != null)
            {
                Physics.RemoveBody(connectedBody);
            }
        }

        internal void SetStatic(bool isStatic)
        {
            if (connectedBody != null)
            {
                connectedBody.BodyType = (isStatic) ? BodyType.Static : BodyType.Kinematic;
                if (!isStatic)
                {
                    Physics.AddMovingBody(connectedBody);
                }
            }
        }

        internal void AddCollider(Body body, float mass)
        {
            DoAddCollider(body, mass);
            if (body.BodyType != BodyType.Static)
            {
                if (rigidbody == null)
                {
                    Physics.AddMovingBody(body);
                }
                else
                {
                    Physics.AddRigidBody(body);
                }
            }
        }

        protected virtual Vector3 GetSize()
        {
            return transform.lossyScale;
        }

        protected abstract void DoAddCollider(Body body, float mass);

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
                Microsoft.Xna.Framework.Vector2 pos = VectorConverter.Convert(position, to2dMode);
                connectedBody.SetTransformIgnoreContacts(ref pos, connectedBody.Rotation);
                Physics.RemoveStays(this);
            }
        }

        public bool Raycast(Ray ray, out RaycastHit hitInfo, float distance)
        {
            bool result = Physics.Raycast(connectedBody, ray, out hitInfo, distance);
            hitInfo.collider = this;
            return result;
        }
    }
}
