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
    public class MeshRenderer : Component, IRenderable
    {
        #region Content properties
        [ContentSerializer(Optional=true)]
        public string texture;
        [ContentSerializer(Optional = true)]
        public string shader;
        [ContentSerializer(Optional = true)]
        public string asset;
        [ContentSerializer(Optional = true)]
        public string mesh;
        #endregion

        [ContentSerializerIgnore]
        public Model model;
        [ContentSerializerIgnore]
        public Texture2D tex;

        private int meshIndex = 0;

        public override void Awake()
        {
            base.Awake();
            ContentHelper.LoadModel(asset);
            ContentHelper.LoadTexture(texture);
        }

        public override void Start()
        {
            base.Start();
            model = ContentHelper.GetModel(asset);
            tex = ContentHelper.GetTexture(texture);

            if (model == null)
            {
                return;
            }
            for (int i = 0; i < model.Meshes.Count; i++)
            {
                if (model.Meshes[i].Name == mesh)
                {
                    meshIndex = i;
                    break;
                }
            }
        }

        #region IRenderable Members
        public void Draw(SpriteBatch batch)
        {
            if (model == null)
            {
                return;
            }
            
            Matrix world = transform.world;
            
            // Do we have negative scale - if so, switch culling
            RasterizerState oldRaster = batch.GraphicsDevice.RasterizerState;
            BlendState oldBlend = batch.GraphicsDevice.BlendState;
            SamplerState oldSample = batch.GraphicsDevice.SamplerStates[0];
            if (transform.lossyScale.X < 0 || transform.lossyScale.Y < 0 || transform.lossyScale.Z < 0)
            {
                batch.GraphicsDevice.RasterizerState = new RasterizerState() { FillMode = oldRaster.FillMode, CullMode = CullMode.CullClockwiseFace };
            }
            if (shader == "iPhone/Particles/Additive Culled")
            {
                batch.GraphicsDevice.BlendState = BlendState.Additive;
                batch.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            }

            // Draw the model.
            ModelMesh mesh = model.Meshes[meshIndex];
            for (int e = 0; e < mesh.Effects.Count; e++)
            {
                BasicEffect effect = mesh.Effects[e] as BasicEffect;
                effect.World = world;
                effect.View = Camera.main.View();
                effect.Projection = Camera.main.projectionMatrix;
                effect.LightingEnabled = false;
                effect.Texture = tex;
                effect.TextureEnabled = true;
                mesh.Draw();
            }

            if (transform.lossyScale.X < 0 || transform.lossyScale.Y < 0 || transform.lossyScale.Z < 0)
            {
                batch.GraphicsDevice.RasterizerState = oldRaster;
            }
            if (shader == "iPhone/Particles/Additive Culled")
            {
                batch.GraphicsDevice.BlendState = oldBlend;
                batch.GraphicsDevice.SamplerStates[0] = oldSample;
            }
        }
        #endregion
    }
}
