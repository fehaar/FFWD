using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD.Extensions;

namespace PressPlay.FFWD
{
    public class Mesh : Asset
    {
        private int meshIndex;

        [ContentSerializer(ElementName="vertices", Optional=true)]
        internal Microsoft.Xna.Framework.Vector3[] _vertices;
        [ContentSerializerIgnore]
        public Vector3[] vertices
        {
            get
            {
                return _vertices.Cast<Vector3>().ToArray();
            }
            set
            {
                _vertices = value.Cast<Microsoft.Xna.Framework.Vector3>().ToArray();
            }
        }

        [ContentSerializer(ElementName = "normals", Optional = true)]
        internal Microsoft.Xna.Framework.Vector3[] _normals;
        [ContentSerializerIgnore]
        public Vector3[] normals
        {
            get
            {
                return _normals.Cast<Vector3>().ToArray();
            }
            set
            {
                _normals = value.Cast<Microsoft.Xna.Framework.Vector3>().ToArray();
            }
        }

        [ContentSerializer(Optional = true)]
        public Vector4[] tangents;

        [ContentSerializer(ElementName = "uv", Optional = true)]
        internal Microsoft.Xna.Framework.Vector2[] _uv;
        [ContentSerializerIgnore]
        public Vector2[] uv
        {
            get
            {
                return _uv.Cast<Vector2>().ToArray();
            }
            set
            {
                _uv = value.Cast<Microsoft.Xna.Framework.Vector2>().ToArray();
            }
        }

        [ContentSerializer(ElementName = "uv2", Optional = true)]
        internal Microsoft.Xna.Framework.Vector2[] _uv2;
        [ContentSerializerIgnore]
        public Vector2[] uv2
        {
            get
            {
                return _uv2.Cast<Vector2>().ToArray();
            }
            set
            {
                _uv2 = value.Cast<Microsoft.Xna.Framework.Vector2>().ToArray();
            }
        }

        [ContentSerializerIgnore]
        public short[] triangles;
        [ContentSerializer(Optional = true)]
        private short[][] triangleSets;
        [ContentSerializer(Optional = true)]
        public Color[] colors;
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
            if (triangleSets != null && triangles == null)
            {
                FlattenTriangleSets();
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

            throw new NotImplementedException("WE NEED TO DO MORE HERE");
            if (_vertices.HasElements())
            {
                clone._vertices = (Microsoft.Xna.Framework.Vector3[])_vertices.Clone();
            }
            if (vertices != null)
            {
                clone.triangles = (short[])triangles.Clone();
                clone.triangleSets = (short[][])triangleSets.Clone();
                clone._uv = (Microsoft.Xna.Framework.Vector2[])_uv.Clone();
                if (normals != null)
                {
                    clone._normals = (Microsoft.Xna.Framework.Vector3[])_normals.Clone();
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
            return String.Format("{0} - {1} ({3})", GetType().Name, name, GetInstanceID());
        }

    }
}
