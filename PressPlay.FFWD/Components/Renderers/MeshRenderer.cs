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
    public class MeshRenderer : Component, IRenderable, Interfaces.IUpdateable
    {
        #region Content properties
        [ContentSerializer(Optional=true)]
        public string Texture;
        [ContentSerializer(Optional = true)]
        public string asset;
        [ContentSerializer(Optional = true)]
        public string mesh;
        #endregion

        [ContentSerializerIgnore]
        public Model model;
        [ContentSerializerIgnore]
        public Texture2D texture;

        private Matrix[] boneTransforms;
        private AnimationPlayer animationPlayer;
        private int meshIndex = 0;

        public override void Awake()
        {
            base.Awake();
            ContentHelper.LoadModel(asset);
            ContentHelper.LoadTexture(Texture);
        }

        public override void Start()
        {
            base.Start();
            model = ContentHelper.GetModel(asset);
            texture = ContentHelper.GetTexture(Texture);

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
            if (model == null)
            {
                return;
            }
            
            Matrix world = transform.world;
            
            // Draw the model.
            ModelMesh mesh = model.Meshes[meshIndex];
            for (int e = 0; e < mesh.Effects.Count; e++)
            {
                if (mesh.Name.Contains("_collider"))
                {
                    continue;
                }
                if (mesh.Effects[e] is BasicEffect)
                {
                    BasicEffect effect = mesh.Effects[e] as BasicEffect;
                    effect.World = Matrix.CreateRotationX(MathHelper.PiOver2) * world;
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
                        model.CopyAbsoluteBoneTransformsTo(boneTransforms);
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
        #endregion
    }
}
