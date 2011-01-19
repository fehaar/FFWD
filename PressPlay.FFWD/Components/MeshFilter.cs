using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD.Components
{
    public class MeshFilter : Component
    {
        [ContentSerializer(ElementName="mesh", Optional=true)]
        public Mesh sharedMesh { get; set; }

        private BasicEffect effect;

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
                device.BlendState = materials[0].blendState;
            }
            effect.VertexColorEnabled = false;
            effect.Alpha = 1.0f;

            VertexPositionNormalTexture[] data = new VertexPositionNormalTexture[sharedMesh.vertices.Length];
            for (int i = 0; i < sharedMesh.vertices.Length; i++)
            {
                data[i] = new VertexPositionNormalTexture()
                {
                    Position = sharedMesh.vertices[i],
                    Normal = sharedMesh.normals[i],
                    TextureCoordinate = sharedMesh.uv[i]
                };
            }

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
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
