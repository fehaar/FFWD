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

        internal override void AddCollider(Body body, float mass)
        {
            lastResizeScale = transform.lossyScale;

            float rad = radius * Math.Max(transform.lossyScale.x, Math.Max(transform.lossyScale.y, transform.lossyScale.z));

            if (rad > 10000) 
            { 
                rad = 10000;
                lastResizeScale = new Vector3(float.NaN, float.NaN, float.NaN);
            } //HACK!!!! to test rescaling of meshes

            connectedBody = Physics.AddCircle(body, isTrigger, rad, center, -MathHelper.ToRadians(transform.rotation.eulerAngles.y), mass);
        }

        /*internal override void ResizeConnectedBody(Vector3 _scale)
        {
            if (lastResizeScale == transform.lossyScale) { return; }

            Fixture fixture = connectedBody.GetFixtureList();
            connectedBody.DestroyFixture(fixture);

            float rad = radius * Math.Max(transform.lossyScale.x, Math.Max(transform.lossyScale.y, transform.lossyScale.z));
            connectedBody = Physics.AddCircle(connectedBody, isTrigger, rad, center, -MathHelper.ToRadians(transform.rotation.eulerAngles.y), connectedBody._mass);
            lastResizeScale = transform.lossyScale;
        }*/
    }
}
