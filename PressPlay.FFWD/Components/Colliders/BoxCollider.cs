using Box2D.XNA;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.Interfaces;
using PressPlay.FFWD;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD.Components
{
    public class BoxCollider : Collider
    {
        #region Content properties
        public Vector3 center { get; set; }
        public Vector3 size { get; set; }
        #endregion

        internal override void AddCollider(Body body, float mass)
        {
            Vector2 sz = (size * gameObject.transform.lossyScale);
            connectedBody = Physics.AddBox(body, isTrigger, sz.x, sz.y, center * gameObject.transform.lossyScale, 0, mass);
        }
    }
}
