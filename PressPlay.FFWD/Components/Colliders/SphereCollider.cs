using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.Interfaces;
using PressPlay.FFWD;
using FarseerPhysics.Dynamics;

namespace PressPlay.FFWD.Components
{
    public class SphereCollider : Collider
    {
        #region Content properties
        public Vector3 center { get; set; }
        public float radius { get; set; }
        #endregion

        internal override void AddCollider(Body body, float mass)
        {
            float rad = radius * Math.Max(transform.lossyScale.x, Math.Max(transform.lossyScale.y, transform.lossyScale.z));
            connectedBody = Physics.AddCircle(body, isTrigger, rad, center * transform.lossyScale, -MathHelper.ToRadians(transform.rotation.eulerAngles.y), mass);
        }
    }
}
