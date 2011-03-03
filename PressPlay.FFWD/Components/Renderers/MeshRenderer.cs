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

            BoundingSphere sphere = new BoundingSphere(transform.position, filter.boundingSphere.Radius * transform.lossyScale.sqrMagnitude);
            if (cam.DoFrustumCulling(ref sphere))
            {
#if DEBUG
                if (Camera.logRenderCalls)
                {
                    Debug.LogFormat("VP cull {0} with radius {1} pos {2} cam {3} at {4}", gameObject, filter.boundingSphere.Radius, transform.position, cam.gameObject, cam.transform.position);
                }
#endif
                return 0;
            }

            if (filter.CanBatch())
            {
                return cam.BatchRender(filter, material, transform);
            }

            Matrix world = transform.world;

            // Draw the model.
            ModelMesh mesh = filter.GetModelMesh();
            if (mesh != null)
            {
#if DEBUG
                if (Camera.logRenderCalls)
                {
                    Debug.LogFormat("Mesh: {0} on {1}", gameObject, cam.gameObject);
                }
#endif

                material.SetBlendState(device);

                for (int e = 0; e < mesh.Effects.Count; e++)
                {
                    BasicEffect effect = mesh.Effects[e] as BasicEffect;
                    //if (filter.sharedMesh.model.Tag is GameObjectAnimationData)
                    //{
                    //    effect.World = (filter.sharedMesh.model.Tag as GameObjectAnimationData).BakedTransform * world;
                    //}
                    //else
                    //{
                        effect.World = world;
                    //}
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
                return 1;
            }
            return 0;
        }
        #endregion
    }
}
