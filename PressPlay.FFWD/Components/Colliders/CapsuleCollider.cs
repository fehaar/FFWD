using Box2D.XNA;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.Interfaces;
using PressPlay.FFWD;
using Microsoft.Xna.Framework.Graphics;

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

        internal override void AddCollider(Body body, float mass)
        {
            Vector2 sz;
            switch (direction)
            {
                case 0:
                    sz = new Vector2(radius * 2, radius);
                    break;
                case 1:
                    sz = new Vector2(height, radius * 2);
                    break;
                case 2:
                    sz = new Vector2(radius * 2, height);
                    break;
                default:
                    return;
            }            
            sz *= (Vector2)gameObject.transform.lossyScale;
            connectedBody = Physics.AddBox(body, isTrigger, sz.x, sz.y, center, 0, mass);
        }
    }
}
