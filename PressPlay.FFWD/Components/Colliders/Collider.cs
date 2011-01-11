using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Components
{
    public abstract class Collider : Component
    {
        #region ContentProperties
        public bool isTrigger { get; set; }
        [ContentSerializer(Optional = true)]
        public string material { get; set; }
        #endregion

        public Bounds bounds
        {
            get
            {
                // TODO: Implement this!
                return new Bounds();
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
            }
        }

        internal virtual BodyDef GetBodyDefinition()
        {
            return new BodyDef() { position = transform.position };
        }

        internal abstract void AddCollider(Body body, float mass);
    }
}
