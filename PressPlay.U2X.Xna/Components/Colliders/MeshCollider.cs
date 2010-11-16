using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PressPlay.U2X.Xna.Interfaces;

namespace PressPlay.U2X.Xna.Components
{
    public class MeshCollider : Component, IRenderable
    {
        #region ContentProperties
        public string Material { get; set; }
        public bool IsTrigger { get; set; }
        public int[] Triangles { get; set; }
        public Vector3[] Vertices { get; set; }
        public Vector3[] Normals { get; set; }
        #endregion

        #region Debug drawing
        private BasicEffect effect;
        private VertexPositionColor[] pointList;
        private VertexPositionColor[] collisionLine;
        #endregion

        public override void Awake()
        {
            pointList = new VertexPositionColor[Vertices.Length];
            for (int i = 0; i < Vertices.Length; i++)
            {
                pointList[i] = new VertexPositionColor(Vertices[i], Color.White);
            }
            List<VertexPositionColor> colVerts = new List<VertexPositionColor>();
            for (int i = 0; i < Triangles.Length; i++)
            {
                if (Vertices[Triangles[i]].Y < 0)
                {
                    if (colVerts.Count > 0)
                    {
                        Vector3 dist = colVerts[colVerts.Count - 1].Position - Vertices[Triangles[i]];
                        if (dist.LengthSquared() < 0.001)
                        {
                            continue;
                        }
                    }
                    colVerts.Add(new VertexPositionColor(Vertices[Triangles[i]], Color.Red));
                }
            }
            collisionLine = colVerts.ToArray();
        }

        private void TestVerts(List<VertexPositionColor> colVerts, int vert1, int vert2)
        {
            Vector3 dir = Vertices[vert2] - Vertices[vert1];
            //if (dir.Y > 0.01)
            //{
                colVerts.Add(new VertexPositionColor(Vertices[vert1] + (dir / 2), Color.Red));
            //}
        }

        public override void Start()
        {
        }


        #region IRenderable Members
        public void Draw(SpriteBatch batch)
        {
            if (effect == null)
            {
                effect = new BasicEffect(batch.GraphicsDevice);
                effect.VertexColorEnabled = true;
            }

            effect.World = transform.world;
            effect.View = Camera.main.View();
            effect.Projection = Camera.main.projectionMatrix;

            RasterizerState oldrasterizerState = batch.GraphicsDevice.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.FillMode = FillMode.WireFrame;
            rasterizerState.CullMode = CullMode.None;
            batch.GraphicsDevice.RasterizerState = rasterizerState;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                batch.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleList,
                    pointList,
                    0,   // vertex buffer offset to add to each element of the index buffer
                    Vertices.Length,   // number of vertices to draw
                    Triangles,
                    0,   // first index element to read
                    Triangles.Length / 3    // number of primitives to draw
                );
                if (collisionLine.Length > 0)
                {
                    batch.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                        PrimitiveType.LineStrip,
                        collisionLine,
                        0,
                        collisionLine.Length - 1
                    );
                }
            }

            batch.GraphicsDevice.RasterizerState = oldrasterizerState;
        }
        #endregion
    }
}
