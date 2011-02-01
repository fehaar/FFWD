using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Components
{
    public class MeshFilter : Component
    {
        [ContentSerializer(ElementName="mesh", Optional=true)]
        public Mesh sharedMesh { get; set; }

        private BasicEffect effect;
        private BoundingSphere boundingSphere = new BoundingSphere();
        private VertexPositionTexture[] data = new VertexPositionTexture[0];

        internal bool CanDraw()
        {
            return (sharedMesh != null && sharedMesh.model == null);
        }

        public ModelMesh GetModelMesh()
        {
            if (sharedMesh != null)
            {
                return sharedMesh.GetModelMesh();
            }
            return null;
        }

        internal bool IsInBoundingFrustrum(BoundingFrustum boundingFrustum, Matrix world)
        {
            if (boundingSphere.Radius == 0)
            {
                sharedMesh.model.Meshes[0].BoundingSphere.Transform(ref world, out boundingSphere);
            }

            return boundingSphere.Intersects(boundingFrustum);
        }

        internal void Draw(GraphicsDevice device, Camera cam, Material[] materials)
        {
            if (sharedMesh.vertices == null)
            {
                return;
            }
            if (effect == null)
            {
                effect = new BasicEffect(device);
            }

            RasterizerState oldrasterizerState = device.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;

            effect.World = transform.world;
            effect.View = cam.View();
            effect.Projection = cam.projectionMatrix;
            if (materials != null && materials.Length > 0 && materials[0].texture != null)
            {
                effect.TextureEnabled = true;
                effect.Texture = materials[0].texture;
                materials[0].SetBlendState(device);
            }
            effect.VertexColorEnabled = false;
            effect.Alpha = 1.0f;

            // TODO: This can be optimized by not recreating data every time
            if (data.Length < sharedMesh.vertices.Length)
            {
                data = new VertexPositionTexture[sharedMesh.vertices.Length];
            }
            for (int i = 0; i < sharedMesh.vertices.Length; i++)
            {
                data[i] = new VertexPositionTexture()
                {
                    Position = sharedMesh.vertices[i],
                    TextureCoordinate = sharedMesh.uv[i]
                };
            }

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives<VertexPositionTexture>(
                    PrimitiveType.TriangleList,
                    data,
                    0,
                    data.Length,
                    sharedMesh.triangles,
                    0,
                    sharedMesh.triangles.Length / 3
                );
            }

            device.RasterizerState = oldrasterizerState;
        }
    }
}
