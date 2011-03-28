using Microsoft.Xna.Framework;
using PressPlay.FFWD.Interfaces;
using PressPlay.FFWD;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;

namespace PressPlay.FFWD.Components
{
    public class BoxCollider : Collider
    {
        #region Content properties
        public Vector3 center { get; set; }
        public Vector3 size { get; set; }
        #endregion

        protected override void DoAddCollider(Body body, float mass)
        {
            Vector2 sz = (size * gameObject.transform.lossyScale);
            connectedBody = body;
            Physics.AddBox(body, isTrigger, sz.x, sz.y, center * gameObject.transform.lossyScale, mass);
        }
    }
}
