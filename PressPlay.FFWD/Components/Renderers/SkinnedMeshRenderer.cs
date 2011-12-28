using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD;
using PressPlay.FFWD.SkinnedModel;

namespace PressPlay.FFWD.Components
{
    public class SkinnedMeshRenderer : Renderer
    {
        public Mesh sharedMesh;
        private Mesh mesh;
        private Matrix[] bones;

        private Animation animation;

        public override void Awake()
        {
            base.Awake();
            animation = GetComponentInParents<Animation>();
            if (sharedMesh.blendIndices != null)
            {
                mesh = (Mesh)sharedMesh.Clone();
                bones = new Matrix[sharedMesh.boneIndices.Count];
            }
        }

        #region IRenderable Members
        public override int Draw(GraphicsDevice device, Camera cam)
        {
            if (sharedMesh == null)
            {
                return 0;
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
                return 0;
            }

            if (sharedMesh.skinnedModel != null)
            {
                // Draw the skinned model with the animation as CPU animation.
                CpuSkinnedModelPart modelPart = sharedMesh.GetSkinnedModelPart();
                Matrix world = transform.world;
                modelPart.SetBones(animation.GetTransforms(), ref world, sharedMesh);
                return cam.BatchRender(sharedMesh, materials, null);
            }
            else if (sharedMesh.blendIndices == null)
            {
                // We do not have bone animation - so just render as a normal model
                return cam.BatchRender(sharedMesh, materials, transform);
            }
            else
            {
                Matrix world = transform.world;
                // Find the current bone data from the bone Transform.
                for (int i = 0; i < bones.Length; i++)
                {
                    bones[i] = Matrix.Identity;
                }

                // We have blended parts that does not come from a bone structure
                for (int i = 0; i < sharedMesh.vertices.Length; i++)
                {
                    CpuSkinningHelpers.SkinVertex(
                        bones,
                        ref sharedMesh.vertices[i],
                        ref sharedMesh.normals[i],
                        ref world,
                        ref sharedMesh.blendIndices,
                        ref sharedMesh.blendWeights[i],
                        out mesh.vertices[i],
                        out mesh.normals[i]);
                }
                return cam.BatchRender(mesh, materials, null);
            }

        }
        #endregion
    }
}