using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PressPlay.U2X.Xna.Interfaces;
using Box2D.XNA;

namespace PressPlay.U2X.Xna.Components
{
    public class MeshCollider : Component, IRenderable
    {
        #region ContentProperties
        public string Material { get; set; }
        public bool IsTrigger { get; set; }
        public short[] Triangles { get; set; }
        public Vector3[] Vertices { get; set; }
//        public Vector3[] Normals { get; set; }
        #endregion

        #region Debug drawing
        private BasicEffect effect;
        private VertexPositionColor[] pointList;
        #endregion

        public override void Awake()
        {
            // For drawing the original mesh
            pointList = new VertexPositionColor[Vertices.Length];
            for (int i = 0; i < Vertices.Length; i++)
            {
                pointList[i] = new VertexPositionColor(Vertices[i], Color.LawnGreen);
            }

            List<Vector2[]> tris = new List<Vector2[]>();
            for (int i = 0; i < Triangles.Length; i+=3)
            {
                if (Vertices[Triangles[i]].Y + Vertices[Triangles[i + 1]].Y + Vertices[Triangles[i + 2]].Y > 1)
                {
                    Debug.Log(" Warning: " + ToString() + " has non zero Y in collider");
                }
                Vector2[] tri = new Vector2[] { 
                    new Vector2(Vertices[Triangles[i]].X, Vertices[Triangles[i]].Z),
                    new Vector2(Vertices[Triangles[i + 2]].X, Vertices[Triangles[i + 2]].Z),
                    new Vector2(Vertices[Triangles[i + 1]].X, Vertices[Triangles[i + 1]].Z)
                };
                tris.Add(tri);
            }
            BodyDef def = new BodyDef() { position = new Vector2(transform.position.X, transform.position.Z), angle = transform.angleY, userData = this };
            Body bd = Physics.AddMesh(tris, 1, def);

            bd.SetUserData(this);
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
            }

            batch.GraphicsDevice.RasterizerState = oldrasterizerState;
        }
        #endregion
    }
}
