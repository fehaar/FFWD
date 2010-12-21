using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD.Components
{
    public class MeshFilter : Component
    {
        [ContentSerializer(ElementName="mesh", Optional=true)]
        public Mesh sharedMesh { get; set; }

        private BasicEffect effect;

        public override void Awake()
        {
            base.Awake();            
            if (sharedMesh != null)
            {
                sharedMesh.Awake();
            }
        }

        public override void Start()
        {
            base.Start();
            if (sharedMesh != null)
            {
                sharedMesh.Start();
            }
        }

        internal bool CanDraw()
        {
            return (sharedMesh != null && sharedMesh.model == null);
        }

        public ModelMesh GetModelMesh()
        {
            if (sharedMesh != null)
            {
                return sharedMesh.GetModelMesh();
            }
            return null;
        }

        internal void Draw(SpriteBatch batch, Material[] materials)
        {
            if (sharedMesh.vertices == null)
            {
                return;
            }
            if (effect == null)
            {
                effect = new BasicEffect(batch.GraphicsDevice);
            }

            effect.World = transform.world;
            effect.View = Camera.main.View();
            effect.Projection = Camera.main.projectionMatrix;
            if (materials != null && materials.Length > 0 && materials[0].texture != null)
            {
                effect.TextureEnabled = true;
                effect.Texture = materials[0].texture;
            }
            effect.VertexColorEnabled = false;
            effect.Alpha = 1.0f;

            RasterizerState oldrasterizerState = batch.GraphicsDevice.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            batch.GraphicsDevice.RasterizerState = rasterizerState;

            BlendState oldBlend = batch.GraphicsDevice.BlendState;
            batch.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            VertexPositionNormalTexture[] data = new VertexPositionNormalTexture[sharedMesh.vertices.Length];
            for (int i = 0; i < sharedMesh.vertices.Length; i++)
            {
                data[i] = new VertexPositionNormalTexture()
                {
                    Position = sharedMesh.vertices[i],
                    Normal = sharedMesh.normals[i],
                    TextureCoordinate = sharedMesh.uv[i]
                };
            }

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                batch.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                    data,
                    0,
                    data.Length,
                    sharedMesh.triangles,
                    0,
                    sharedMesh.triangles.Length / 3
                );
            }

            batch.GraphicsDevice.RasterizerState = oldrasterizerState;
            batch.GraphicsDevice.BlendState = oldBlend;
        }
    }
}
