using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD.SkinnedModel;

namespace PressPlay.FFWD
{
    public class Mesh : Asset
    {
        [ContentSerializer(Optional = true)]
        public string asset { get; set; }

        private int meshIndex;

        [ContentSerializer(Optional=true)]
        public Microsoft.Xna.Framework.Vector3[] vertices;
        [ContentSerializer(Optional = true)]
        public Microsoft.Xna.Framework.Vector3[] normals;
        [ContentSerializer(Optional = true)]
        public Microsoft.Xna.Framework.Vector2[] uv;
        [ContentSerializer(Optional = true)]
        public short[] triangles;
        [ContentSerializer(Optional = true)]
        private short[][] triangleSets;
        [ContentSerializer(Optional = true)]
        internal BoneWeight[] boneWeights;
        [ContentSerializer(Optional = true)]
        internal Matrix[] bindPoses;

        internal Dictionary<string, byte> boneIndices;
        internal byte[] blendIndices;
        internal Microsoft.Xna.Framework.Vector4[] blendWeights;

        [ContentSerializer(ElementName="bounds", Optional=true)]
        public Bounds bounds;

        protected override void DoLoadAsset(AssetHelper assetHelper)
        {
            // If this is a static mesh, we do not need to load the data
            if (vertices != null)
            {
                if (triangleSets != null && triangles == null)
                {
                    FlattenTriangleSets();
                }
                return;
            }

            if (!String.IsNullOrEmpty(asset))
            {
                MeshData data = assetHelper.Load<MeshData>("Models/" + asset);
                if (data != null)
                {
                    if (data.meshParts.Count > 0)
                    {
                        MeshDataPart part = data.meshParts[name];
                        if (part != null)
                        {
                            boneIndices = data.boneIndices;
                            blendIndices = part.blendIndices;
                            blendWeights = part.blendWeights;
                            vertices = (Microsoft.Xna.Framework.Vector3[])part.vertices.Clone();
                            triangleSets = (short[][])part.triangles.Clone();
                            FlattenTriangleSets();
                            uv = (Microsoft.Xna.Framework.Vector2[])part.uv.Clone();
                            if (part.normals != null)
                            {
                                normals = (Microsoft.Xna.Framework.Vector3[])part.normals.Clone();
                            }
                        }
                        return;
                    }
                }
#if DEBUG
                else
                {
                    Debug.LogWarning("Cannot find a way to load the mesh " + asset + "/" + name);
                }
#endif
            }
        }

        private void FlattenTriangleSets()
        {
            int triCount = 0;
            int triIndex = 0;
            for (int i = 0; i < triangleSets.Length; i++)
            {
                triCount += triangleSets[i].Length;
            }
            triangles = new short[triCount];
            for (int i = 0; i < triangleSets.Length; i++)
            {
                triangleSets[i].CopyTo(triangles, triIndex);
                triIndex += triangleSets[i].Length;
            }
        } 

        public void Clear()
        {
            vertices = null;
            normals = null;
            uv = null;
            triangles = null;
            triangleSets = null;
        }

        public int subMeshCount
        {
            get
            {
                if (triangleSets == null)
                {
                    if (triangles == null)
                    {
                        return 0;
                    }
                    return 1;
                }
                return triangleSets.Length;
            }
        }

        public short[] GetTriangles(int subMeshIndex)
        {
            if (triangleSets == null && subMeshIndex == 0)
            {
                return triangles;
            }
            return triangleSets[subMeshIndex];
        }

        #region ICloneable Members
        internal override UnityObject Clone()
        {
            Mesh clone = new Mesh();
            clone.meshIndex = meshIndex;

            if (vertices != null)
            {
                clone.vertices = (Microsoft.Xna.Framework.Vector3[])vertices.Clone();
                clone.triangles = (short[])triangles.Clone();
                clone.triangleSets = (short[][])triangleSets.Clone();
                clone.uv = (Microsoft.Xna.Framework.Vector2[])uv.Clone();
                if (normals != null)
                {
                    clone.normals = (Microsoft.Xna.Framework.Vector3[])normals.Clone();
                }
            }
            clone.bounds = bounds;
            // Note that these are not actually cloned as they will not be changed
            clone.blendIndices = blendIndices;
            clone.blendWeights = blendWeights;
            return clone;
        }
        #endregion

        public override string ToString()
        {
            return String.Format("{0} - {1}/{2} ({3})", GetType().Name, asset, name, GetInstanceID());
        }

    }
}
