using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.Components;
using PressPlay.FFWD.Interfaces;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Components
{
    public class SkinnedMeshRenderer : Renderer
    {

        public Mesh sharedMesh { get; set; }

        private Animation animation;

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

            // Create animation players/clips for the rigid model
            ModelData modelData = sharedMesh.model.Tag as ModelData;
            animation = GetComponentInParents<Animation>();
            if (modelData != null)
            {
                if (modelData.ModelAnimationClips != null)
                {
                    animation.Initialize(modelData);
                }
            }
        }

        #region IRenderable Members
        public override void Draw(SpriteBatch batch)
        {
            if (sharedMesh == null || sharedMesh.model == null)
            {
                return;
            }
            
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


            ModelMesh mesh = sharedMesh.GetModelMesh();
            for (int e = 0; e < mesh.Effects.Count; e++)
            {
                Matrix[] boneTransforms = null;
                if (animation != null)
                {
                    boneTransforms = animation.GetTransforms();
                }

                SkinnedEffect sEffect = mesh.Effects[e] as SkinnedEffect;
                if (sEffect != null)
                {
                    if (boneTransforms != null)
                    {
                        sEffect.SetBoneTransforms(boneTransforms);
                    }
                    sEffect.World = Matrix.CreateScale(0.01f) * transform.world;
                    sEffect.View = Camera.main.View();
                    sEffect.Projection = Camera.main.projectionMatrix;
                    sEffect.AmbientLightColor = new Vector3(1);
                    if (material.texture != null)
                    {
                        sEffect.Texture = material.texture;
                    }
                    mesh.Draw();
                }
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
