using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Components
{
    public class Rigidbody : Component
    {
        public float mass { get; set; }
        public float drag { get; set; }
        public float angularDrag { get; set; }
        public bool isKinematic { get; set; }
        public bool freezeRotation { get; set; }

        public override void Awake()
        {
            base.Awake();
        }

        [ContentSerializerIgnore]
        public float velocity { get; set; }

        public void AddForce(Vector3 elasticityForce)
        {
            
        }
    }
}
