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
        private Matrix[] boneTransforms;
        private AnimationPlayer animationPlayer;

        public Mesh sharedMesh { get; set; }

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

            boneTransforms = new Matrix[sharedMesh.model.Bones.Count];
            // Look up our custom skinning information.
            SkinningData skinningData = sharedMesh.model.Tag as SkinningData;
            if (skinningData != null)
            {
                // Create an animation player, and start decoding an animation clip.
                animationPlayer = new AnimationPlayer(skinningData);
                AnimationClip clip = skinningData.AnimationClips["Take 001"];
                animationPlayer.StartClip(clip);
            }
        }

        #region IUpdateable Members
        public void Update()
        {
            if (animationPlayer != null)
            {
                animationPlayer.Update(0.0f, true, transform.world);
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
                Matrix[] bones = new Matrix[0];
                if (animationPlayer != null)
                {
                    bones = animationPlayer.GetSkinTransforms();
                    sharedMesh.model.CopyAbsoluteBoneTransformsTo(boneTransforms);
                }

                SkinnedEffect sEffect = mesh.Effects[e] as SkinnedEffect;
                sEffect.SetBoneTransforms(bones);
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
