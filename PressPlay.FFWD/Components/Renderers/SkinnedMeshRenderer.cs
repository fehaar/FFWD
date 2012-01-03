using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD;
using PressPlay.FFWD.SkinnedModel;
using Microsoft.Xna.Framework.Content;

namespace PressPlay.FFWD.Components
{
    public class SkinnedMeshRenderer : Renderer
    {
        [ContentSerializer(ElementName="bones", Optional=true)]
        internal int[] boneIds;
        public Mesh sharedMesh;
        private Mesh mesh;
        private Matrix[] bindPoses;
        [ContentSerializerIgnore]
        public Transform[] bones;

        private Animation animation;

        internal override void FixReferences(System.Collections.Generic.Dictionary<int, UnityObject> idMap)
        {
            base.FixReferences(idMap);
            if (boneIds != null)
            {
                bones = new Transform[boneIds.Length];
                bindPoses = new Matrix[boneIds.Length];
                for (int i = 0; i < boneIds.Length; i++)
                {
                    bones[i] = idMap[boneIds[i]] as Transform;
                }
            }
        }

        public override void Awake()
        {
            base.Awake();
            animation = GetComponentInParents<Animation>();
            mesh = (Mesh)sharedMesh.Clone();
            if (sharedMesh.blendIndices != null)
            {
                bindPoses = new Matrix[sharedMesh.boneIndices.Count];
                bones = new Transform[sharedMesh.boneIndices.Count];
                foreach (var name in sharedMesh.boneIndices.Keys)
                {
                    bones[sharedMesh.boneIndices[name]] = transform.parent.FindChild("//" + name);
                }
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
            else if (sharedMesh.boneWeights != null)
            {
                // We do not have bone animation - so just render as a normal model
                Matrix world = transform.world;
                // Find the current bone data from the bone Transform.
                for (int i = 0; i < bones.Length; i++)
                {
                    bindPoses[i] = Matrix.Identity;// Matrix.Invert(sharedMesh.bindPoses[i]);
                }

                // We have blended parts that does not come from a bone structure
                for (int i = 0; i < sharedMesh.vertices.Length; i++)
                {
                    CpuSkinningHelpers.SkinVertex(
                        bindPoses,
                        ref sharedMesh.vertices[i],
                        ref sharedMesh.normals[i],
                        ref world,
                        ref sharedMesh.boneWeights[i],
                        out mesh.vertices[i],
                        out mesh.normals[i]);
                }
                return cam.BatchRender(mesh, materials, null);
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
                for (int i = 0; i < bindPoses.Length; i++)
                {
                    bindPoses[i] = Matrix.Identity; // bones[i].world;
                }

                // We have blended parts that does not come from a bone structure
                for (int i = 0; i < sharedMesh.vertices.Length; i++)
                {
                    CpuSkinningHelpers.SkinVertex(
                        bindPoses,
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