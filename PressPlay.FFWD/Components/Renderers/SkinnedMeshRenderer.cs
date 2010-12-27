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
    public class SkinnedMeshRenderer : Renderer, Interfaces.IUpdateable
    {
        private SkinnedAnimationPlayer animationPlayer;

        public Mesh sharedMesh { get; set; }
        private ModelAnimationClip rootClip;
        private RootAnimationPlayer rootPlayer;
        private ModelAnimationClip clip;

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
            if (modelData != null)
            {
                if (modelData.RootAnimationClips != null && modelData.RootAnimationClips.ContainsKey("Take 001"))
                {
                    rootClip = modelData.RootAnimationClips["Take 001"];

                    rootPlayer = new RootAnimationPlayer();
                    rootPlayer.StartClip(rootClip, 1, TimeSpan.Zero);
                }
                if (modelData.ModelAnimationClips != null && modelData.ModelAnimationClips.ContainsKey("Take 001"))
                {
                    clip = modelData.ModelAnimationClips["Take 001"];
                    animationPlayer = new SkinnedAnimationPlayer(modelData.BindPose, modelData.InverseBindPose, modelData.SkeletonHierarchy);
                    animationPlayer.StartClip(clip, 1, TimeSpan.Zero);
                }
            }

        }

        #region IUpdateable Members
        public void Update()
        {
            if (rootPlayer != null)
            {
                rootPlayer.Update();
            }
            if (animationPlayer != null)
            {
                animationPlayer.Update();
            }
        }
        #endregion

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
                if (animationPlayer != null)
                {
                    boneTransforms = (animationPlayer as SkinnedAnimationPlayer).GetSkinTransforms();
                }

                Matrix rootTransform = Matrix.Identity;
                if (rootPlayer != null)
                {
                    rootTransform = rootPlayer.GetCurrentTransform();
                }

                SkinnedEffect sEffect = mesh.Effects[e] as SkinnedEffect;
                if (boneTransforms != null)
                {
                    sEffect.SetBoneTransforms(boneTransforms);
                }
                sEffect.World = Matrix.CreateScale(0.01f) * rootTransform * transform.world;
                sEffect.View = Camera.main.View();
                sEffect.Projection = Camera.main.projectionMatrix;
                sEffect.AmbientLightColor = new Vector3(1);
                if (material.texture != null)
                {
                    sEffect.Texture = material.texture;
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
