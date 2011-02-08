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
        public override int Draw(GraphicsDevice device, Camera cam)
        {
            if (filter == null)
            {                
                return 0;
            }
            if (filter.CanBatch())
            {
                return cam.BatchRender(filter, material, transform);
            }

            Matrix world = transform.world;

            BoundingSphere sphere = new BoundingSphere(transform.position, filter.boundingSphere.Radius);
            if (cam.DoFrustumCulling(ref sphere))
            {
                return 0;
            }

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
                    effect.View = cam.view;
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
            return 1;
        }
        #endregion
    }
}
