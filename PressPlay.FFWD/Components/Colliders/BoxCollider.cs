using Box2D.XNA;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.Interfaces;
using PressPlay.FFWD.Extensions;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD.Components
{
    public class BoxCollider : Collider
//#if DEBUG
//                , IRenderable
//#endif
    {
        #region Content properties
        public Vector3 center { get; set; }
        public Vector3 size { get; set; }
        #endregion

        protected override void AddCollider(Body body)
        {
            Vector2 sz = (size * gameObject.transform.lossyScale).To2d();
            Vector3 transCenter = Vector3.Transform(center, transform.world);
            Physics.AddBox(body, sz.X, sz.Y, transCenter.To2d(), transform.angleY, 1);
        }

//#if DEBUG
//        #region IRenderable Members
//        private BasicEffect effect;
//        private VertexPositionColor[] pointList;
//        public void Draw(SpriteBatch batch)
//        {
//            if (effect == null)
//            {
//                effect = new BasicEffect(batch.GraphicsDevice);
//                effect.VertexColorEnabled = true;
//            }
//            if (pointList == null)
//            {
//                // For drawing the original mesh
//                pointList = new VertexPositionColor[6];
//                Vector2 sz = size.To2d() / 2;
//                Color cl = Color.Purple;
//                pointList[0] = new VertexPositionColor(new Vector3(center.X - sz.X, 0, center.Z - sz.Y), cl);
//                pointList[1] = new VertexPositionColor(new Vector3(center.X + sz.X, 0, center.Z + sz.Y), cl);
//                pointList[2] = new VertexPositionColor(new Vector3(center.X - sz.X, 0, center.Z + sz.Y), cl);
//                pointList[3] = new VertexPositionColor(new Vector3(center.X - sz.X, 0, center.Z - sz.Y), cl);
//                pointList[4] = new VertexPositionColor(new Vector3(center.X + sz.X, 0, center.Z - sz.Y), cl);
//                pointList[5] = new VertexPositionColor(new Vector3(center.X + sz.X, 0, center.Z + sz.Y), cl);
//            }

//            effect.World = transform.world;
//            effect.View = Camera.main.View();
//            effect.Projection = Camera.main.projectionMatrix;

//            RasterizerState oldrasterizerState = batch.GraphicsDevice.RasterizerState;
//            RasterizerState rasterizerState = new RasterizerState();
//            rasterizerState.FillMode = FillMode.WireFrame;
//            rasterizerState.CullMode = CullMode.None;
//            batch.GraphicsDevice.RasterizerState = rasterizerState;

//            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
//            {
//                pass.Apply();
//                batch.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
//                    PrimitiveType.TriangleList,
//                    pointList,
//                    0,   // vertex buffer offset to add to each element of the index buffer
//                    2    // number of triangles to draw
//                );
//            }

//            batch.GraphicsDevice.RasterizerState = oldrasterizerState;
//        }
//        #endregion
//#endif
    }
}
