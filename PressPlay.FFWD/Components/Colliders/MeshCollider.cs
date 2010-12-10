using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.Interfaces;
using PressPlay.FFWD.Extensions;
using Box2D.XNA;

namespace PressPlay.FFWD.Components
{
    public class MeshCollider : Collider, IRenderable
    {
        #region ContentProperties
        public short[] triangles { get; set; }
        public Vector3[] vertices { get; set; }
        #endregion

        #region Debug drawing
        //private BasicEffect effect;
        private VertexPositionColor[] pointList;
        #endregion

        public override void Awake()
        {
            // For drawing the original mesh
            pointList = new VertexPositionColor[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                pointList[i] = new VertexPositionColor(vertices[i], Color.LawnGreen);
            }
        }

        protected override BodyDef GetBodyDefinition()
        {
            return new BodyDef() { position = transform.position.To2d(), angle = transform.angleY, userData = this };
        }

        protected override void AddCollider(Body body)
        {
            List<Vector2[]> tris = new List<Vector2[]>();
            for (int i = 0; i < triangles.Length; i += 3)
            {
                if (vertices[triangles[i]].Y + vertices[triangles[i + 1]].Y + vertices[triangles[i + 2]].Y > 1)
                {
                    Debug.Log(" Warning: " + ToString() + " has non zero Y in collider");
                }
                Vector2[] tri = new Vector2[] { 
                    new Vector2(vertices[triangles[i]].X, vertices[triangles[i]].Z),
                    new Vector2(vertices[triangles[i + 2]].X, vertices[triangles[i + 2]].Z),
                    new Vector2(vertices[triangles[i + 1]].X, vertices[triangles[i + 1]].Z)
                };
                tris.Add(tri);
            }
            Physics.AddMesh(body, tris, 1);
        }

        public void Select()
        {
            for (int i = 0; i < pointList.Length; i++)
            {
                pointList[i].Color = Color.Red;
            }
        }

        public override void Start()
        {
        }


        #region IRenderable Members
        public void Draw(SpriteBatch batch)
        {
            //if (effect == null)
            //{
            //    effect = new BasicEffect(batch.GraphicsDevice);
            //    effect.VertexColorEnabled = true;
            //}

            //effect.World = transform.world;
            //effect.View = Camera.main.View();
            //effect.Projection = Camera.main.projectionMatrix;

            //RasterizerState oldrasterizerState = batch.GraphicsDevice.RasterizerState;
            //RasterizerState rasterizerState = new RasterizerState();
            //rasterizerState.FillMode = FillMode.WireFrame;
            //rasterizerState.CullMode = CullMode.None;
            //batch.GraphicsDevice.RasterizerState = rasterizerState;

            //foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    batch.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
            //        PrimitiveType.TriangleList,
            //        pointList,
            //        0,   // vertex buffer offset to add to each element of the index buffer
            //        vertices.Length,   // number of vertices to draw
            //        triangles,
            //        0,   // first index element to read
            //        triangles.Length / 3    // number of primitives to draw
            //    );
            //}

            //batch.GraphicsDevice.RasterizerState = oldrasterizerState;
        }
        #endregion
    }
}
