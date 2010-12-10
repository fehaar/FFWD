using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.Interfaces;
using PressPlay.FFWD.Extensions;
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
            return new BodyDef() { position = transform.position.To2d() };
        }

        internal override void AddCollider(Body body)
        {
            float rad = radius * Math.Max(transform.lossyScale.X, Math.Max(transform.lossyScale.Y, transform.lossyScale.Z));
            Vector3 transCenter = Vector3.Transform(center, transform.world);
            Physics.AddCircle(body, rad, transCenter.To2d(), transform.angleY, 1);
        }
    }
}
