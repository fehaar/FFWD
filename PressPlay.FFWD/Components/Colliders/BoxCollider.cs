using Box2D.XNA;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.Interfaces;
using PressPlay.FFWD.Extensions;

namespace PressPlay.FFWD.Components
{
    public class BoxCollider : Collider
#if DEBUG
                , IDebugRenderable
#endif
    {
        #region Content properties
        public Vector3 center { get; set; }
        public Vector3 size { get; set; }
        #endregion

        public override void Awake()
        {
            Vector2 sz = size.To2d() / 2;
            Body bd = Physics.AddBox(sz.X, sz.Y, transform.position.To2d(), transform.angleY, 1);
            if (isTrigger)
            {
                bd.SetType(BodyType.Kinematic);
            }
            bd.SetUserData(this);
        }

#if DEBUG
        #region IDebugRenderable Members
        public void DebugRender(DebugDraw drawer)
        {
            FixedArray8<Vector2> fa = new FixedArray8<Vector2>();
            Vector2 pos = transform.position.To2d();
            Vector2 cen = center.To2d();
            Vector2 sz = size.To2d() / 2;
            fa[0] = pos + cen + new Vector2(-sz.X, -sz.Y);
            fa[1] = pos + cen + new Vector2(sz.X, -sz.Y);
            fa[2] = pos + cen + new Vector2(sz.X, sz.Y);
            fa[3] = pos + cen + new Vector2(-sz.X, sz.Y);
            drawer.DrawPolygon(ref fa, 4, (isTrigger) ? Color.HotPink : Color.Green);
        }
        #endregion
#endif
    }
}
