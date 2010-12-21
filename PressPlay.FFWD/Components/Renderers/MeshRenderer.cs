using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.Animation;
using PressPlay.FFWD.Interfaces;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Components
{
    public class MeshRenderer : Renderer
    {
        private MeshFilter filter;

        public override void Start()
        {
            base.Start();
            filter = (MeshFilter)GetComponent(typeof(MeshFilter));
        }

        #region IRenderable Members
        public override void Draw(SpriteBatch batch)
        {
            if (filter == null)
            {                
                return;
            }
            if (filter.CanDraw())
            {
                filter.Draw(batch, materials);
                return;
            }
            
            Matrix world = transform.world;

            // Do we have negative scale - if so, switch culling
            RasterizerState oldRaster = batch.GraphicsDevice.RasterizerState;
            BlendState oldBlend = batch.GraphicsDevice.BlendState;
            SamplerState oldSample = batch.GraphicsDevice.SamplerStates[0];
            if (transform.lossyScale.x < 0 || transform.lossyScale.y < 0 || transform.lossyScale.z < 0)
            {
                batch.GraphicsDevice.RasterizerState = new RasterizerState() { FillMode = oldRaster.FillMode, CullMode = CullMode.CullClockwiseFace };
            }
            if (material.IsAdditive())
            {
                batch.GraphicsDevice.BlendState = BlendState.Additive;
                batch.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            }

            // Draw the model.
            ModelMesh mesh = filter.GetModelMesh();
            for (int e = 0; e < mesh.Effects.Count; e++)
            {
                BasicEffect effect = mesh.Effects[e] as BasicEffect;
                effect.World = world;
                effect.View = Camera.main.View();
                effect.Projection = Camera.main.projectionMatrix;
                effect.LightingEnabled = false;
                if (material.texture != null)
                {
                    effect.TextureEnabled = true;
                    effect.Texture = material.texture;
                }
                mesh.Draw();
            }

            if (transform.lossyScale.x < 0 || transform.lossyScale.y < 0 || transform.lossyScale.z < 0)
            {
                batch.GraphicsDevice.RasterizerState = oldRaster;
            }
            if (material.IsAdditive())
            {
                batch.GraphicsDevice.BlendState = oldBlend;
                batch.GraphicsDevice.SamplerStates[0] = oldSample;
            }
        }
        #endregion
    }
}
