using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using PressPlay.FFWD;

namespace PressPlay.FFWD.Components
{
    public class Rigidbody : Component
    {
        private float _mass = 1.0f;
        public float mass { 
            get
            {
                return _mass;
            }
            set
            {
                _mass = value;
                RescaleMass();
            }
        }

        private float _drag;
        public float drag {
            get 
            {
                if (body != null)
                {
                    _drag = body.LinearDamping; 
                    
                }

                return _drag; 
            }
            set 
            {
                _drag = value;
                if (body != null)
                {
                    body.LinearDamping = value;
                }
            } 
        }
        public float angularDrag { get; set; }
        [ContentSerializer(Optional = true)]
        public bool freezeRotation { get; set; }
        [ContentSerializer(ElementName = "isKinematic", Optional = true)]
        private bool _isKinematic = false;
        [ContentSerializerIgnore]
        public bool isKinematic
        { 
            get
            {
                return _isKinematic;
            }
            set
            {
                if (_isKinematic != value)
                {
                    _isKinematic = value;
                    if (_isKinematic)
                    {
                        Physics.RemoveRigidBody(body);
                        body.BodyType = BodyType.Kinematic;
                    }
                    else
                    {
                        Physics.AddRigidBody(body);
                        body.BodyType = BodyType.Dynamic;
                    }
                }
            }
        }

        private Body body;

        public override void Awake()
        {
            if (collider != null)
            {
                collider.CheckDropAxis();
                body = collider.connectedBody;
                if (body == null)
                {
                    body = Physics.AddBody();
                    body.Position = VectorConverter.Convert(transform.position, collider.to2dMode);
                    body.Rotation = MathHelper.ToRadians(VectorConverter.Angle(transform.rotation.eulerAngles, collider.to2dMode));
                    body.UserData = collider;
                    collider.AddCollider(body, mass);
                }
                body.BodyType = (isKinematic) ? BodyType.Kinematic : BodyType.Dynamic;
                body.Enabled = gameObject.active;
                body.LinearDamping = drag;
                body.AngularDamping = angularDrag;
                body.FixedRotation = freezeRotation;
                RescaleMass();
                if (!isKinematic)
                {
                    Physics.AddRigidBody(body);
                }
            }
            else
            {
#if DEBUG
                Debug.LogWarning("No collider set on this rigid body " + ToString());
#endif
            }
        }

        protected override void Destroy()
        {
            base.Destroy();
            if (body != null)
            {
                Physics.RemoveBody(body);
            }
        }

        private void RescaleMass()
        {
            if (body != null && body.Mass > 0)
            {
                float bodyMass = body.Mass;
                float massRatio = mass / bodyMass;
                for (int i = 0; i < body.FixtureList.Count; i++)
                {
                    Fixture f = body.FixtureList[i];
                    f.Shape.Density *= massRatio;
                }
                body.ResetMassData();
            }
        }

        [ContentSerializerIgnore]
        public Vector3 velocity
        {
            get
            {
                if (body == null)
                {
                    return Vector3.zero;
                }
                return VectorConverter.Convert(body.LinearVelocity, body.UserData.to2dMode);
            }
            set
            {
                if (body != null)
                {
                    body.LinearVelocity = VectorConverter.Convert(value, body.UserData.to2dMode);
                }
            }
        }

        public void AddForce(Vector3 elasticityForce)
        {
            AddForce(elasticityForce, ForceMode.Force);
        }

        public void AddForce(Vector3 elasticityForce, ForceMode mode)
        {
            Vector2 ef = VectorConverter.Convert(elasticityForce, body.UserData.to2dMode);
            switch (mode)
            {
                case ForceMode.Force:
                    body.ApplyForce(ef, VectorConverter.Convert(gameObject.transform.position, body.UserData.to2dMode));
                    break;
                case ForceMode.Acceleration:
                    throw new NotImplementedException();
                case ForceMode.Impulse:
                    body.ApplyLinearImpulse(ef, VectorConverter.Convert(gameObject.transform.position, body.UserData.to2dMode));
                    break;
                case ForceMode.VelocityChange:
                    throw new NotImplementedException();
            }
        }

        public void MovePosition(Vector3 position)
        {
            if (body != null)
            {
                Microsoft.Xna.Framework.Vector2 pos = VectorConverter.Convert(position, body.UserData.to2dMode);
                body.SetTransformIgnoreContacts(ref pos, body.Rotation);
                Physics.RemoveStays(collider);
            }
        }

        public Vector3 GetPointVelocity(Vector3 worldPoint)
        {
            return body.GetLinearVelocityFromWorldPoint(VectorConverter.Convert(worldPoint, collider.to2dMode));
        }
    }
}
