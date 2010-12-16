using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.Interfaces;
using PressPlay.FFWD;
using Box2D.XNA;

namespace PressPlay.FFWD.Components
{
    public class SphereCollider : Collider
    {
        #region Content properties
        public Vector3 center { get; set; }
        public float radius { get; set; }
        #endregion

        internal override BodyDef GetBodyDefinition()
        {
            return new BodyDef() { position = transform.position };
        }

        internal override void AddCollider(Body body, float mass)
        {
            float rad = radius * Math.Max(transform.lossyScale.x, Math.Max(transform.lossyScale.y, transform.lossyScale.z));
            Vector3 transCenter = Microsoft.Xna.Framework.Vector3.Transform(center, transform.world);
            Physics.AddCircle(body, isTrigger, rad, transCenter, transform.angleY, mass);
        }
    }
}
