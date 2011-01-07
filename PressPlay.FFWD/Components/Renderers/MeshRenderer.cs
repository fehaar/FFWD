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
            if (filter.CanDraw())
            {
                filter.Draw(device, cam, materials);
                return;
            }

            Matrix world = transform.world;

            // Do we have negative scale - if so, switch culling
            RasterizerState oldRaster = device.RasterizerState;
            if (transform.lossyScale.x < 0 || transform.lossyScale.y < 0 || transform.lossyScale.z < 0)
            {
                device.RasterizerState = new RasterizerState() { FillMode = oldRaster.FillMode, CullMode = CullMode.CullClockwiseFace };
            }
            device.BlendState = material.blendState;

            // Draw the model.
            ModelMesh mesh = filter.GetModelMesh();
            for (int e = 0; e < mesh.Effects.Count; e++)
            {
                BasicEffect effect = mesh.Effects[e] as BasicEffect;
                effect.World = world;
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

            if (transform.lossyScale.x < 0 || transform.lossyScale.y < 0 || transform.lossyScale.z < 0)
            {
                device.RasterizerState = oldRaster;
            }
        }
        #endregion
    }
}
