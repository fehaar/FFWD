using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Components
{
    public class MeshFilter : Component
    {
        [ContentSerializer(ElementName="mesh", Optional=true)]
        public Mesh sharedMesh { get; set; }

        private Mesh _mesh;
        [ContentSerializerIgnore]
        public Mesh mesh 
        { 
            get
            {
                if (_mesh == null)
                {
                    _mesh = (Mesh)sharedMesh.Clone();
                }
                return _mesh;
            }
            set
            {
                _mesh = value;
            }
        }

        private BasicEffect effect;
        public BoundingSphere boundingSphere
        {
            get
            {
                if (sharedMesh != null)
                {
                    return sharedMesh.boundingSphere;
                }
                return new BoundingSphere();
            }
        }
        //private VertexPositionTexture[] data = new VertexPositionTexture[0];

        internal bool CanBatch()
        {
            return (sharedMesh != null && sharedMesh.vertices != null);
        }

        public ModelMesh GetModelMesh()
        {
            if (sharedMesh != null)
            {
                return sharedMesh.GetModelMesh();
            }
            return null;
        }
    }
}
