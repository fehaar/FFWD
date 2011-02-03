using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        public override void Draw(GraphicsDevice device, Camera cam)
        {
            if (filter == null)
            {                
                return;
            }
            if (filter.CanBatch())
            {
                cam.BatchRender(filter, material, transform);
                return;
            }

            Matrix world = transform.world;

            //viewport culling
            //if (!filter.IsInBoundingFrustrum(cam.GetBoundingFrustum(), world))
            //{
            //    return;
            //}

            // Draw the model.
            ModelMesh mesh = filter.GetModelMesh();
            if (mesh != null)
            {
                material.SetBlendState(device);

                for (int e = 0; e < mesh.Effects.Count; e++)
                {
                    BasicEffect effect = mesh.Effects[e] as BasicEffect;
                    if (filter.sharedMesh.model.Tag is GameObjectAnimationData)
                    {
                        effect.World = (filter.sharedMesh.model.Tag as GameObjectAnimationData).BakedTransform * world;
                    }
                    else
                    {
                        effect.World = world;
                    }
                    effect.View = cam.View();
                    effect.Projection = cam.projectionMatrix;
                    effect.LightingEnabled = false;
                    if (material.texture != null)
                    {
                        effect.TextureEnabled = true;
                        effect.Texture = material.texture;
                    }
                    else
                    {
                        effect.DiffuseColor = (Vector3)material.color;
                    }
                    mesh.Draw();
                }
            }

        }
        #endregion
    }
}
