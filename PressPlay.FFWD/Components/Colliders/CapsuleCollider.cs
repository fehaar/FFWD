using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.Interfaces;
using PressPlay.FFWD;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PressPlay.FFWD.Components
{
    public class CapsuleCollider : Collider
    {
        #region Content properties
        public Vector3 center;
        public float radius;
        public float height;
        public int direction;
        #endregion

        protected override void DoAddCollider(Body body, float mass)
        {
            connectedBody = body;
            Vector2 cen = VectorConverter.Convert(center * transform.lossyScale, to2dMode);

            if (direction == 0 || height <= radius * 2)
	        {
                float rad = radius * transform.lossyScale.z;
                Physics.AddCircle(body, isTrigger, rad, cen, mass);
	        }
            else
	        {
                if (direction == 1)
	            {
                    Vector2 sz = new Vector2(height - radius * 2, radius * 2);

                    sz *= (Vector2)gameObject.transform.lossyScale;
                    Physics.AddBox(body, isTrigger, sz.x, sz.y, cen, mass);
                    sz /= 2;
                    float rad = sz.y;
                    sz.y = 0;
                    Physics.AddCircle(body, isTrigger, rad, cen + sz, mass);
                    Physics.AddCircle(body, isTrigger, rad, cen - sz, mass);
                }
                else
	            {
                    Vector2 sz = new Vector2(radius * 2, height - radius * 2);
                    sz *= (Vector2)gameObject.transform.lossyScale;
                    Physics.AddBox(body, isTrigger, sz.x, sz.y, cen, mass);
                    sz /= 2;
                    float rad = sz.x;
                    sz.x = 0;
                    Physics.AddCircle(body, isTrigger, rad, cen + sz, mass);
                    Physics.AddCircle(body, isTrigger, rad, cen - sz, mass);
                }
	        }
        }
    }
}
