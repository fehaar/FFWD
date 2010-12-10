using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Box2D.XNA;
using PressPlay.FFWD.Extensions;

namespace PressPlay.FFWD.Components
{
    public class Rigidbody : Component
    {
        public float mass { get; set; }
        public float drag { get; set; }
        public float angularDrag { get; set; }
        public bool isKinematic { get; set; }
        public bool freezeRotation { get; set; }

        private Body body;

        public override void Awake()
        {
            if (collider != null)
            {
                BodyDef def = collider.GetBodyDefinition();
                def.userData = this;
                def.type = (isKinematic) ? BodyType.Kinematic : BodyType.Dynamic;
                def.active = gameObject.active;
                def.linearDamping = drag;
                def.angularDamping = angularDrag;
                def.fixedRotation = freezeRotation;
                body = Physics.AddBody(def);
                collider.AddCollider(body, mass);
            }
            else
            {
                Debug.LogWarning("No collider set on this rigid body " + ToString());
            }
        }

        [ContentSerializerIgnore]
        public Vector2 velocity
        {
            get
            {
                return body.GetLinearVelocity();
            }
            set
            {
                body.SetLinearVelocity(value);
            }
        }

        public void AddForce(Vector3 elasticityForce, ForceMode mode = ForceMode.Force)
        {
            switch (mode)
            {
                case ForceMode.Force:
                    body.ApplyForce(elasticityForce.To2d(), gameObject.transform.position.To2d());
                    break;
                case ForceMode.Acceleration:
                    break;
                case ForceMode.Impulse:
                    body.ApplyLinearImpulse(elasticityForce.To2d(), gameObject.transform.position.To2d());
                    break;
                case ForceMode.VelocityChange:
                    break;
            }
        }
    }
}
