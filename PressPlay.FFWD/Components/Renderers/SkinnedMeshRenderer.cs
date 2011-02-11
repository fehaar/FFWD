using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD;
using PressPlay.FFWD.SkinnedModel;

namespace PressPlay.FFWD.Components
{
    public class SkinnedMeshRenderer : Renderer
    {

        public Mesh sharedMesh { get; set; }

        private Animation animation;

        public override void Awake()
        {
            base.Awake();
            // Create animation players/clips for the rigid model
            animation = GetComponentInParents<Animation>();
            //if ((sharedMesh.skinnedModel != null) && (sharedMesh.skinnedModel.SkinningData != null))
            //{
            //    if (sharedMesh.skinnedModel.SkinningData.AnimationClips != null)
            //    {
            //        animation.Initialize(sharedMesh.skinnedModel.SkinningData);
            //    }
            //}
        }

        #region IRenderable Members
        public override int Draw(GraphicsDevice device, Camera cam)
        {
            if (sharedMesh == null || sharedMesh.skinnedModel == null)
            {
                return 0;
            }

            // Check the frustum of the camera
            BoundingSphere sphere = new BoundingSphere(transform.position, sharedMesh.boundingSphere.Radius);
            if (cam.DoFrustumCulling(ref sphere))
            {
                return 0;
            }

            // Do we have negative scale - if so, switch culling
            //RasterizerState oldRaster = device.RasterizerState;
            //if (transform.lossyScale.x < 0 || transform.lossyScale.y < 0 || transform.lossyScale.z < 0)
            //{
            //    device.RasterizerState = new RasterizerState() { FillMode = oldRaster.FillMode, CullMode = CullMode.CullClockwiseFace };
            //}
            material.SetBlendState(device);

            // Draw the model.
            CpuSkinnedModelPart modelPart = sharedMesh.GetSkinnedModelPart();
            Matrix world = sharedMesh.skinnedModel.BakedTransform * transform.world;
            modelPart.SetBones(animation.GetTransforms(), ref world);

            cam.BatchRender(modelPart, sharedMaterial, transform);

            //modelPart.Effect.World = transform.world;
            //modelPart.Effect.View = cam.view;
            //modelPart.Effect.Projection = cam.projectionMatrix;
            //if (material.texture != null)
            //{
            //    modelPart.Effect.TextureEnabled = true;
            //    modelPart.Effect.Texture = material.texture;
            //}
            //else
            //{
            //    modelPart.Effect.DiffuseColor = material.color;
            //}

            //modelPart.Draw();

            //if (transform.lossyScale.x < 0 || transform.lossyScale.y < 0 || transform.lossyScale.z < 0)
            //{
            //    device.RasterizerState = oldRaster;
            //}
            return 1;
        }
        #endregion
    }
}