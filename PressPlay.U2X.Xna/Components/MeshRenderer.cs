using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PressPlay.U2X.Xna.Animation;
using PressPlay.U2X.Xna.Interfaces;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.U2X.Xna.Components
{
    public class MeshRenderer : Component, IRenderable, Interfaces.IUpdateable
    {
        [ContentSerializerIgnore]
        public Model model;
        [ContentSerializerIgnore]
        public Texture2D texture;
        private Matrix[] boneTransforms;
        private AnimationPlayer animationPlayer;

        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            base.Start();

            boneTransforms = new Matrix[model.Bones.Count];
            // Look up our custom skinning information.
            SkinningData skinningData = model.Tag as SkinningData;
            if (skinningData != null)
            {
                // Create an animation player, and start decoding an animation clip.
                animationPlayer = new AnimationPlayer(skinningData);
                AnimationClip clip = skinningData.AnimationClips["Take 001"];
                animationPlayer.StartClip(clip);
            }
        }

        #region IUpdateable Members

        public void Update(GameTime gameTime)
        {
            if (animationPlayer != null)
            {
                animationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
            }
        }

        #endregion

        #region IRenderable Members
        public void Draw(SpriteBatch batch)
        {
            Matrix world = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);

            if (animationPlayer != null)
            {
                model.CopyAbsoluteBoneTransformsTo(boneTransforms);
            }

            // Draw the model.
            for (int i = 0; i < model.Meshes.Count; i++)
            {
                ModelMesh mesh = model.Meshes[i];
                for (int e = 0; e < mesh.Effects.Count; e++)
                {
                    if (mesh.Effects[e] is BasicEffect)
                    {
                        BasicEffect effect = mesh.Effects[e] as BasicEffect;
                        effect.World = world;
                        effect.View = Camera.main.View();
                        effect.Projection = Camera.main.projectionMatrix;
                        effect.LightingEnabled = false;
                        effect.Texture = texture;
                        effect.TextureEnabled = true;
                    }
                    if (mesh.Effects[e] is SkinnedEffect)
                    {
                        Matrix[] bones = new Matrix[0];
                        if (animationPlayer != null)
                        {
                            bones = animationPlayer.GetSkinTransforms();
                        }

                        SkinnedEffect sEffect = mesh.Effects[e] as SkinnedEffect;
                        sEffect.SetBoneTransforms(bones);
                        sEffect.World = world;
                        sEffect.View = Camera.main.View();
                        sEffect.Projection = Camera.main.projectionMatrix;
                        sEffect.EnableDefaultLighting();
                        sEffect.SpecularColor = new Vector3(0.25f);
                        sEffect.SpecularPower = 16;
                        sEffect.Texture = texture;
                    }
                    mesh.Draw();
                }
            }
            
        }
        #endregion
    }
}
