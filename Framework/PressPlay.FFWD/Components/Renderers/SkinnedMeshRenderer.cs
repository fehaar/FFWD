using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Components
{
    public class SkinnedMeshRenderer : Renderer
    {
        [ContentSerializer(ElementName="bones", Optional=true)]
        internal string[] boneIds;
        public Mesh sharedMesh;
        private Mesh mesh;
        private Matrix[] bindPoses;
        [ContentSerializerIgnore]
        public Transform[] bones;

        private Animation animation;

        public override void Awake()
        {
            base.Awake();
            if (boneIds != null && bones == null)
            {
                bones = new Transform[boneIds.Length];
                bindPoses = new Matrix[boneIds.Length];
                for (int i = 0; i < boneIds.Length; i++)
                {
                    bones[i] = transform.parent.FindChild("//" + boneIds[i]);
                    if (bones[i] == null)
                    {
                        Debug.LogError(string.Format("The bone {0} did not exist for the renderer {1}.", boneIds[i], this));
                    }
                }
            }
            animation = GetComponentInParents<Animation>();
            // Get a local mesh copy that we can molest
            mesh = (Mesh)sharedMesh.Clone();
        }

        #region IRenderable Members
        public override void Draw(GraphicsDevice device, Camera cam)
        {
            if (sharedMesh == null)
            {
                return;
            }

            // Check the frustum of the camera
            BoundingSphere sphere = new BoundingSphere(transform.position, sharedMesh.bounds.boundingSphere.Radius * transform.lossyScale.sqrMagnitude);
            if (cam.DoFrustumCulling(ref sphere))
            {
#if DEBUG
                if (Camera.logRenderCalls)
                {
                    Debug.LogFormat("VP cull {0} with radius {1} pos {2} cam {3} at {4}", gameObject, sharedMesh.bounds.boundingSphere.Radius, transform.position, cam.gameObject, cam.transform.position);
                }
#endif
                return;
            }

            if (sharedMesh.boneWeights != null)
            {
                // This is the rendering of data gotten directly from Unity
                //Matrix world = transform.world;
                Matrix world = Matrix.Identity;
                // Find the current bone data from the bone Transform.
                for (int i = 0; i < bones.Length; i++)
                {
                    bindPoses[i] = Matrix.Identity;
                    //bindPoses[i] = bones[i].world * Matrix.Invert(transform.world);
                    bindPoses[i] = sharedMesh.bindPoses[i] * bones[i].world;
                    //Debug.Log(bones[i].name + " : " + bones[i].localPosition);
                }

                // We have blended parts that does not come from a bone structure
                for (int i = 0; i < sharedMesh._vertices.Length; i++)
                {
                    CpuSkinningHelpers.SkinVertex(
                        bindPoses,
                        ref sharedMesh._vertices[i],
                        ref sharedMesh._normals[i],
                        ref world,
                        ref sharedMesh.boneWeights[i],
                        out mesh._vertices[i],
                        out mesh._normals[i]);
                }
                cam.BatchRender(mesh, sharedMaterials, null);
            }
        }
        #endregion
    }
}