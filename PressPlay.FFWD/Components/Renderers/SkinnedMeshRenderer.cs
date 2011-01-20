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
        }

        #region IRenderable Members
        public override void Draw(GraphicsDevice device, Camera cam)
        {
            if (sharedMesh == null || sharedMesh.skinnedModel == null)
            {
                return;
            }
            
            // Do we have negative scale - if so, switch culling
            RasterizerState oldRaster = device.RasterizerState;
            if (transform.lossyScale.x < 0 || transform.lossyScale.y < 0 || transform.lossyScale.z < 0)
            {
                device.RasterizerState = new RasterizerState() { FillMode = oldRaster.FillMode, CullMode = CullMode.CullClockwiseFace };
            }
            device.RasterizerState = new RasterizerState() { FillMode = oldRaster.FillMode, CullMode = CullMode.None };
            device.BlendState = material.blendState;

            // Draw the model.
            CpuSkinnedModelPart modelPart = sharedMesh.GetSkinnedModelPart();
            modelPart.SetBones(animation.GetTransforms());

            modelPart.Effect.World = sharedMesh.skinnedModel.SkinningData.BakedTransform * transform.world;
            modelPart.Effect.View = cam.View();
            modelPart.Effect.Projection = cam.projectionMatrix;
            if (material.texture != null)
            {
                modelPart.Effect.TextureEnabled = true;
                modelPart.Effect.Texture = material.texture;
            }
            else
            {
                modelPart.Effect.DiffuseColor = (Vector3)material.color;
            }

            modelPart.Draw();

            device.RasterizerState = oldRaster;
            if (transform.lossyScale.x < 0 || transform.lossyScale.y < 0 || transform.lossyScale.z < 0)
            {
                device.RasterizerState = oldRaster;
            }
        }
        #endregion
    }
}