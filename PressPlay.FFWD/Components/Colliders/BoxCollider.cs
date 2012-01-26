using Microsoft.Xna.Framework;
using PressPlay.FFWD.Interfaces;
using PressPlay.FFWD;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Components
{
    public class BoxCollider : Collider
    {
        #region Content properties
        public Vector3 center { get; set; }
        public Vector3 size { get; set; }
        [ContentSerializerIgnore]
        public Vector3 extents
        {
            get
            {
                return size / 2;
            }
            set
            {
                size = value * 2;
            }
        }
        #endregion

        protected override void DoAddCollider(Body body, float mass)
        {
            Vector2 sz = (size * gameObject.transform.lossyScale).Convert(to2dMode, true);
            connectedBody = body;
            Physics.AddBox(body, isTrigger, sz.x, sz.y, center * transform.lossyScale, mass);
        }
    }
}
