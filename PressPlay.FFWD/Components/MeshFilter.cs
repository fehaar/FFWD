using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Components
{
    public class MeshFilter : Component
    {
        [ContentSerializer(ElementName="mesh", Optional=true)]
        public Mesh sharedMesh { get; set; }

        public bool isStatic;

        private Mesh _mesh;
        [ContentSerializerIgnore]
        public Mesh mesh 
        { 
            get
            {
                if ((_mesh == null) && (sharedMesh != null))
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

        public BoundingSphere boundingSphere
        {
            get
            {
                if (_mesh != null)
                {
                    return _mesh.boundingSphere;
                }
                if (sharedMesh != null)
                {
                    return sharedMesh.boundingSphere;
                }
                return new BoundingSphere();
            }
        }
        //private VertexPositionTexture[] data = new VertexPositionTexture[0];

        public override void Awake()
        {
            base.Awake();
            // Do this to force the mesh to get cloned on awake if it is already set.
            // If sharedMesh is changed later the clone will happen there.
            Mesh mesh = this.mesh;
        }

        internal bool CanBatch()
        {
            return (mesh != null && mesh.vertices != null);
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
